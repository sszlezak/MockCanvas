using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentMainView : ContentPage
{
	private StudentMainViewViewModel? ViewModel => BindingContext as StudentMainViewViewModel;

	public StudentMainView()
	{
		InitializeComponent();
		BindingContext = new StudentMainViewViewModel();
	}

	private async void OpenCourseClicked(object sender, EventArgs e)
	{
		if (ViewModel?.SelectedCourse == null || ViewModel.SelectedStudent == null)
		{
			await DisplayAlert("No Course Selected", "Please select a course first.", "OK");
			return;
		}

		StudentCourseDetailViewModel.CurrentCourse = ViewModel.SelectedCourse;
		StudentCourseDetailViewModel.CurrentStudent = ViewModel.SelectedStudent;

		await Shell.Current.GoToAsync("//StudentCourseDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//MainPage");
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		ViewModel?.Refresh();
	}
}