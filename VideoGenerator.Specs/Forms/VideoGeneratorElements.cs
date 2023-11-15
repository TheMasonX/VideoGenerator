using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using SpecFlow.Actions.WindowsAppDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGenerator.Specs.Drivers;

public class VideoGeneratorElements
{
    private readonly AppDriver _appDriver;

    public VideoGeneratorElements (AppDriver appDriver)
    {
        _appDriver = appDriver;
    }

    public WindowsElement FilesTab => _appDriver.Current.FindElementByAccessibilityId("filesTab");
    public WindowsElement ImageEditorTab => _appDriver.Current.FindElementByAccessibilityId("imageEditorTab");
    public WindowsElement FilesGrid => _appDriver.Current.FindElementByAccessibilityId("fileGrid");
    public WindowsElement FilesGrid_DataGrid => _appDriver.Current.FindElementByAccessibilityId("filesGrid_DataGrid");
    public AppiumWebElement FilesGrid_DataGrid2 => FilesGrid.FindElementByAccessibilityId("filesGrid_DataGrid");
}