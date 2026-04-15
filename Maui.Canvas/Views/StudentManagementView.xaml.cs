using Library.Canvas.Model;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class StudentManagementView : ContentPage
{
	public StudentManagementView()
	{
		InitializeComponent();
		BindingContext = new StudentManagementViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		(BindingContext as StudentManagementViewModel)?.Refresh();
	}

	private async void AddStudentClicked(object sender, EventArgs e)
	{
		// studentId=0 = creating new
		await Shell.Current.GoToAsync("//StudentDetail?studentId=0");
	}

	private async void EditStudentClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Student student)
		{
			await Shell.Current.GoToAsync($"//StudentDetail?studentId={student.Id}");
		}
	}

	private void RemoveStudentClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Student student)
		{
			StudentServiceProxy.Current.Delete(student);
			(BindingContext as StudentManagementViewModel)?.Refresh();
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}