using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class AssignmentRefContentDetailView : ContentPage
{
	public static int CurrentCourseId { get; set; }
	public static int CurrentModuleId { get; set; }

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
				.GetModuleContentById(CurrentCourseId, CurrentModuleId, ContentId) as ModuleAssignment
				?? new ModuleAssignment();
		}

		BindingContext = moduleAssignment;

		var course = CourseServiceProxy.Current.GetById(CurrentCourseId);
		AssignmentPicker.ItemsSource = course?.Assignments;
		AssignmentPicker.SelectedItem = moduleAssignment.Assignment;
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var moduleAssignment = BindingContext as ModuleAssignment;
		if (moduleAssignment == null) return;

		moduleAssignment.Assignment = AssignmentPicker.SelectedItem as Assignment;

		CourseServiceProxy.Current.AddOrUpdateModuleContent(CurrentCourseId, CurrentModuleId, moduleAssignment);
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}
}