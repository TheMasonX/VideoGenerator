using System.Drawing;
using System.Text.RegularExpressions;

using VideoGenerator.Models;
using VideoGenerator.Utils.Video;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Tests;

[TestClass]
public class VideoUtilsTests
{
    private static readonly Regex imageRegex = new(@"(\.png)|(\.bmp)|(\.jpg)");
    [TestMethod]
    public void CreateVideoTest ()
    {
        string folder = $"{Environment.GetEnvironmentVariable("UserProfile")}/Pictures";
        string[] files = Directory.GetFiles(folder).Where(f => imageRegex.IsMatch(f)).ToArray();
        List<Image> images = new(files.Length);
        foreach (string file in files)
        {
            images.Add(Image.FromFile(file));
        }

        VideoUtils.CreateVideo(images, $"{folder}/Video.mp4", TimeSpan.FromSeconds(20), new OpenCvSharp.Size(640, 480));
    }
}