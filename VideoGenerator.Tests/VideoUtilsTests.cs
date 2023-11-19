#define USE_LOCAL_FILES

using System.Drawing;
using System.Text.RegularExpressions;

using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;
using VideoGenerator.Utils.Video;
using VideoGenerator.ViewModels;

namespace VideoGenerator.Tests;


[TestClass]
public partial class VideoUtilsTests
{
    private static (string folder, string[] files) GetFolderAndFiles()
    {
#if USE_LOCAL_FILES
        string folder = $"{Environment.GetEnvironmentVariable("UserProfile")}/Pictures";
        string[] files = Directory.GetFiles(folder).Where(f => RegexEx.ImageFile.IsMatch(f)).ToArray();
        return (folder, files);
#else
        string folder = @".\";
        string[] files = Enumerable.Repeat(@".\uv_test.png", 50).ToArray();
        return (folder, files);
#endif
    }

    [TestMethod]
    public void CreateVideo ()
    {
        var folderAndFiles = GetFolderAndFiles();
        string folder = folderAndFiles.folder;
        string[] files = folderAndFiles.files;
        Image[] images = files.Select(f => Image.FromFile(f)).ToArray();
        VideoUtils.CreateVideo(images, $@"{folder}\Video.mp4", TimeSpan.FromSeconds(20), new OpenCvSharp.Size(1920, 1080), VideoUtils.EdgeFilter);
    }

    [TestMethod]
    public void CreateVideo_EdgeFilter ()
    {
        var folderAndFiles = GetFolderAndFiles();
        string folder = folderAndFiles.folder;
        string[] files = folderAndFiles.files;
        Image[] images = files.Select(f => Image.FromFile(f)).ToArray();
        VideoUtils.CreateVideo(images, $@"{folder}\Video_Edge.mp4", TimeSpan.FromSeconds(20), new OpenCvSharp.Size(1920, 1080), VideoUtils.EdgeFilter);
    }
}