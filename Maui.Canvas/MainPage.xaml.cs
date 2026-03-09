namespace Maui.Canvas
{
	public partial class MainPage : ContentPage
	{
		public MainPage()
		{
			InitializeComponent();
		}

		private void TeacherClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//TeacherMenu");
		}

		private void StudentClicked(object sender, EventArgs e)
		{
			Shell.Current.GoToAsync("//StudentMenu");
		}
	}
}