namespace Maui.Canvas.Views;

public partial class TeacherMainView : ContentPage
{
	public TeacherMainView()
	{
		InitializeComponent();
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//MainPage");
	}
}