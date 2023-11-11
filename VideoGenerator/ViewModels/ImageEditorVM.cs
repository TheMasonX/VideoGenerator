using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public class ImageEditorVM : ObservableObject, IDisposable
{
    public ImageEditorVM ()
    {

    }


    public void Dispose ()
    {
        throw new NotImplementedException();
    }


    #region Properties

    private ImageData? _image;
    private ImageData? Image
    {
        get => _image;
        set
        {
            if (SetProperty(ref _image, value))
                CreateNewBitmap(value?.GetData());
        }
    }

    private WriteableBitmap? _bitmap;
    public WriteableBitmap? Bitmap
    {
        get => _bitmap;
        set => SetProperty(ref _bitmap, value);
    }

    private ObservableCollection<IImageTool>? _imageTools;
    public ObservableCollection<IImageTool> ImageTools
    {
        get => _imageTools ??= Application.Current.Dispatcher.Invoke(() => new ObservableCollection<IImageTool>());
        set => SetProperty(ref _imageTools, value);
    }

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    public bool OpenImage(ImageData? image)
    {
        if (image is null) return false;

        Image = image;
        return true;
    }

    #endregion Public Methods

    #region Private Methods

    private void CreateNewBitmap (Image? image)
    {
        if (image is null || image.Width <= 0 || image.Height <= 0) return;

        var format = image.PixelFormat.Convert();
        WriteableBitmap newBitmap = new(image.Width, image.Height, image.PhysicalDimension.Width, image.PhysicalDimension.Height, format, null);
        //WriteableBitmap newBitmap = new(image.Width, image.Height, image.PhysicalDimension.Width, image.PhysicalDimension.Height, PixelFormats.Default, null);
        newBitmap.Lock();
        try
        {
            using (Bitmap temp = new Bitmap(image.Width, image.Height, newBitmap.BackBufferStride, image.PixelFormat, newBitmap.BackBuffer))
            using (Graphics graphics = Graphics.FromImage(temp))
            {
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height));
                newBitmap.AddDirtyRect(new Int32Rect(0,0, image.Width,image.Height));
            }
        }
        finally
        {
            newBitmap.Unlock();
            Bitmap = newBitmap;
        }
    }

    #endregion Private Methods
}