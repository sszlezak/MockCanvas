using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class TeacherMainView : ContentPage
{
	public TeacherMainView()
	{
		InitializeComponent();
		BindingContext = new TeacherMainViewViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		(BindingContext as TeacherMainViewViewModel)?.Refresh();
	}

	private async void OpenCourseClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherMainViewViewModel;
		if (vm?.SelectedCourse == null)
		{
			await DisplayAlert("No Course Selected", "Please select a course first.", "OK");
			return;
		}

		TeacherCourseDetailViewModel.CurrentCourse = vm.SelectedCourse;
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private async void ManageStudentsClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//StudentManagement");
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//MainPage");
	}
}