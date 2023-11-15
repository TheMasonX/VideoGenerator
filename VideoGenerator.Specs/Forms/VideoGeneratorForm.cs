using SpecFlow.Actions.WindowsAppDriver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoGenerator.Specs.Drivers;

public class VideoGeneratorForm : VideoGeneratorElements
{
    public VideoGeneratorForm (AppDriver appDriver) : base(appDriver)
    {
    }

    public void ClickFilesTab () => FilesTab.Click();
    public void ClickImageEditorTab () => ImageEditorTab.Click();
    public void ClickFilesGrid () => FilesGrid.Click();
}
