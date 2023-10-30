﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
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
        Task.Run(() =>
        {
            List<string> files = new(100);
            for (int i = 0; i < 40; i++)
            {
                files.Add(@"C:\Users\TheMasonX\Pictures\uv_test.png");
            }

            //for(int i = 0; i < 5; i++)
            //{
            //Task.Delay(3000).Wait();
            OpenFiles(files);
            //}
        });
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

    private IAppStatus? _status;
    public IAppStatus? Status
    {
        get => _status;
        set => SetProperty(ref _status, value);
    }

    #endregion Properties

    #region Commands

    [RelayCommand]
    public Task OnOpenFiles ()
    {
        return Task.Run(() =>
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
    public Task OnSave ()
    {
        return Task.Run(() =>
        {
            Save();
        });
    }

    [RelayCommand]
    public Task OnSaveAs ()
    {
        return Task.Run(() =>
        {
            SaveAs();
        });
    }

    #endregion Commands

    #region Public Methods

    #region Open

    public void OpenFiles (IEnumerable<string>? files)
    {
        if (files is null || !files.Any()) return;

        int addCount = files.Count();
        List<Task> tasks = new(addCount);
        Status = new LoadingStatus(FileGrid.Count, FileGrid.Count + addCount, "Item", "Items");
        //foreach (string file in files)
        //{
        //    tasks.Add(Task.Run(() => OpenFile(file, false)));
        //}
        //Task.WhenAll(tasks).Wait();

        Task.Run(() =>
        {
            foreach (string file in files)
            {
                if (file.IsNullOrEmpty() || !Regex.IsMatch(file!, InputExtensionRegex)) continue;
                if (FileGrid.OpenFile(file, true))
                    Status.Update(1);
                else
                    Status = new TextStatus() { Status = $"Load Error for {file}", BGColor = Brushes.Red };
            }
            Status.Hide();
            FileGrid.Refresh();
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

    // Private Methods

    #endregion Private Methods
}