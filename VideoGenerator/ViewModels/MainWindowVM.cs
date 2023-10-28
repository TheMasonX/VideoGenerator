using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using VideoGenerator.Extensions;
using VideoGenerator.Models;
using VideoGenerator.Properties;

namespace VideoGenerator.ViewModels;

public partial class MainWindowVM : ObservableObject, IDisposable
{
    private const string _openFileDialogTitle = "Open Image Files";
    private const string _inputExtensionFilter = "Image files (*.bmp, *.jpg, *.png)|*.bmp;*.jpg;*.png|All files (*.*)|*.*";
    public string InputExtensionRegex => @"(\.bmp)|(\.jpg)|(\.png)";

    private const string _saveFileDialogTitle = "Save Video File";
    private const string _outputExtensionFilter = "Video File (*.mp4)|*.mp4|All files (*.*)|*.*";

    public MainWindowVM ()
    {
        OpenFile(@".\uv_test.png");
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

    private ObservableCollection<ImageData> _imageFiles;
    public ObservableCollection<ImageData> ImageFiles
    {
        get => _imageFiles ??= new();
        set => SetProperty(ref _imageFiles, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public Task OnOpenFiles()
    {
        return Task.Run(() =>
        {
            OpenFileDialog openFileDialog = new()
            {
                Title = "Select Images To Convert",
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
    public Task OnSave ()
    {
        return Save();
    }

    [RelayCommand]
    public Task OnSaveAs ()
    {
        return Save();
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public Task OpenFiles(IEnumerable<string>? files)
    {
        if (files is null || !files.Any()) return Task.CompletedTask;

        List<Task> tasks = new(files.Count());
        foreach(string file in files)
        {
            tasks.Add(OpenFile(file));
        }
        return Task.WhenAll(tasks);
    }

    public Task OpenFile (string? file)
    {
        if (file.IsNullOrEmpty() || !Regex.IsMatch(file, InputExtensionRegex)) return Task.CompletedTask;

        var image = new ImageData(file!);
        Application.Current.Dispatcher.Invoke(() => ImageFiles.Add(image));

        return Task.CompletedTask;
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

    //Private Methods

    #endregion Private Methods
}