using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public class FileGridVM : ObservableObject, IDisposable
{
    public FileGridVM ()
    {

    }

    public void Dispose ()
    {
        throw new NotImplementedException();
    }


    #region Properties

    private List<ImageData>? _imageFiles;
    public List<ImageData> ImageFiles
    {
        get => _imageFiles ??= new();
        set => SetProperty(ref _imageFiles, value);
    }

    private ListCollectionView? _imageFilesView;
    public ListCollectionView ImageFilesView
    {
        get => _imageFilesView ??= new(ImageFiles);
        //get => _imageFilesView ??= new(ImageFiles);
        set => SetProperty(ref _imageFilesView, value);
    }

    private string? _fileNameFilter;
    public string FileNameFilter
    {
        get => _fileNameFilter ??= "";
        set
        {
            if (SetProperty(ref _fileNameFilter, value) && EnableFileNameFilter)
                ImageFilesView.Refresh();
        }
    }

    private bool _enableFileNameFilter;
    public bool EnableFileNameFilter
    {
        get => _enableFileNameFilter;
        set
        {
            if (SetProperty(ref _enableFileNameFilter, value))
            {
                ImageFilesView.Filter = value ? FilterImageFileNames : null;
                ImageFilesView.Refresh();
            }
        }
    }

    private bool _isFilterOpen;
    public bool IsFilterOpen
    {
        get => _isFilterOpen;
        set => SetProperty(ref _isFilterOpen, value);
    }

    #endregion Properties

    #region Commands

    //Commands

    #endregion Commands

    #region Public Methods

    //Public Methods

    #endregion Public Methods

    #region Private Methods

    private bool FilterImageFileNames (object file)
    {
        if (!EnableFileNameFilter || FileNameFilter.IsNullOrEmpty()) return true;
        if (file is not ImageData imageData) return false;

        return imageData.Name.Contains(FileNameFilter, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion Private Methods
}