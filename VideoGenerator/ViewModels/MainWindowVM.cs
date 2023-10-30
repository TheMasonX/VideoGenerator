using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using VideoGenerator.Models;
using VideoGenerator.Properties;
using VideoGenerator.Utils.Extensions;

namespace VideoGenerator.ViewModels;

public partial class MainWindowVM : ObservableObject, IDisposable
{
    private const string _openFileDialogTitle = "Select Images To Convert";
    private const string _inputExtensionFilter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
    public string InputExtensionRegex => @"(\.bmp)|(\.jpg)|(\.png)";

    private const string _saveFileDialogTitle = "Save Video File";
    private const string _outputExtensionFilter = "Video File (*.mp4)|*.mp4|All files (*.*)|*.*";

    public MainWindowVM ()
    {
        Test();
    }

    private void Test ()
    {
        //Task.Run(() =>
        //{
            List<string> files = new(100);
            for (int i = 0; i < 50; i++)
            {
                files.Add(@".\uv_test.png");
            }

            //for(int i = 0; i < 5; i++)
            //{
            //Task.Delay(3000).Wait();
            OpenFiles(files);
            //}
        //});
    }

    public void Dispose ()
    {
        throw new NotImplementedException();
    }


    #region Properties

    private string? _outputFileName;
    public string OutputFileName
    {
        get => _outputFileName ??= "";
        set
        {
            if (SetProperty(ref _outputFileName, value))
            {
                Settings.Default.OutputFileName = value;
                Settings.Default.Save();
            }
        }
    }

    private string? _outputFilePath;
    public string OutputFilePath
    {
        get => _outputFilePath ??= "";
        set
        {
            if (SetProperty(ref _outputFilePath, value))
            {
                Settings.Default.OutputFilePath = value;
                Settings.Default.Save();
            }
        }
    }

    private FileGridVM _fileGrid;
    public FileGridVM FileGrid
    {
        get => _fileGrid ??= new FileGridVM();
        set => SetProperty(ref _fileGrid, value);
    }

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

    private IAppStatus? _status;
    public IAppStatus? Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public void OnOpenFiles ()
    {
        Task.Run(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = _openFileDialogTitle,
                RestoreDirectory = true,
                CheckFileExists = true,
                Multiselect = true,
                Filter = _inputExtensionFilter,
            };
            openFileDialog.ShowDialog();
            OpenFiles(openFileDialog.FileNames);
        });
    }

    [RelayCommand]
    public void OnSave ()
    {
        Task.Run(() =>
        {
            Save();
        });
    }

    [RelayCommand]
    public void OnSaveAs ()
    {
        Task.Run(() =>
        {
            SaveAs();
        });
    }

    [RelayCommand]
    public void ToggleFilenameFilter ()
    {
        EnableFileNameFilter = !EnableFileNameFilter;
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public void OpenFiles (IEnumerable<string>? files)
    {
        if (files is null || !files.Any()) return;

        List<Task> tasks = new(files.Count());
        Status = new LoadingAppStatus(ImageFiles.Count, ImageFiles.Count + files.Count(), "Item", "Items");
        //foreach (string file in files)
        //{
        //    tasks.Add(Task.Run(() => OpenFile(file, false)));
        //}
        //Task.WhenAll(tasks).Wait();

        Task.Run(() =>
        {
            foreach (string file in files)
            {
                OpenFile(file, false);
            }
            Status.Hide();
        });

        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            ImageFilesView.CommitNew();
            //ImageFilesView.Refresh();
        });

    }

    public void OpenFile (string? file, bool update = true)
    {
        if (file.IsNullOrEmpty() || !Regex.IsMatch(file!, InputExtensionRegex)) return;

        ImageData data = new(file!);
        //ImageFiles.Add(data);
        Status?.Update(1);
        Application.Current.Dispatcher.BeginInvoke(() =>
        {
            ImageFilesView.AddNewItem(data);
            if (update) ImageFilesView.CommitNew();
        });
    }

    #endregion Open

    public Task Save ()
    {
        if (OutputFilePath.IsNullOrEmpty()) //No path, try save as instead
        {
            return SaveAs();
        }

        return Task.Run(() =>
        {
            if (File.Exists(OutputFilePath))
                File.Delete(OutputFilePath);

            //using (StreamWriter sw = File.CreateText(OutputFilePath))
            //{
            //    sw.Write(OutputText);
            //    sw.Flush();
            //    sw.Close();
            //}
        });
    }

    public Task SaveAs ()
    {
        return Task.Run(() =>
        {
            SaveFileDialog fileDialog = new()
            {
                Title = _saveFileDialogTitle,
                FileName = OutputFileName,
                OverwritePrompt = true,
                AddExtension = true,
                DefaultExt = ".h",
                RestoreDirectory = true,
                Filter = _outputExtensionFilter,
            };
            if (!Settings.Default.OutputDirectory.IsNullOrEmpty())
                fileDialog.InitialDirectory = Settings.Default.OutputDirectory;

            bool? result = fileDialog.ShowDialog();
            if (!result.HasValue || !result.Value) return; //Invalid selection

            OutputFilePath = fileDialog.FileName;

            if (!OutputFilePath.IsNullOrEmpty()) Save(); //Valid FilePath
        });
    }

    #endregion Public Methods

    #region Private Methods

    private bool FilterImageFileNames(object file)
    {
        if (!EnableFileNameFilter || FileNameFilter.IsNullOrEmpty()) return true;
        if (file is not ImageData imageData) return false;

        return imageData.Name.Contains(FileNameFilter, StringComparison.CurrentCultureIgnoreCase);
    }

    #endregion Private Methods
}