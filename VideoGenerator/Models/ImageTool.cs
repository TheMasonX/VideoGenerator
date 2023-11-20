using System;

using CommunityToolkit.Mvvm.ComponentModel;

using VideoGenerator.Utils.Video;

using OldColor = System.Drawing.Color;
using NewColor = System.Windows.Media.Color;
using NewColors = System.Windows.Media.Colors;
using ImageFilter = System.Action<OpenCvSharp.Mat, OpenCvSharp.Mat>;

namespace VideoGenerator.Models;

public interface IImageTool
{
    string Name { get; }
    string? Description { get; }
    string IconPath { get; }
    ImageFilter? Filter { get; }
}

public class ImageTool : ObservableObject, IImageTool, IDisposable
{
    public ImageTool ()
    {

    }

    public void Dispose ()
    {
        GC.SuppressFinalize(this);
    }


    #region Properties

    private string? _name;
    public string Name
    {
        get => _name = "";
        private set => SetProperty(ref _name, value);
    }

    private string? _description;
    public string? Description
    {
        get => _description;
        private set => SetProperty(ref _description, value);
    }

    private string? _iconPath;
    public string IconPath
    {
        get => _iconPath ??= "";
        private set => SetProperty(ref _iconPath, value);
    }

    private NewColor? _minColor;
    public NewColor MinColor
    {
        get => _minColor ??= NewColors.Black;
        private set => SetProperty(ref _minColor, value);
    }

    private NewColor? _maxColor;
    public NewColor MaxColor
    {
        get => _maxColor ??= NewColors.Black;
        private set => SetProperty(ref _maxColor, value);
    }

    private ImageFilter? _filter;
    public ImageFilter? Filter
    {
        get => _filter ??= GetFilter();
        private set => SetProperty(ref _filter, value);
    }

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    //Public Methods

    #endregion Public Methods

    #region Private Methods

    private ImageFilter? GetFilter()
    {
        return VideoUtils.GetFilterMethod(VideoFilter.Other);
    }

    #endregion Private Methods
}