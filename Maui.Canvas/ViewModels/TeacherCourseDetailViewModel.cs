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

		public DateTime SemesterStartDate
		{
			get => SelectedCourse?.SemesterStart ?? DateTime.Now;
			set
			{
				if (SelectedCourse != null)
				{
					SelectedCourse.SemesterStart = value;
					NotifyPropertyChanged();
				}
			}
		}

		public DateTime SemesterEndDate
		{
			get => SelectedCourse?.SemesterEnd ?? DateTime.Now;
			set
			{
				if (SelectedCourse != null)
				{
					SelectedCourse.SemesterEnd = value;
					NotifyPropertyChanged();
				}
			}
		}

		// When the teacher selects a different course, the date pickers
		// need to update to show that course's dates
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
					// Tell the UI to re-read the date properties too
					// since they depend on which course is selected
					NotifyPropertyChanged("SemesterStartDate");
					NotifyPropertyChanged("SemesterEndDate");
				}
			}
		}

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

		public ObservableCollection<string> Announcements
		{
			get
			{
				if (Course == null) return new ObservableCollection<string>();
				return new ObservableCollection<string>(Course.Announcements);
			}
		}

		private string? _newAnnouncement;
		public string? NewAnnouncement
		{
			get => _newAnnouncement;
			set
			{
				if (_newAnnouncement != value)
				{
					_newAnnouncement = value;
					NotifyPropertyChanged();
				}
			}
		}

		public ObservableCollection<Module> Modules
		{
			get
			{
				if (Course == null) return new ObservableCollection<Module>();
				return new ObservableCollection<Module>(Course.Modules);
			}
		}

		public ObservableCollection<Student> Roster
		{
			get
			{
				if (Course == null) return new ObservableCollection<Student>();

				var filtered = Course.Roster.AsEnumerable();

				if (!string.IsNullOrWhiteSpace(RosterSearch))
				{
					var search = RosterSearch.ToLower();
					filtered = filtered.Where(s =>
						(s.Name != null && s.Name.ToLower().Contains(search))
						|| (s.Code != null && s.Code.ToLower().Contains(search))
						|| (s.Classification != null && s.Classification.ToLower().Contains(search))
					);
				}

				return new ObservableCollection<Student>(filtered);
			}
		}

		private string? _rosterSearch;
		public string? RosterSearch
		{
			get => _rosterSearch;
			set
			{
				if (_rosterSearch != value)
				{
					_rosterSearch = value;
					NotifyPropertyChanged();
					NotifyPropertyChanged("Roster");
				}
			}
		}

		public ObservableCollection<Student> AvailableStudents
		{
			get
			{
				if (Course == null) return new ObservableCollection<Student>();
				return new ObservableCollection<Student>(StudentServiceProxy.Current.Students
				.Where(s => !Course.Roster.Any(r => r.Id == s.Id)));
			}
		}

		public Student? SelectedAvailableStudent { get; set; }

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
			NotifyPropertyChanged("Modules");
			NotifyPropertyChanged("Roster");
			NotifyPropertyChanged("AvailableStudents");
			NotifyPropertyChanged("SemesterStartDate");
			NotifyPropertyChanged("SemesterEndDate");
			NotifyPropertyChanged("Announcements");
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