using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class TeacherCourseDetailViewModel : INotifyPropertyChanged
	{
		public static Course? CurrentCourse { get; set; }

		public Course? Course => CurrentCourse;

		public string CourseTitle => Course == null ? "" : $"{Course.Name} ({Course.Code})";

		public ObservableCollection<AssignmentGroup> AssignmentGroups
		{
			get
			{
				if (Course == null) return new ObservableCollection<AssignmentGroup>();
				return new ObservableCollection<AssignmentGroup>(Course.AssignmentGroups);
			}
		}

		public ObservableCollection<Assignment> Assignments
		{
			get
			{
				if (Course == null) return new ObservableCollection<Assignment>();
				return new ObservableCollection<Assignment>(Course.Assignments);
			}
		}

		private Assignment? _selectedAssignment;
		public Assignment? SelectedAssignment
		{
			get => _selectedAssignment;
			set
			{
				if (_selectedAssignment != value)
				{
					_selectedAssignment = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("Submissions");
				}
			}
		}

		public ObservableCollection<SubmissionEntry> Submissions
		{
			get
			{
				if (Course == null || SelectedAssignment == null)
					return new ObservableCollection<SubmissionEntry>();

				var rows = new ObservableCollection<SubmissionEntry>();
				foreach (var student in Course.Roster)
				{
					var sub = SelectedAssignment.Submissions
						.FirstOrDefault(s => s.StudentId == student.Id);
					rows.Add(new SubmissionEntry { Student = student, Submission = sub });
				}
				return rows;
			}
		}

		public SubmissionEntry? SelectedSubmission { get; set; }

		public void SaveGroupWeight(AssignmentGroup group, string weightText)
		{
			if (double.TryParse(weightText, out double weight) && weight >= 0)
				group.Weight = weight;
		}

		public void Refresh()
		{
			NotifyPropertyChanged("CourseTitle");
			NotifyPropertyChanged("AssignmentGroups");
			NotifyPropertyChanged("Assignments");
			NotifyPropertyChanged("Submissions");
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

	public class SubmissionEntry
	{
		public Student Student { get; set; } = null!;
		public Submission? Submission { get; set; }

		public string StudentName => Student?.DisplayName ?? "";
		public string SubmittedOn => Submission == null ? "Not submitted" : Submission.SubmissionDate.ToString("MM/dd/yyyy");
		public string GradeDisplay => Submission?.PointsEarned == null ? "Not graded" : $"{Submission.PointsEarned:F1} pts ({Submission.Grade:F1}%)";
	}
}