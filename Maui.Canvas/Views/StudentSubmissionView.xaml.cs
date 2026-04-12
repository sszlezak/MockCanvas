using Library.Canvas.Model;

namespace Maui.Canvas.Views;

public partial class StudentSubmissionView : ContentPage
{
	// Static state set by StudentCourseDetailView before navigating here
	public static Assignment? CurrentAssignment { get; set; }
	public static Student? CurrentStudent { get; set; }

	public StudentSubmissionView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (CurrentAssignment == null || CurrentStudent == null) return;

		AssignmentInfoLabel.Text = $"{CurrentAssignment.Name} ({CurrentAssignment.AvailablePoints} pts) - Due {CurrentAssignment.DueDate:MM/dd/yyyy}";
		AssignmentDescLabel.Text = CurrentAssignment.Description;

		var existing = CurrentAssignment.Submissions
			.FirstOrDefault(s => s.StudentId == CurrentStudent.Id);

		if (existing != null)
		{
			ResponseEntry.Text = existing.Content;
			ResponseEntry.IsEnabled = false;
			SubmitButton.IsEnabled = false;
			StatusLabel.Text = $"Submitted on {existing.SubmissionDate:MM/dd/yyyy}";
		}
		else
		{
			ResponseEntry.Text = "";
			ResponseEntry.IsEnabled = true;
			SubmitButton.IsEnabled = true;
			StatusLabel.Text = "Not yet submitted";
		}
	}

	private async void SubmitClicked(object sender, EventArgs e)
	{
		if (CurrentAssignment == null || CurrentStudent == null) return;

		if (string.IsNullOrWhiteSpace(ResponseEntry.Text))
		{
			await DisplayAlert("Empty Response", "Please enter a response before submitting.", "OK");
			return;
		}

		var submission = new Submission
		{
			Id = CurrentAssignment.Submissions.Count + 1,
			StudentId = CurrentStudent.Id,
			AssignmentId = CurrentAssignment.Id,
			Content = ResponseEntry.Text,
			SubmissionDate = DateTime.Now
		};

		CurrentAssignment.Submissions.Add(submission);

		await DisplayAlert("Submitted", "Your response has been submitted.", "OK");
		await Shell.Current.GoToAsync("//StudentCourseDetail");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//StudentCourseDetail");
	}
}