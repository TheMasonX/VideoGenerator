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

public static class VideoUtils
{


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

    public static void EdgeFilter(this Mat src, Mat dst)
    {
        using Mat gray = new();
        using Mat canny = new();
        using Mat laplacian = new();

        double srcBlend = 0.8;
        double edgeBlend = 1.2;

        Cv2.CvtColor(src, gray, ColorConversionCodes.BGR2GRAY);
        Cv2.Canny(gray, canny, 100, 180);
        Blend(src, srcBlend, canny, edgeBlend, dst);
    }

    public static bool CreateVideo(IEnumerable<Image>? images, string path, TimeSpan duration, Size size, Action<Mat, Mat>? filter = null)
    {
        if (path.IsNullOrEmpty() || images is null || !images.Any()) return false;

        var interpolationFlags = InterpolationFlags.Cubic;
        Trace.WriteLine($"CreateVideo Starting to save {images.Count()} images to {path}");
        double fps = images.Count() / duration.TotalSeconds;
        int frameNumber = 0;
        using VideoWriter writer = new (path, FourCC.H264, fps, size);
        foreach(Image image in images)
        {
            if (image is not Bitmap bitmap) continue;
            frameNumber++;
            //Trace.WriteLine($"Converting #{frameNumber}");
            using Mat src = new (new Size(bitmap.Width, bitmap.Height), MatType.CV_8UC3);
            bitmap.ToMat(src);
            if (src.Empty()) continue;

            using Mat dst = new();
            if (filter is not null)
            {
                using Mat filtered = new();
                filter(src, filtered);
                Cv2.Resize(filtered, dst, size, 0, 0, interpolationFlags);
            }
            else
                Cv2.Resize(src, dst, size, 0, 0, interpolationFlags);

            writer.Write(dst);
            //Trace.WriteLine($"Wrote #{frameNumber}");
        }
        Trace.WriteLine($"CreateVideo finished saving!");
        ////InputArray inputArray = InputArray.Create<>(images);
        return true;
    }
}
