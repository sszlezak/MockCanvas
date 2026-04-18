using Library.Canvas.Model;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class PageContentDetailView : ContentPage
{
	public int ContentId { get; set; }

	public PageContentDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (ContentId == 0)
		{
			BindingContext = new ModulePage();
		}
		else
		{
			BindingContext = CourseServiceProxy.Current
				.GetModuleContentById(ModuleDetailViewModel.CurrentCourseId, ModuleDetailViewModel.CurrentModuleId, ContentId) as ModulePage
				?? new ModulePage();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var page = BindingContext as ModulePage;
		CourseServiceProxy.Current.AddOrUpdateModuleContent(
			ModuleDetailViewModel.CurrentCourseId,
			ModuleDetailViewModel.CurrentModuleId, page);
		await Shell.Current.GoToAsync("//ModuleDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//ModuleDetail");
	}
}