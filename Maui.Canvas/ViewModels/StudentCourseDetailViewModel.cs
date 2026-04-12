using Library.Canvas.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	// Simple display helpers — no INotifyPropertyChanged needed, just data holders
	public class AssignmentGradeRow
	{
		public string AssignmentName { get; set; } = "";
		public string Grade { get; set; } = "";
	}

	public class ModuleContentRow
	{
		public string ContentType { get; set; } = "";
		public string Title { get; set; } = "";
	}

	public class ModuleRow
	{
		public string Header { get; set; } = "";
		public List<ModuleContentRow> Items { get; set; } = new();
	}

	public class StudentCourseDetailViewModel : INotifyPropertyChanged
	{
		public static Course? CurrentCourse { get; set; }
		public static Student? CurrentStudent { get; set; }

		public Course? Course => CurrentCourse;
		public Student? Student => CurrentStudent;

		public ObservableCollection<Assignment> Assignments { get; private set; } = new();
		public ObservableCollection<ModuleRow> Modules { get; private set; } = new();
		public ObservableCollection<Student> Roster { get; private set; } = new();
		public ObservableCollection<AssignmentGradeRow> Grades { get; private set; } = new();

		public string CourseTitle => Course == null ? "" : $"{Course.Name} ({Course.Code})";
		public string CourseAverage { get; private set; } = "";
		public string LetterGrade { get; private set; } = "";

		public StudentCourseDetailViewModel()
		{
			Refresh();
		}

		public void Refresh()
		{
			Assignments.Clear();
			Modules.Clear();
			Roster.Clear();
			Grades.Clear();

			if (Course == null) return;

			foreach (var assignment in Course.Assignments)
				Assignments.Add(assignment);

			foreach (var module in Course.Modules)
			{
				var row = new ModuleRow { Header = $"Module {module.Id}" };
				foreach (var content in module.Content)
				{
					string type = content switch
					{
						ModuleAssignment => "Assignment",
						ModuleFile => "File",
						ModulePage => "Page",
						_ => "Item"
					};
					row.Items.Add(new ModuleContentRow
					{
						ContentType = type,
						Title = content.Title ?? "(Untitled)"
					});
				}
				Modules.Add(row);
			}

			foreach (var student in Course.Roster)
				Roster.Add(student);

			if (Student != null)
			{
				foreach (var kvp in Course.GetStudentAssignmentGrades(Student.Id))
				{
					Grades.Add(new AssignmentGradeRow
					{
						AssignmentName = kvp.Key,
						Grade = kvp.Value.HasValue ? $"{kvp.Value.Value:F1}%" : "Not graded"
					});
				}

				double avg = Course.GetStudentAverage(Student.Id);
				CourseAverage = avg > 0 ? $"{avg:F1}%" : "N/A";
				LetterGrade = avg >= 90 ? "A"
					: avg >= 80 ? "B"
					: avg >= 70 ? "C"
					: avg >= 60 ? "D"
					: avg > 0 ? "F"
					: "N/A";
			}

			NotifyPropertyChanged(nameof(CourseTitle));
			NotifyPropertyChanged(nameof(CourseAverage));
			NotifyPropertyChanged(nameof(LetterGrade));
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}