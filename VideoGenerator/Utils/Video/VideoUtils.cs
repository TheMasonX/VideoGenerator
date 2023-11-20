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

using OldColor = System.Drawing.Color;
using NewColor = System.Windows.Media.Color;

using Size = OpenCvSharp.Size;
using ColorSpace = OpenCvSharp.ColorConversionCodes;
using ImageFilter = System.Action<OpenCvSharp.Mat, OpenCvSharp.Mat>;

namespace VideoGenerator.Utils.Video;

[Flags]
public enum VideoFilter
{
    None            = 0x00,
    Canny           = 0x01,
    Laplacian       = 0x02,
    Contours        = 0x04,
    HighPass         = 0x08,
    Other           = 0x10,
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

    #region Color Conversion

    /// <summary>
    /// Returns a Scalar in BGR ColorSpace
    /// </summary>
    /// <param name="color"></param>
    /// <returns>Scalar in BGR ColorSpace</returns>
    public static Scalar ToScalar (this Color color)
    {
        return new Scalar(color.B, color.G, color.R);
    }

    #endregion Color Conversion

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
            Cv2.CvtColor(srcA, newA, ColorSpace.GRAY2BGR);
            Cv2.AddWeighted(newA, blendA, srcB, blendB, 0, dst);
            return;
        }
        else if (channelsB < channelsMax)
        {
            using Mat newB = new();
            Cv2.CvtColor(srcB, newB, ColorSpace.GRAY2BGR);
            Cv2.AddWeighted(srcA, blendA, newB, blendB, 0, dst);
            return;
        }

        Cv2.AddWeighted(srcA, blendA, srcB, blendB, 0, dst);
        return;
    }

    #endregion Blend

    #region Masks

    public static void CombineMasksAndNegate (this Mat src, Mat dst, IEnumerable<ImageFilter> masks)
    {
        //PingPong Buffer
        bool outToA = false;
        bool isFirst = true;
        using Mat bufA = new();
        using Mat bufB = new();
        foreach (ImageFilter mask in masks)
        {
            outToA = !outToA;  //gets flipped at the start so we know it's in the same state as it wrote when ready to output
            mask(src, dst);
            if (isFirst)
            {
                isFirst = false;
                Cv2.CopyTo(dst, outToA ? bufA : bufB);
                continue;
            }
            Cv2.BitwiseOr(dst, outToA ? bufB : bufA, outToA ? bufA : bufB);
        }

        Cv2.BitwiseNot(outToA ? bufA : bufB, dst);
    }

    public static void CombineMasks (this Mat src, Mat dst, IEnumerable<ImageFilter> masks)
    {
        //PingPong Buffer
        bool outToA = false;
        bool isFirst = true;
        using Mat bufA = new();
        using Mat bufB = new();
        foreach (ImageFilter mask in masks)
        {
            outToA = !outToA;  //gets flipped at the start so we know it's in the same state as it wrote when ready to output
            mask(src, dst);
            if (isFirst)
            {
                isFirst = false;
                Cv2.CopyTo(dst, outToA ? bufA : bufB);
                continue;
            }
            Cv2.BitwiseOr(dst, outToA ? bufB : bufA, outToA ? bufA : bufB);
        }

        Cv2.CopyTo(outToA ? bufA : bufB, dst);
    }

    public static void GreenBGMask (this Mat src, Mat dst)
    {
        Color minColor = Color.FromArgb(0, 80, 50);
        Color maxColor = Color.FromArgb(60, 255, 125);
        MaskColors(src, dst, minColor, maxColor);
    }

    public static void WhiteBGMask (this Mat src, Mat dst)
    {
        Color minColor = Color.FromArgb(60, 100, 100);
        Color maxColor = Color.FromArgb(255, 255, 255);
        MaskColors(src, dst, minColor, maxColor);
    }

    public static void MaskColors (this Mat src, Mat dst, Color minColor, Color maxColor)
    {
        Cv2.InRange(src, minColor.ToScalar(), maxColor.ToScalar(), dst);
    }

    #endregion


    #region Filters

    public static void CannyFilter (this Mat src, Mat dst)
    {
        using Mat gray = new();
        using Mat canny = new();

        double srcBlend = 0.8;
        double edgeBlend = 1.2;

        Cv2.CvtColor(src, gray, ColorSpace.BGR2GRAY);
        Cv2.Canny(gray, canny, 100, 180);
        Blend(src, srcBlend, canny, edgeBlend, dst);
    }

    public static void LaplacianFilter (this Mat src, Mat dst)
    {
        using Mat gray = new();
        using Mat laplacian = new();

        double srcBlend = 0.8;
        double edgeBlend = 1.5;

        Cv2.CvtColor(src, gray, ColorSpace.BGR2GRAY);
        Cv2.Laplacian(gray, laplacian, MatType.CV_8UC1);
        Blend(src, srcBlend, laplacian, edgeBlend, dst);
    }

    public static void RemoveBGFilter (this Mat src, Mat dst)
    {
        using Mat bgMask = new();
        ImageFilter[] maskFilters = [GreenBGMask, WhiteBGMask];
        CombineMasksAndNegate(src, bgMask, maskFilters);
        Cv2.BitwiseAnd(src, bgMask.CvtColor(ColorSpace.GRAY2BGR), dst);
    }

    public static void ContoursV2Filter (this Mat src, Mat dst)
    {
        src.CopyTo(dst);
        using Mat bgFiltered = new();
        ImageFilter[] maskFilters = [GreenBGMask, WhiteBGMask];
        CombineMasksAndNegate(src, bgFiltered, maskFilters);

        var contours = bgFiltered.FindContoursAsMat(RetrievalModes.CComp, ContourApproximationModes.ApproxTC89L1);
        for (int i = 0; i < contours.Length; i++)
        {
            Cv2.DrawContours(dst, contours, i, Scalar.Blue, 35, LineTypes.Link8);
        }

        Trace.WriteLine($"{contours.Length} Contours Found!");
    }

    public static void ContoursFilter (this Mat src, Mat dst)
    {
        src.CopyTo(dst);
        using Mat gray = new();
        using Mat canny = new();
        using Mat noiseReduction = new();
        //using Mat laplacian = new();
        //using Mat combined = new();

        //Cv2.CvtColor(src, gray, ColorSpace.BGR2GRAY);
        //Cv2.Laplacian(gray, laplacian, MatType.CV_8UC1);
        //Cv2.Canny(gray, canny, 80, 160);
        //Cv2.Add(canny, laplacian, dst);

        using Mat highPass = new();
        using Mat thresholded = new();
        HighPassFilter(src.CvtColor(ColorSpace.BGR2GRAY), highPass);
        //Cv2.CvtColor(src, gray, ColorSpace.BGR2GRAY);
        Cv2.Canny(highPass, canny, 80, 160);
        using Mat blur = new();
        int kernalSize = 15;
        double sigma = 10.0;

        Cv2.GaussianBlur(canny, blur, new(kernalSize, kernalSize), sigma, 0, BorderTypes.Replicate);
        using Mat combined = new();
        Blend(src, .3, blur, 1, dst);
        //return;
        //HighPassFilter(src.CvtColor(ColorSpace.BGR2GRAY), highPass);
        //Cv2.Threshold(highPass, dst, 25, 255, ThresholdTypes.Binary);

        var contours = dst.CvtColor(ColorSpace.BGR2GRAY).FindContoursAsMat(RetrievalModes.Tree, ContourApproximationModes.ApproxSimple);
        Cv2.DrawContours(dst, contours, 0, Scalar.Blue, 35, LineTypes.Link8);
        Trace.WriteLine($"{contours.Length} Contours Found!");
    }

    public static void HighPassFilter (this Mat src, Mat dst)
    {
        using Mat blur = new();
        using Mat diff = new();
        int kernalSize = 35;
        double sigma = 25.0;

        Cv2.GaussianBlur(src, blur, new(kernalSize, kernalSize), sigma, 0, BorderTypes.Replicate);
        Cv2.Subtract(src, blur, diff);
        Cv2.Multiply(diff, 5, dst);
    }

    public static ImageFilter? GetFilterMethod (this VideoFilter filter)
    {
        return filter switch
        {
            VideoFilter.None => null,
            VideoFilter.Canny => CannyFilter,
            VideoFilter.Laplacian => LaplacianFilter,
            VideoFilter.Contours => ContoursV2Filter,
            VideoFilter.HighPass => HighPassFilter,
            VideoFilter.Other => RemoveBGFilter,
            _ => null,
        };
    }

    #endregion Filters

    #region Create Video

    public static bool CreateVideo (IEnumerable<Image>? images, string outputPath, TimeSpan duration, Size ouputResolution, ImageFilter? filter = null)
    {
        return CreateVideo(images, outputPath, (images?.Count() ?? 0) / duration.TotalSeconds, ouputResolution, filter);
    }

    public static bool CreateVideo(IEnumerable<Image>? images, string outputPath, double fps, Size ouputResolution, ImageFilter? filter = null)
    {
        if (outputPath.IsNullOrEmpty() || images is null || !images.Any()) return false;

        var interpolationFlags = InterpolationFlags.Cubic;

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
        Trace.WriteLine($"CreateVideo finished saving {images.Count()} took {sw.Elapsed}!");
        return true;
    }

    #endregion Create Video
}
