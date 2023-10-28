using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoGenerator.Extensions;

namespace VideoGenerator.Models;

public interface IFileData<T>
{
    string Path { get; init; }
    string Name { get; init; }
    FileInfo? Info { get; init; }
    T? GetData ();
    bool SaveData (string outputPath);
}

public abstract class FileDataModel<T> : ObservableObject, IFileData<T>, IDisposable
{
    public FileDataModel (string filePath)
    {
        if (filePath.IsNullOrEmpty()) return;

        Path = filePath;
        Info = new(Path);
        Name = Info.Name;
        LoadData();
    }

    public abstract void Dispose ();


    #region Properties

    private readonly string? _filePath;
    public string Path
    {
        get => _filePath ?? "";
        init => SetProperty(ref _filePath, value);
    }

    private readonly string? _fileName;
    public string Name
    {
        get => _fileName ?? "";
        init => SetProperty(ref _fileName, value);
    }

    private readonly FileInfo? _fileInfo;
    public FileInfo? Info
    {
        get => _fileInfo;
        init => SetProperty(ref _fileInfo, value);
    }

    #endregion Properties

    #region Public Methods

    public abstract T? GetData ();
    public abstract bool SaveData (string outputPath);

    #endregion Public Methods

    #region Private Methods

    protected abstract void LoadData ();

    #endregion Private Methods
}

public class ImageDataModel : FileDataModel<Image>
{
    private Image? _data;

    public ImageDataModel (string filePath) : base(filePath) { }

    public override void Dispose ()
    {
        _data?.Dispose();
        _data = null;
        GC.SuppressFinalize(this);
    }

    #region Public Methods

    public override Image? GetData ()
    {
        return _data;
    }

    public override bool SaveData (string outputPath)
    {
        if (_data is null || outputPath.IsNullOrEmpty()) return false;

        try
        {
            _data.Save(outputPath);
            return true;
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Saving {Name} to {outputPath} threw error: {ex}");
            return false;
        }
    }

    #endregion Public Methods

    #region Private Methods

    protected override void LoadData ()
    {
        if (Info is null) return;

        try
        {
            _data = Image.FromFile(Path);
        }
        catch (Exception ex)
        {
            Trace.WriteLine($"Loading {Name} from {Path} threw error: {ex}");
        }
    }

    #endregion Private Methods
}