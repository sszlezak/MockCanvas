using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

[QueryProperty(nameof(ModuleId), "moduleId")]
public partial class ModuleDetailView : ContentPage
{
	// Static parent context — set by TeacherCourseDetailView before navigating here.
	public static int CurrentCourseId { get; set; }

	public int ModuleId { get; set; }

	public ModuleDetailView()
	{
		InitializeComponent();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		if (ModuleId == 0)
		{
			BindingContext = new Module();
		}
		else
		{
			BindingContext = CourseServiceProxy.Current.GetModuleById(CurrentCourseId, ModuleId) ?? new Module();
		}
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		var module = BindingContext as Module;
		var saved = CourseServiceProxy.Current.AddOrUpdateModule(CurrentCourseId, module);
		if (saved != null) ModuleId = saved.Id;
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private bool EnsureModuleSaved()
	{
		var module = BindingContext as Module;
		if (module == null) return false;

		if (module.Id == 0)
		{
			var saved = CourseServiceProxy.Current.AddOrUpdateModule(CurrentCourseId, module);
			if (saved != null) ModuleId = saved.Id;
		}
		return ModuleId != 0;
	}

	private async void AddPageClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		PageContentDetailView.CurrentCourseId = CurrentCourseId;
		PageContentDetailView.CurrentModuleId = ModuleId;
		await Shell.Current.GoToAsync("//PageContentDetail?contentId=0");
	}

	private async void AddFileClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		FileContentDetailView.CurrentCourseId = CurrentCourseId;
		FileContentDetailView.CurrentModuleId = ModuleId;
		await Shell.Current.GoToAsync("//FileContentDetail?contentId=0");
	}

	private async void AddAssignmentRefClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		AssignmentRefContentDetailView.CurrentCourseId = CurrentCourseId;
		AssignmentRefContentDetailView.CurrentModuleId = ModuleId;
		await Shell.Current.GoToAsync("//AssignmentRefContentDetail?contentId=0");
	}

	private async void EditContentClicked(object sender, EventArgs e)
	{
		if (sender is not Button btn || btn.CommandParameter is not ModuleContent content) return;

		switch (content)
		{
			case ModulePage:
				PageContentDetailView.CurrentCourseId = CurrentCourseId;
				PageContentDetailView.CurrentModuleId = ModuleId;
				await Shell.Current.GoToAsync($"//PageContentDetail?contentId={content.Id}");
				break;
			case ModuleFile:
				FileContentDetailView.CurrentCourseId = CurrentCourseId;
				FileContentDetailView.CurrentModuleId = ModuleId;
				await Shell.Current.GoToAsync($"//FileContentDetail?contentId={content.Id}");
				break;
			case ModuleAssignment:
				AssignmentRefContentDetailView.CurrentCourseId = CurrentCourseId;
				AssignmentRefContentDetailView.CurrentModuleId = ModuleId;
				await Shell.Current.GoToAsync($"//AssignmentRefContentDetail?contentId={content.Id}");
				break;
		}
	}

	private async void DeleteContentClicked(object sender, EventArgs e)
	{
		if (sender is not Button btn || btn.CommandParameter is not ModuleContent content) return;

		bool confirm = await DisplayAlert("Delete Content",
			$"Delete '{content.Title}'?", "Delete", "Cancel");
		if (!confirm) return;

		CourseServiceProxy.Current.DeleteModuleContent(CurrentCourseId, ModuleId, content);
		BindingContext = CourseServiceProxy.Current.GetModuleById(CurrentCourseId, ModuleId) ?? new Module();
	}
}