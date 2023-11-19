using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenCvSharp;
using OpenCvSharp.Extensions;

using VideoGenerator.Utils.Extensions;

using Size = OpenCvSharp.Size;

namespace VideoGenerator.Utils.Video;

[Flags]
public enum VideoFilter
{
    None            = 0x00,
    Canny           = 0x01,
    Contours        = 0x02,
    Other           = 0x04,
}

public enum Resolution
{
    None = 0,
    VGA = 1,
    _720 = 2,
    _1080 = 3,
    _1440 = 4,
}

public static class VideoUtils
{
    #region Blend

    public static void Blend (this Mat srcA, Mat srcB, double blend, Mat dst)
    {
        Blend(srcA, 1.0 - blend, srcB, blend, dst);
    }

    public static void Blend(Mat srcA, double blendA, Mat srcB, double blendB, Mat dst)
    {
        int channelsA = srcA.Channels();
        int channelsB = srcB.Channels();
        int channelsMax = Math.Max(channelsA, channelsB);
        if(channelsA < channelsMax)
        {
            using Mat newA = new();
            Cv2.CvtColor(srcA, newA, ColorConversionCodes.GRAY2BGR);
            Cv2.AddWeighted(newA, blendA, srcB, blendB, 0, dst);
            return;
        }
        else if (channelsB < channelsMax)
        {
            using Mat newB = new();
            Cv2.CvtColor(srcB, newB, ColorConversionCodes.GRAY2BGR);
            Cv2.AddWeighted(srcA, blendA, newB, blendB, 0, dst);
            return;
        }

        Cv2.AddWeighted(srcA, blendA, srcB, blendB, 0, dst);
        return;
    }

    #endregion Blend

    #region Filters

    public static void CannyFilter (this Mat src, Mat dst)
    {
        using Mat gray = new();
        using Mat canny = new();

        double srcBlend = 0.8;
        double edgeBlend = 1.2;

        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.Canny(gray, canny, 100, 180);
        Blend(src, srcBlend, canny, edgeBlend, dst);
    }

    public static void ContoursFilter (this Mat src, Mat dst)
    {
        src.CopyTo(dst);
        using Mat gray = new();
        using Mat canny = new();
        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.Canny(gray, canny, 100, 180);
        var contours = canny.FindContoursAsMat(RetrievalModes.Tree, ContourApproximationModes.ApproxNone);
        Cv2.DrawContours(dst, contours, 0, Scalar.Red, 30, LineTypes.AntiAlias);
    }

    public static Action<OpenCvSharp.Mat, OpenCvSharp.Mat>? GetFilterMethod (this VideoFilter filter)
    {
        return filter switch
        {
            VideoFilter.None => null,
            VideoFilter.Canny => VideoUtils.CannyFilter,
            VideoFilter.Contours => VideoUtils.ContoursFilter,
            _ => null,
        };
    }

    #endregion Filters

    #region CreateVideo

    public static bool CreateVideo (IEnumerable<Image>? images, string outputPath, TimeSpan duration, Size ouputResolution, Action<Mat, Mat>? filter = null)
    {
        return CreateVideo(images, outputPath, (images?.Count() ?? 0) / duration.TotalSeconds, ouputResolution, filter);
    }

    public static bool CreateVideo(IEnumerable<Image>? images, string outputPath, double fps, Size ouputResolution, Action<Mat, Mat>? filter = null)
    {
        if (outputPath.IsNullOrEmpty() || images is null || !images.Any()) return false;

        var interpolationFlags = InterpolationFlags.Cubic;
        //Trace.WriteLine($"CreateVideo Starting to save {images.Count()} images to {outputPath}");

        var sw = Stopwatch.StartNew();
        using VideoWriter writer = new (outputPath, FourCC.H264, fps, ouputResolution);
        foreach(Image image in images)
        {
            if (image is not Bitmap bitmap) continue;
            using Mat src = new (new Size(bitmap.Width, bitmap.Height), MatType.CV_8UC3);
            bitmap.ToMat(src);
            if (src.Empty()) continue;

            using Mat dst = new();
            if (filter is not null)
            {
                using Mat filtered = new();
                filter(src, filtered);
                Cv2.Resize(filtered, dst, ouputResolution, 0, 0, interpolationFlags);
            }
            else
                Cv2.Resize(src, dst, ouputResolution, 0, 0, interpolationFlags);

            writer.Write(dst);
        }
        sw.Stop();
        Trace.WriteLine($"CreateVideo finished saving {images.Count()} in {sw.ElapsedMilliseconds}ms!");
        return true;
    }

    #endregion CreateVideo

    public static Size ToSize (this Resolution resolution)
    {
        return resolution switch
        {
            Resolution.None => new(),
            Resolution.VGA => new(620, 480),
            Resolution._720 => new(1280, 720),
            Resolution._1080 => new(1920, 1080),
            Resolution._1440 => new(2540, 1440),
            _ => new(),
        };
    }
}
