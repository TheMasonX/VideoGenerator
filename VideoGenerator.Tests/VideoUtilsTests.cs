//#define USE_LOCAL_FILES

using System.Diagnostics;
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

        string folder = @$"{Environment.GetEnvironmentVariable("UserProfile")}\Pictures\3D Scans\Owl Scans\Scan 3";
        string[] files = Directory.GetFiles(folder).Where(f => RegexEx.ImageFile.IsMatch(f)).ToArray();
        return (folder, files);
#else
        string folder = @".\";
        string[] files = Enumerable.Repeat(@".\contours_test.png", 20).ToArray();
        return (folder, files);
#endif
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow(VideoFilter.Canny)]
    [DataRow(VideoFilter.Laplacian)]
    [DataRow(VideoFilter.Contours)]
    [DataRow(VideoFilter.HighPass)]
    [DataRow(VideoFilter.Other)]
    public void CreateVideo (VideoFilter filter)
    {
        var folderAndFiles = GetFolderAndFiles();
        string folder = folderAndFiles.folder;
        string[] files = folderAndFiles.files;

        var sw = Stopwatch.StartNew();
        //var images = files.Select(f => Image.FromFile(f)).ToArray();
        var images = files.AsParallel().Select(f => Image.FromFile(f)).Take(10).ToArray();
        sw.Stop();
        Trace.WriteLine($"Creating image array took {sw}");
        var filterMethod = filter.GetFilterMethod();
        VideoUtils.CreateVideo(images, $@"{Environment.CurrentDirectory}\Video{(filterMethod is null ? "" : $"_{filter}")}.mp4", TimeSpan.FromSeconds(10), Resolution.VGA.ToSize(), filterMethod);
    }
}