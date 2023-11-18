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
    public static bool CreateVideo(IEnumerable<Image>? images, string path, TimeSpan duration, Size size)
    {
        if (path.IsNullOrEmpty() || images is null || !images.Any()) return false;

        Trace.WriteLine($"CreateVideo Starting to save {images.Count()} images to {path}");
        double fps = images.Count() / duration.TotalSeconds;
        int frameNumber = 0;
        using VideoWriter writer = new (path, FourCC.H264, fps, size);
        foreach(Image image in images)
        {
            frameNumber++;
            //Trace.WriteLine($"Converting #{frameNumber}");
            Mat inputFrame = new (new Size(image.Width, image.Height), MatType.CV_8UC3);
            (image as Bitmap)?.ToMat(inputFrame);
            if (inputFrame.Empty()) continue;

            using var dst = new Mat();
            Cv2.Resize(inputFrame, dst, size, 0, 0, InterpolationFlags.Cubic);
            //Trace.WriteLine($"Writing #{frameNumber}");
            writer.Write(dst);
            //Trace.WriteLine($"Wrote #{frameNumber}");
        }
        Trace.WriteLine($"CreateVideo finished saving!");
        ////InputArray inputArray = InputArray.Create<>(images);
        //writer.Write();
        return true;
    }
}
