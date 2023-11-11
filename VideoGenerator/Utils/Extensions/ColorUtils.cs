using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OldColor = System.Drawing.Color;
using NewColor = System.Windows.Media.Color;

using OldPixelFormat = System.Drawing.Imaging.PixelFormat;
using NewPixelFormat = System.Windows.Media.PixelFormat;
using NewPixelFormats = System.Windows.Media.PixelFormats;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing.Imaging;

namespace VideoGenerator.Utils.Extensions;

public static class ColorUtils
{
    public static NewColor Convert(this OldColor color)
    {
        NewColor ret = new()
        {
            R = color.R,
            G = color.G,
            B = color.B,
            A = color.A,
        };
        return ret;
    }

    public static OldColor Convert (this NewColor color)
    {
        OldColor ret = OldColor.FromArgb(color.A, color.R, color.G, color.B);
        return ret;
    }

    public static NewPixelFormat Convert(this OldPixelFormat format)
    {
        switch(format)
        {
            case OldPixelFormat.Format8bppIndexed:
                return NewPixelFormats.Indexed8;
            case OldPixelFormat.Format24bppRgb:
                return NewPixelFormats.Rgb24;
            case OldPixelFormat.Format32bppArgb:
                return NewPixelFormats.Bgra32;
            default:
                return NewPixelFormats.Default;
        }
    }

    public static OldPixelFormat Convert (this NewPixelFormat format)
    {
        if(format == NewPixelFormats.Rgb24) return OldPixelFormat.Format24bppRgb;
        if(format == NewPixelFormats.Indexed8) return OldPixelFormat.Format8bppIndexed;
        return OldPixelFormat.DontCare;
    }

    public static BitmapPalette? Convert(this ColorPalette? palette)
    {
        var colors = palette?.Entries?.Select(c => c.Convert())?.ToList();
        if(colors is null || colors.Count <= 0 || colors.Count > 256) return null;

        return new BitmapPalette(colors);
    }

    public static WriteableBitmap? ToWriteableBitmap (this Image? image)
    {
        if (image is null || image.Width <= 0 || image.Height <= 0) return null;

        var palette = image.Palette.Convert();
        WriteableBitmap newBitmap = new(image.Width, image.Height, 96, 96, NewPixelFormats.Rgb24, palette);
        newBitmap.Lock();
        try
        {
            using (Bitmap temp = new Bitmap(image.Width, image.Height, newBitmap.BackBufferStride, OldPixelFormat.Format24bppRgb, newBitmap.BackBuffer))
            using (Graphics graphics = Graphics.FromImage(temp))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                newBitmap.AddDirtyRect(new Int32Rect(0, 0, image.Width, image.Height));
            }
        }
        finally
        {
            newBitmap.Unlock();
        }
        return newBitmap;
    }
}
