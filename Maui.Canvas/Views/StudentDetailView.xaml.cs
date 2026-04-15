using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(StudentId), "studentId")]
public partial class StudentDetailView : ContentPage
{
	public int StudentId { get; set; }

	public StudentDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (StudentId == 0)
		{
			BindingContext = new Student();
		}
		else
		{
			BindingContext = StudentServiceProxy.Current.GetById(StudentId) ?? new Student();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var student = BindingContext as Student;
		StudentServiceProxy.Current.AddOrUpdate(student);
		await Shell.Current.GoToAsync("//StudentManagement");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//StudentManagement");
	}
}