using Library.Canvas.Model;
using Library.Canvas.Services;
using Maui.Canvas.ViewModels;

namespace Maui.Canvas.Views;

public partial class ModuleDetailView : ContentPage
{
	private ModuleDetailViewModel ViewModel => BindingContext as ModuleDetailViewModel;

	public ModuleDetailView()
	{
		InitializeComponent();
		BindingContext = new ModuleDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		// Every time we arrive at this page, reload from the proxy.
		// This catches new content added by child detail views.
		ViewModel?.Load();
	}

	private async void OkClicked(object sender, EventArgs e)
	{
		ViewModel?.Save();
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private async void GoBackClicked(object sender, EventArgs e)
	{
		await Shell.Current.GoToAsync("//TeacherCourseDetail");
	}

	private bool EnsureModuleSaved()
	{
		if (ViewModel == null) return false;

		if (ModuleDetailViewModel.CurrentModuleId == 0)
		{
			ViewModel.Save();
		}
		return ModuleDetailViewModel.CurrentModuleId != 0;
	}

	private async void AddPageClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		await Shell.Current.GoToAsync("//PageContentDetail?contentId=0");
	}

	private async void AddFileClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		await Shell.Current.GoToAsync("//FileContentDetail?contentId=0");
	}

	private async void AddAssignmentRefClicked(object sender, EventArgs e)
	{
		if (!EnsureModuleSaved()) return;
		await Shell.Current.GoToAsync("//AssignmentRefContentDetail?contentId=0");
	}

	private async void EditContentClicked(object sender, EventArgs e)
	{
		if (sender is not Button btn || btn.CommandParameter is not ModuleContent content) return;

		switch (content)
		{
			case ModulePage:
				await Shell.Current.GoToAsync($"//PageContentDetail?contentId={content.Id}");
				break;
			case ModuleFile:
				await Shell.Current.GoToAsync($"//FileContentDetail?contentId={content.Id}");
				break;
			case ModuleAssignment:
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

		CourseServiceProxy.Current.DeleteModuleContent(
			ModuleDetailViewModel.CurrentCourseId,
			ModuleDetailViewModel.CurrentModuleId,
			content);
		ViewModel?.Refresh();
	}
}