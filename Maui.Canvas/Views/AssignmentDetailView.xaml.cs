using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

// QueryProperty attributes wire up URL parameters to C# properties
// When someone navigates here with "?courseId=5&assignmentId=2",
// MAUI automatically sets CourseId=5 and AssignmentId=2 before NavigatedTo fires
// Recommended MAUI way to pass data between pages
[QueryProperty(nameof(CourseId), "courseId")]
[QueryProperty(nameof(AssignmentId), "assignmentId")]
public partial class AssignmentDetailView : ContentPage
{
	public int CourseId { get; set; }
	public int AssignmentId { get; set; }

	public AssignmentDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		// AssignmentId == 0 is the "new assignment" convention
		if (AssignmentId == 0)
		{
			BindingContext = new Assignment
			{
				DueDate = DateTime.Now.AddDays(7)   // sensible default
			};
		}
		else // Otherwise, look up the existing one and bind to it
		{
			BindingContext = CourseServiceProxy.Current
				.GetAssignmentById(CourseId, AssignmentId)
				?? new Assignment();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		// The BindingContext is the Assignment being edited
		// Entry bindings in XAML write directly to its properties
		var assignment = BindingContext as Assignment;
		CourseServiceProxy.Current.AddOrUpdateAssignment(CourseId, assignment);

		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}
}