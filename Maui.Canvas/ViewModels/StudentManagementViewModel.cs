using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class StudentManagementViewModel : INotifyPropertyChanged
	{
		public ObservableCollection<Student> Students // refreshes every time it's accessed
		{
			get
			{
				return new ObservableCollection<Student>(StudentServiceProxy.Current.Students);
			}
		}

		public Student? SelectedStudent { get; set; }

		public void Delete()
		{
			// Cascade-delete is built into StudentServiceProxy.Delete
			StudentServiceProxy.Current.Delete(SelectedStudent);
			Refresh();
		}

		public void Refresh()
		{
			NotifyPropertyChanged("Students");
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}