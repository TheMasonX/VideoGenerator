//#define USE_LOCAL_FILES

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
        string[] files = Enumerable.Repeat(@".\uv_test.png", 50).ToArray();
        return (folder, files);
#endif
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow(VideoFilter.Canny)]
    [DataRow(VideoFilter.Contours)]
    public void CreateVideo (VideoFilter filter)
    {
        var folderAndFiles = GetFolderAndFiles();
        string folder = folderAndFiles.folder;
        string[] files = folderAndFiles.files;
        
        var images = files.Select(f => Image.FromFile(f)).AsQueryable();
        var filterMethod = filter.GetFilterMethod();
        VideoUtils.CreateVideo(images, $@"{Environment.CurrentDirectory}\Video{(filterMethod is null ? "" : $"_{filter}")}.mp4", TimeSpan.FromSeconds(40), Resolution.VGA.ToSize(), filterMethod);
    }
}