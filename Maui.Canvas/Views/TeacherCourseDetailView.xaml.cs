using Maui.Canvas.ViewModels;
using Library.Canvas.Model;

namespace Maui.Canvas.Views;

public partial class TeacherCourseDetailView : ContentPage
{
	public TeacherCourseDetailView()
	{
		InitializeComponent();
		BindingContext = new TeacherCourseDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		(BindingContext as TeacherCourseDetailViewModel)?.Refresh();
	}

	private void SaveWeightClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (sender is Button btn && btn.CommandParameter is AssignmentGroup group)
		{
			if (btn.Parent is Grid grid)
			{
				var entry = grid.Children.OfType<Entry>().FirstOrDefault();
				if (entry != null)
					vm?.SaveGroupWeight(group, entry.Text);
			}
		}
	}

	private async void GradeClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is SubmissionEntry entry)
		{
			if (entry.Submission == null)
			{
				await DisplayAlert("No Submission", "This student has not submitted yet.", "OK");
				return;
			}

			SubmissionDetailViewModel.CurrentSubmission = entry.Submission;
			SubmissionDetailViewModel.CurrentAssignment =
				(BindingContext as TeacherCourseDetailViewModel)?.SelectedAssignment;

			await Shell.Current.GoToAsync("//SubmissionDetail");
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}