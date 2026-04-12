using Maui.Canvas.ViewModels;
using Library.Canvas.Model;

namespace Maui.Canvas.Views;

public partial class SubmissionDetailView : ContentPage
{
	private Submission? Sub => SubmissionDetailViewModel.CurrentSubmission;
	private Assignment? Assignment => SubmissionDetailViewModel.CurrentAssignment;

	public SubmissionDetailView()
	{
		InitializeComponent();
		BindingContext = new SubmissionDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (Sub == null) return;

		PointsEntry.Text = Sub.PointsEarned.HasValue ? Sub.PointsEarned.Value.ToString("F1") : "";
		PercentEntry.Text = Sub.Grade.HasValue ? Sub.Grade.Value.ToString("F1") : "";
		CommentEntry.Text = Sub.Comment ?? "";
	}

	private async void SaveByPointsClicked(object sender, EventArgs e)
	{
		if (Sub == null || Assignment == null) return;

		if (!double.TryParse(PointsEntry.Text, out double points) || points < 0)
		{
			await DisplayAlert("Invalid Input", "Please enter a valid points value.", "OK");
			return;
		}

		if (points > Assignment.AvailablePoints)
		{
			await DisplayAlert("Invalid Input", $"Points cannot exceed {Assignment.AvailablePoints}.", "OK");
			return;
		}

		Sub.PointsEarned = points;
		Sub.Grade = points / Assignment.AvailablePoints * 100;
		Sub.Comment = CommentEntry.Text;
		Sub.Feedback = CommentEntry.Text;
		Sub.SubmissionDate = DateTime.Now;

		PercentEntry.Text = Sub.Grade.Value.ToString("F1");

		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private async void SaveByPercentClicked(object sender, EventArgs e)
	{
		if (Sub == null || Assignment == null) return;

		if (!double.TryParse(PercentEntry.Text, out double percent) || percent < 0 || percent > 100)
		{
			await DisplayAlert("Invalid Input", "Please enter a percentage between 0 and 100.", "OK");
			return;
		}

		Sub.PointsEarned = percent / 100.0 * Assignment.AvailablePoints;
		Sub.Grade = percent;
		Sub.Comment = CommentEntry.Text;
		Sub.Feedback = CommentEntry.Text;
		Sub.SubmissionDate = DateTime.Now;

		PointsEntry.Text = Sub.PointsEarned.Value.ToString("F1");

		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherCourseDetail");
	}
}