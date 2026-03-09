using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class StudentMainViewViewModel : INotifyPropertyChanged
	{
		private Student? _selectedStudent;

		public ObservableCollection<Student> Students { get; private set; } = new();
		public ObservableCollection<Course> EnrolledCourses { get; private set; } = new();

		public Student? SelectedStudent
		{
			get => _selectedStudent;
			set
			{
				if (_selectedStudent != value)
				{
					_selectedStudent = value;
					NotifyPropertyChanged();
					_selectedCourse = null;
					NotifyPropertyChanged(nameof(SelectedCourse));
					LoadEnrolledCourses();
				}
			}
		}

		private Course? _selectedCourse;
		public Course? SelectedCourse
		{
			get => _selectedCourse;
			set
			{
				if (_selectedCourse != value)
				{
					_selectedCourse = value;
					NotifyPropertyChanged();
				}
			}
		}

		public StudentMainViewViewModel()
		{
			LoadStudents();
		}

		public void Refresh()
		{
			LoadStudents();
			LoadEnrolledCourses();
		}

		private void LoadStudents()
		{
			var students = CourseServiceProxy.Current.Courses
				.SelectMany(c => c.Roster)
				.GroupBy(s => s.Id)
				.Select(g => g.First())
				.ToList();

			Students.Clear();
			foreach (var student in students)
			{
				Students.Add(student);
			}
		}

		private void LoadEnrolledCourses()
		{
			EnrolledCourses.Clear();

			if (SelectedStudent == null)
			{
				return;
			}

			var courses = CourseServiceProxy.Current.Courses
				.Where(c => c.Roster.Any(s => s.Id == SelectedStudent.Id))
				.ToList();

			foreach (var course in courses)
			{
				EnrolledCourses.Add(course);
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}