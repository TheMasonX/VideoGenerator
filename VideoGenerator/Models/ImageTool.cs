using System;

using CommunityToolkit.Mvvm.ComponentModel;

namespace VideoGenerator.Models;

public interface IImageTool
{
    string Name { get; }
    string? Description { get; }
    string IconPath { get; }
}

public class ImageTool : ObservableObject, IImageTool, IDisposable
{
    public ImageTool ()
    {

    }

    public void Dispose ()
    {
        throw new NotImplementedException();
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

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    //Public Methods

    #endregion Public Methods

    #region Private Methods

    //Private Methods

    #endregion Private Methods
}