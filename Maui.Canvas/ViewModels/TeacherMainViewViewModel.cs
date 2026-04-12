using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class TeacherMainViewViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<Course> Courses
		{
			get
			{
				return new ObservableCollection<Course>(CourseServiceProxy.Current.Courses);
			}
		}

		public Course? SelectedCourse { get; set; }

		public void Refresh()
		{
			NotifyPropertyChanged("Courses");
		}

		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}