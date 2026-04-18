using Library.Canvas.Model;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class FileContentDetailView : ContentPage
{

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
				.GetModuleContentById(ModuleDetailViewModel.CurrentCourseId, ModuleDetailViewModel.CurrentModuleId, ContentId) as ModuleFile
				?? new ModuleFile();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var file = BindingContext as ModuleFile;
		CourseServiceProxy.Current.AddOrUpdateModuleContent(
			ModuleDetailViewModel.CurrentCourseId, ModuleDetailViewModel.CurrentModuleId, file);
		await Shell.Current.GoToAsync("//ModuleDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ModuleDetail");
	}
}