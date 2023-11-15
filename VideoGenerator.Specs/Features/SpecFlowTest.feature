Feature: SpecFlowTest
![SpecFlowTest](https://github.com/TheMasonX/VideoGenerator/)
Simple calculator for adding **two** numbers

Link to a feature: [SpecFlowTest](VideoGenerator.Specs/Features/SpecFlowTest.feature)
***Further read***: **[Learn more about how to generate Living Documentation](https://docs.specflow.org/projects/specflow-livingdoc/en/latest/LivingDocGenerator/Generating-Documentation.html)**


Scenario: Click On The File Grid
	Given App is loaded
	When Click on FilesTab
	When Wait 1 Seconds
	When Click on ImageEditorTab
	When Wait 5 Seconds
	When Click on FilesTab
	When Wait 5 Seconds
	When Click on FilesGrid
	When Wait 5 Seconds
	When Click on ImageEditorTab
	When Wait 10 Seconds
