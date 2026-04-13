using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class FileContentDetailView : ContentPage
{
	public static int CurrentCourseId { get; set; }
	public static int CurrentModuleId { get; set; }

	public int ContentId { get; set; }

	public FileContentDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (ContentId == 0)
		{
			BindingContext = new ModuleFile();
		}
		else
		{
			BindingContext = CourseServiceProxy.Current
				.GetModuleContentById(CurrentCourseId, CurrentModuleId, ContentId) as ModuleFile
				?? new ModuleFile();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var file = BindingContext as ModuleFile;
		CourseServiceProxy.Current.AddOrUpdateModuleContent(CurrentCourseId, CurrentModuleId, file);
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}
}