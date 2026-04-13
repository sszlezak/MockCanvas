using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ContentId), "contentId")]
public partial class PageContentDetailView : ContentPage
{
	public static int CurrentCourseId { get; set; }
	public static int CurrentModuleId { get; set; }

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
				.GetModuleContentById(CurrentCourseId, CurrentModuleId, ContentId) as ModulePage
				?? new ModulePage();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var page = BindingContext as ModulePage;
		CourseServiceProxy.Current.AddOrUpdateModuleContent(CurrentCourseId, CurrentModuleId, page);
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		ModuleDetailView.CurrentCourseId = CurrentCourseId;
		await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={CurrentModuleId}");
	}
}