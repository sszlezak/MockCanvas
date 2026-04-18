using Library.Canvas.Model;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class AssignmentRefContentDetailView : ContentPage
{
	public int ContentId { get; set; }

	public AssignmentRefContentDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		ModuleAssignment moduleAssignment;
		if (ContentId == 0)
		{
			moduleAssignment = new ModuleAssignment();
		}
		else
		{
			moduleAssignment = CourseServiceProxy.Current
				.GetModuleContentById(ModuleDetailViewModel.CurrentCourseId, ModuleDetailViewModel.CurrentModuleId, ContentId) as ModuleAssignment
				?? new ModuleAssignment();
		}

		BindingContext = moduleAssignment;

		var course = CourseServiceProxy.Current.GetById(ModuleDetailViewModel.CurrentCourseId);
		AssignmentPicker.ItemsSource = course?.Assignments;
		AssignmentPicker.SelectedItem = moduleAssignment.Assignment;
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var moduleAssignment = BindingContext as ModuleAssignment;
		if (moduleAssignment == null) return;

		moduleAssignment.Assignment = AssignmentPicker.SelectedItem as Assignment;

		CourseServiceProxy.Current.AddOrUpdateModuleContent(
			ModuleDetailViewModel.CurrentCourseId,
			ModuleDetailViewModel.CurrentModuleId, moduleAssignment);
		await Shell.Current.GoToAsync("//ModuleDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ModuleDetail");
	}
}