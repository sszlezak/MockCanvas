using Library.Canvas.Model;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentCourseDetailView : ContentPage
{
	private StudentCourseDetailViewModel? ViewModel => BindingContext as StudentCourseDetailViewModel;

	public StudentCourseDetailView()
	{
		InitializeComponent();
		BindingContext = new StudentCourseDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		ViewModel?.Refresh();
	}

	private async void SubmitAssignmentClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Assignment assignment)
		{
			if (StudentCourseDetailViewModel.CurrentStudent == null) return;

			StudentSubmissionView.CurrentAssignment = assignment;
			StudentSubmissionView.CurrentStudent = StudentCourseDetailViewModel.CurrentStudent;

			await Shell.Current.GoToAsync("//StudentSubmission");
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//StudentMenu");
	}
}