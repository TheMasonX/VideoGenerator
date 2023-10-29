using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VideoGenerator.ViewModels;

namespace VideoGenerator;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow ()
    {
        InitializeComponent();
    }

    private void Window_Drop (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm) return;
        var files = GetFiles(e, vm.InputExtensionRegex);

        vm.OpenFiles(files);
    }

    private void Window_DragEnter (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm) return;

        GetFiles(e, vm.InputExtensionRegex);
    }

    private void Window_DragOver (object sender, DragEventArgs e)
    {
        if (DataContext is not MainWindowVM vm) return;

        GetFiles(e, vm.InputExtensionRegex);
    }

    private IEnumerable<string>? GetFiles(DragEventArgs e, string regex)
    {
        if (!e.Data.GetDataPresent(DataFormats.FileDrop))
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return null;
        }
        var files = ((string[])e.Data.GetData(DataFormats.FileDrop));
        var filteredFiles = files.Where(f => Regex.Match(f, regex).Success);
        if (!filteredFiles.Any())
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;
            return null;
        }

        e.Effects = DragDropEffects.Copy;
        e.Handled = true;
        return files;
    }
}