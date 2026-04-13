using Maui.Canvas.ViewModels;
using Library.Canvas.Model;
using Library.Canvas.Services;

namespace Maui.Canvas.Views;

public partial class TeacherCourseDetailView : ContentPage
{
	public TeacherCourseDetailView()
	{
		InitializeComponent();
		BindingContext = new TeacherCourseDetailViewModel();
	}

	private void ContentPage_NavigatedTo(object sender, NavigatedToEventArgs e)
	{
		(BindingContext as TeacherCourseDetailViewModel)?.Refresh();
	}

	private void SaveWeightClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (sender is Button btn && btn.CommandParameter is AssignmentGroup group)
		{
			if (btn.Parent is Grid grid)
			{
				var entry = grid.Children.OfType<Entry>().FirstOrDefault();
				if (entry != null)
					vm?.SaveGroupWeight(group, entry.Text);
			}
		}
	}

	private async void GradeClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is SubmissionEntry entry)
		{
			if (entry.Submission == null)
			{
				await DisplayAlert("No Submission", "This student has not submitted yet.", "OK");
				return;
			}

			SubmissionDetailViewModel.CurrentSubmission = entry.Submission;
			SubmissionDetailViewModel.CurrentAssignment =
				(BindingContext as TeacherCourseDetailViewModel)?.SelectedAssignment;

			await Shell.Current.GoToAsync("//SubmissionDetail");
		}
	}

	private async void AddAssignmentClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		AssignmentDetailView.CurrentCourseId = vm.Course.Id;
		await Shell.Current.GoToAsync("//AssignmentDetail?assignmentId=0");
	}

	private async void EditAssignmentClicked(object sender, EventArgs e)
	{
		// CommandParameter: carries the Assignment object the button belongs to
		if (sender is Button btn && btn.CommandParameter is Assignment assignment)
		{
			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;

			AssignmentDetailView.CurrentCourseId = vm.Course.Id;
			await Shell.Current.GoToAsync($"//AssignmentDetail?assignmentId={assignment.Id}");
		}
	}

	private async void DeleteAssignmentClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Assignment assignment)
		{
			// Confirm before destroying data. DisplayAlert with two buttons returns
			// true if the user pressed the first, false for the second
			bool confirm = await DisplayAlert(
				"Delete Assignment",
				$"Delete '{assignment.Name}' and all its submissions?",
				"Delete", "Cancel");

			if (!confirm) return;

			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;

			CourseServiceProxy.Current.DeleteAssignment(vm.Course.Id, assignment);
			vm.Refresh(); // Re-bind the list so the deleted row disappears
		}
	}

	private async void AddModuleClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		ModuleDetailView.CurrentCourseId = vm.Course.Id;
		await Shell.Current.GoToAsync("//ModuleDetail?moduleId=0");
	}

	private async void EditModuleClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Module module)
		{
			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;

			ModuleDetailView.CurrentCourseId = vm.Course.Id;
			await Shell.Current.GoToAsync($"//ModuleDetail?moduleId={module.Id}");
		}
	}

	private async void DeleteModuleClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Module module)
		{
			// Confirm before destroying data. DisplayAlert with two buttons returns
			// true if the user pressed the first, false for the second
			bool confirm = await DisplayAlert(
				"Delete Module",
				$"Delete 'Module {module.Id}' and all its submissions?",
				"Delete", "Cancel");

			if (!confirm) return;

			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;

			CourseServiceProxy.Current.DeleteModule(vm.Course.Id, module);
			vm.Refresh(); // Re-bind the list so the deleted row disappears
		}
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}