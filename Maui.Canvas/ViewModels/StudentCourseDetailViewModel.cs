using Library.Canvas.Model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class StudentCourseDetailViewModel : INotifyPropertyChanged
	{
		public static Course? CurrentCourse { get; set; }
		public static Student? CurrentStudent { get; set; }

		public Course? Course => CurrentCourse;
		public Student? Student => CurrentStudent;

		public ObservableCollection<Assignment> Assignments { get; private set; } = new();
		public ObservableCollection<Module> Modules { get; private set; } = new();
		public ObservableCollection<Student> Roster { get; private set; } = new();

		public string CourseTitle => Course == null ? "" : $"{Course.Name} ({Course.Code})";

		public StudentCourseDetailViewModel()
		{
			Refresh();
		}

		public void Refresh()
		{
			Assignments.Clear();
			Modules.Clear();
			Roster.Clear();

			if (Course == null)
				return;

			foreach (var assignment in Course.Assignments)
				Assignments.Add(assignment);

			foreach (var module in Course.Modules)
				Modules.Add(module);

			foreach (var student in Course.Roster)
				Roster.Add(student);

			NotifyPropertyChanged(nameof(Course));
			NotifyPropertyChanged(nameof(Student));
			NotifyPropertyChanged(nameof(CourseTitle));
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}