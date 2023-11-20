﻿using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Serilog;

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
        GC.SuppressFinalize(this);
    }


    #region Properties

    private ImageData? _image;
    public ImageData? Image
    {
        get => _image;
        private set
        {
            if (SetProperty(ref _image, value))
            {
                CreateNewBitmap(value?.GetData());
            }
        }
    }

    private WriteableBitmap? _bitmap;
    public WriteableBitmap? Bitmap
    {
        get => _bitmap;
        set
        {
            if (SetProperty(ref _bitmap, value) && value is not null)
            {
                ResetControls();
            }
        }
    }

    private ObservableCollection<IImageTool>? _imageTools;
    public ObservableCollection<IImageTool> ImageTools
    {
        get => _imageTools ??= Application.Current.Dispatcher.Invoke(() => new ObservableCollection<IImageTool>());
        set => SetProperty(ref _imageTools, value);
    }

    private readonly double _minZoom = 0.01;
    public double MinZoom => _minZoom;
    private readonly double _maxZoom = 5;
    public double MaxZoom => _maxZoom;

    private double _zoom = 1;
    public double Zoom
    {
        get => _zoom;
        set => SetProperty(ref _zoom, value);
    }

    private double _centerX = .5;
    public double CenterX
    {
        get => _centerX;
        set => SetProperty(ref _centerX, value);
    }

    private double _centerY = .5;
    public double CenterY
    {
        get => _centerY;
        set => SetProperty(ref _centerY, value);
    }

    private double _actualWidth = 0;
    public double ActualWidth
    {
        get => _actualWidth;
        set => SetProperty(ref _actualWidth, value);
    }

    private double _actualHeight = 0;
    public double ActualHeight
    {
        get => _actualHeight;
        set => SetProperty(ref _actualHeight, value);
    }

    #endregion Properties

    #region Commands

    private RelayCommand<RoutedEventArgs>? _mouseWheelCommand;
    public ICommand MouseWheelCommand => _mouseWheelCommand ??= new RelayCommand<RoutedEventArgs>(OnMouseWheel);

    public void OnMouseWheel (RoutedEventArgs? e)
    {
        if (e is not MouseWheelEventArgs args)
        {
            return;
        }

        var delta = args.Delta;
        var oldZoom = Zoom;
        Zoom = Math.Clamp(Zoom + delta * .001, MinZoom, MaxZoom);

        Log.Debug("Delta {Delta}, Zoom went from {OldZoom} to {NewZoom}", delta, oldZoom, Zoom);
    }

    #endregion Commands

    #region Public Methods

    public bool OpenImage (ImageData? image)
    {
        if (image is null)
        {
            return false;
        }

        Image = image;
        return true;
    }

    public void ResetControls ()
    {
        if (Bitmap is null)
        {
            return;
        }

        CenterX = CenterY = .5;
        Zoom = 1;
    }

    #endregion Public Methods

    #region Private Methods

    private void CreateNewBitmap (Image? image)
    {
        if (image is null || image.Width <= 0 || image.Height <= 0)
        {
            return;
        }

        Bitmap = image.ToWriteableBitmap();
    }

    #endregion Private Methods
}