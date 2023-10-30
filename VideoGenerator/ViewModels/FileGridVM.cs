using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using VideoGenerator.Models;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public partial class FileGridVM : ObservableObject, IDisposable
{
    private object _lock = new();

    public FileGridVM ()
    {

    }

    public void Dispose ()
    {
        if(_imageFiles is not null)
        {
            foreach (var file in _imageFiles)
                file.Dispose();
        }
        if(_imageFilesView is not null)
        {
        }
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
        get => _imageFilesView ??= CreateFileListView(ImageFiles); 
        //get => _imageFilesView ??= new(ImageFiles);
        set => SetProperty(ref _imageFilesView, value);
    }

    private string? _fileNameFilter;
    public string FileNameFilter
    {
        get => _fileNameFilter ??= "";
        set
        {
            if (SetProperty(ref _fileNameFilter, value) && EnableFileNameFilter && ImageFilesView.CanFilter)
                Refresh();
        }
    }

    private bool _enableFileNameFilter;
    public bool EnableFileNameFilter
    {
        get => _enableFileNameFilter;
        set
        {
            if (SetProperty(ref _enableFileNameFilter, value))
                Refresh();
        }
    }

    public int Count => ImageFilesView?.Count ?? 0;

    private bool _isFilterOpen;
    public bool IsFilterOpen
    {
        get => _isFilterOpen;
        set => SetProperty(ref _isFilterOpen, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public void ToggleFilenameFilter ()
    {
        EnableFileNameFilter = !EnableFileNameFilter;
    }

    #endregion Commands

    #region Public Methods

    public bool OpenFile (string? file, bool update = true)
    {
        if (file.IsNullOrEmpty()) return false;

        ImageData data = new(file!);
        lock (_lock)
        {
            ImageFiles.Add(data);
        }
        //    ImageFilesView.Dispatcher.BeginInvoke(() =>
        //{
            
        //        ImageFiles.Add(data);
        //        //ImageFilesView.AddNewItem(data);
        //    }
        //});

        return true;
    }

    public void Refresh ()
    {
        ImageFilesView.Dispatcher.BeginInvoke(() =>
        {
            lock (_lock)
            {
                ImageFilesView.Refresh();
            }
        });
    }


    #endregion Public Methods

    #region Private Methods

    private ListCollectionView CreateFileListView<T>(List<T> files)
    {
        return Application.Current.Dispatcher.Invoke(() =>
        {
            var view = new ListCollectionView(files) { Filter = FilterImageFileNames };
            BindingOperations.EnableCollectionSynchronization(view, _lock);
            return view;
        });
    }

    private bool FilterImageFileNames (object file)
    {
        if (!EnableFileNameFilter || FileNameFilter.IsNullOrEmpty()) return true;
        if (file is not ImageData imageData) return false;

        return imageData.Name.Contains(FileNameFilter, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion Private Methods
}