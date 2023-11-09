using System.Drawing;
using VideoGenerator.Models;

namespace VideoGenerator.Tests;

[TestClass]
public class FileModelTests
{
    [DataTestMethod]
    [DataRow(@".\uv_test.png", 4096, 4096, DisplayName = "UV_Test")]
    public void ImageModelTest (string imagePath, int width, int height)
    {
        using (ImageData imageData = new(imagePath))
        {
            Image? image = imageData.GetData();
            FileInfo fileInfo = new(imagePath);

            Assert.IsNotNull(image);
            Assert.AreEqual(fileInfo.Name, imageData.Name);
            Assert.AreEqual(fileInfo.FullName, imageData.Info!.FullName);
            Assert.AreEqual(width, image.Width);
            Assert.AreEqual(height, image.Height);
        }
    }
}