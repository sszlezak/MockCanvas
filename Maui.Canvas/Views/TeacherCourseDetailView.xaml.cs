using Maui.Canvas.ViewModels;
using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Linq;

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

		// Populate the copy-from picker with all courses EXCEPT the current one.
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course != null)
		{
			CopySourcePicker.ItemsSource = CourseServiceProxy.Current.Courses
				.Where(c => c.Id != vm.Course.Id).ToList();
		}
	}

	private async void CopyAssignmentsClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		var source = CopySourcePicker.SelectedItem as Course;
		if (source == null)
		{
			await DisplayAlert("No Course Selected", "Pick a course to copy from.", "OK");
			return;
		}

		int count = CourseServiceProxy.Current.CopyAssignments(source.Id, vm.Course.Id);
		await DisplayAlert("Copied", $"{count} assignment(s) copied.", "OK");
		vm.Refresh();
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

		ModuleDetailViewModel.CurrentCourseId = vm.Course.Id;
		ModuleDetailViewModel.CurrentModuleId = 0;
		await Shell.Current.GoToAsync("//ModuleDetail");
	}

	private async void EditModuleClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Module module)
		{
			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;

			ModuleDetailViewModel.CurrentCourseId = vm.Course.Id;
			ModuleDetailViewModel.CurrentModuleId = module.Id;
			await Shell.Current.GoToAsync("//ModuleDetail");
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

	private void AddStudentClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		var selected = vm.SelectedAvailableStudent;
		if (selected == null)
		{
			DisplayAlert("No Student Selected", "Pick a student from the dropdown first.", "OK");
			return;
		}

		CourseServiceProxy.Current.AddStudentToCourse(vm.Course.Id, selected.Id);
		vm.Refresh();
	}

	private void RemoveStudentClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is Student student) // assigns student to student of remove button
		{
			var vm = BindingContext as TeacherCourseDetailViewModel;
			if (vm?.Course == null) return;
			CourseServiceProxy.Current.RemoveStudentFromCourse(vm.Course.Id, student.Id);
			vm.Refresh();
		}
	}

	private async void ExportRosterClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		var csv = CourseServiceProxy.Current.ExportRosterAsCsv(vm.Course.Id);

		// Write to a temp file, then let the user save it.
		var fileName = $"{vm.Course.Code}_roster.csv";
		var filePath = Path.Combine(FileSystem.CacheDirectory, fileName);
		File.WriteAllText(filePath, csv);

		// ShareFile opens the system share dialog so the user can save/email/etc.
		await Share.Default.RequestAsync(new ShareFileRequest
		{
			Title = $"Export {vm.Course.Code} Roster",
			File = new ShareFile(filePath)
		});
	}

	private async void ImportRosterClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		try
		{
			// FilePicker opens the system file browser.
			var result = await FilePicker.Default.PickAsync(new PickOptions
			{
				PickerTitle = "Select a roster CSV file",
				FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
				{
					{ DevicePlatform.WinUI, new[] { ".csv" } },
					{ DevicePlatform.Android, new[] { "text/csv" } },
					{ DevicePlatform.iOS, new[] { "public.comma-separated-values-text" } }
				})
			});

			if (result == null) return;  // User cancelled.

			var csvText = await File.ReadAllTextAsync(result.FullPath);
			var count = CourseServiceProxy.Current.ImportRosterFromCsv(vm.Course.Id, csvText);

			await DisplayAlert("Import Complete", $"{count} new student(s) imported.", "OK");
			vm.Refresh();
		}
		catch (Exception ex)
		{
			await DisplayAlert("Import Error", $"Failed to import: {ex.Message}", "OK");
		}
	}

	private void PostAnnouncementClicked(object sender, EventArgs e)
	{
		var vm = BindingContext as TeacherCourseDetailViewModel;
		if (vm?.Course == null) return;

		if (string.IsNullOrWhiteSpace(vm.NewAnnouncement)) return;

		vm.Course.Announcements.Insert(0, vm.NewAnnouncement); // Add new announcement to the top of the list
		vm.NewAnnouncement = "";
		vm.Refresh();
	}

	private void GoBackClicked(object sender, EventArgs e)
	{
		Shell.Current.GoToAsync("//TeacherMenu");
	}
}