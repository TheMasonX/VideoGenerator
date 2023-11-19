using System.Drawing;
using System.Text.RegularExpressions;

using VideoGenerator.Models;
using VideoGenerator.Utils.Video;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Tests;

[TestClass]
public partial class VideoUtilsTests
{
    [GeneratedRegex(@"(\.png)|(\.bmp)|(\.jpg)")]
    private static partial Regex GetImageFileRegex ();
    private static readonly Regex imageRegex = GetImageFileRegex();
    [TestMethod]
    public void CreateVideo ()
    {
        //string folder = $"{Environment.GetEnvironmentVariable("UserProfile")}/Pictures";
        //string[] files = Directory.GetFiles(folder).Where(f => imageRegex.IsMatch(f)).ToArray();
        var files = Enumerable.Repeat(".\uv_test.png", 50).ToArray();
        List<Image> images = new(files.Length);
        foreach (string file in files)
        {
            images.Add(Image.FromFile(file));
        }

        VideoUtils.CreateVideo(images, $"{folder}/Video.mp4", TimeSpan.FromSeconds(20), new OpenCvSharp.Size(1920, 1080), VideoUtils.EdgeFilter);
    }

    [TestMethod]
    public void CreateVideo_EdgeFilter ()
    {
        //string folder = $"{Environment.GetEnvironmentVariable("UserProfile")}/Pictures";
        //string[] files = Directory.GetFiles(folder).Where(f => imageRegex.IsMatch(f)).ToArray();
        var files = Enumerable.Repeat(".\uv_test.png", 50).ToArray();
        List<Image> images = new(files.Length);
        foreach (string file in files)
        {
            images.Add(Image.FromFile(file));
        }

        VideoUtils.CreateVideo(images, $"{folder}/Video.mp4", TimeSpan.FromSeconds(20), new OpenCvSharp.Size(1920, 1080), VideoUtils.EdgeFilter);
    }
}