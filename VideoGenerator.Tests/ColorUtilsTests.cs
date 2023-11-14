using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OldColor = System.Drawing.Color;
using NewColor = System.Windows.Media.Color;
using NewColors = System.Windows.Media.Colors;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.Tests;

public static class AssertEx
{
    public static void AreEqual(OldColor expected, NewColor actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }

    public static void AreEqual (NewColor expected, OldColor actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }

    public static void AreEqual (OldColor expected, OldColor actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }

    public static void AreEqual (NewColor expected, NewColor actual)
    {
        Assert.AreEqual(expected.R, actual.R);
        Assert.AreEqual(expected.G, actual.G);
        Assert.AreEqual(expected.B, actual.B);
        Assert.AreEqual(expected.A, actual.A);
    }
}

[TestClass]
public class ColorUtilsTests
{
    public static (OldColor, NewColor)[] pairs = new (OldColor, NewColor)[]
    {
        (OldColor.Black, NewColors.Black),
        (OldColor.Red, NewColors.Red),
        (OldColor.Blue, NewColors.Blue),
        (OldColor.Green, NewColors.Green),
        (OldColor.Yellow, NewColors.Yellow),
        (OldColor.White, NewColors.White),
        (OldColor.Gray, NewColors.Gray),
        (OldColor.Orange, NewColors.Orange),
        (OldColor.Brown, NewColors.Brown),
        (OldColor.Pink, NewColors.Pink),
        (OldColor.Purple, NewColors.Purple),
    };

    [TestMethod]
    public void ConvertForward ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item1.Convert();
            AssertEx.AreEqual(colorPair.Item2, converted);
        }
    }

    [TestMethod]
    public void ConvertBack ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item2.Convert();
            AssertEx.AreEqual(colorPair.Item1, converted);
        }
    }

    [TestMethod]
    public void DoubleConvertBidirectional ()
    {
        foreach (var colorPair in pairs)
        {
            var converted = colorPair.Item1.Convert().Convert();
            AssertEx.AreEqual(colorPair.Item1, converted);

            var converted2 = colorPair.Item2.Convert().Convert();
            AssertEx.AreEqual(colorPair.Item2, converted2);
        }
    }
}