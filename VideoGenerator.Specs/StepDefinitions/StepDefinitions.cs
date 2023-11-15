using TechTalk.SpecFlow;
using VideoGenerator.Specs.Drivers;

namespace VideoGenerator.Specs.StepDefinitions;

[Binding]
public sealed class StepDefinitions
{
    private readonly VideoGeneratorForm _form;

    public StepDefinitions (VideoGeneratorForm form)
    {
        _form = form;
    }

    [Given(@"App is loaded")]
    public void GivenAppIsLoadedAsync ()
    {
        Task.Delay(5000).Wait();
    }

    [When(@"Click on FilesTab")]
    public void WhenClickOnFilesTab ()
    {
        _form.ClickFilesTab();
    }

    [When(@"Click on ImageEditorTab")]
    public void WhenClickOnImageEditorTab ()
    {
        _form.ClickImageEditorTab();
    }

    [When(@"Click on FilesGrid")]
    public void WhenClickOnFilesGrid ()
    {
        _form.ClickFilesGrid();
    }


    [When(@"Wait (.*) Seconds")]
    public void WhenWaitSeconds (int seconds)
    {
        Task.Delay(seconds * 1000).Wait();
    }
}
