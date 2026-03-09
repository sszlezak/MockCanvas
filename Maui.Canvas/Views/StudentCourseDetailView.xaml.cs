using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentCourseDetailView : ContentPage
{
	private StudentCourseDetailViewModel ViewModel => BindingContext as StudentCourseDetailViewModel;

	public StudentCourseDetailView()
	{
		InitializeComponent();
		BindingContext = new StudentCourseDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		ViewModel?.Refresh();
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//StudentMenu");
	}
}