using System.Drawing;

using BenchmarkDotNet.Attributes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.Benchmarks;

//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net60)]
//[SimpleJob(runtimeMoniker: RuntimeMoniker.Net80)]
[MemoryDiagnoser]
[TestClass]
public class Benchmarks
{
    Image? bitmapImage;

    [GlobalSetup]
    [TestInitialize]
    public void Setup ()
    {
        bitmapImage = Image.FromFile(".\\uv_test.png");
    }

    [GlobalCleanup]
    [TestCleanup]
    public void Cleanup ()
    {

    }

    [Benchmark]
    [TestMethod]
    public void GetWriteableBitmap ()
    {
        var bitmap = bitmapImage!.ToWriteableBitmap();
        Assert.IsNotNull(bitmap);
        Assert.AreEqual(bitmapImage!.Width, bitmap.PixelWidth);
        Assert.AreEqual(bitmapImage!.Height, bitmap.PixelHeight);
    }
}