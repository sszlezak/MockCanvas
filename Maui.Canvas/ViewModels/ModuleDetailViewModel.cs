using Library.Canvas.Model;
using Library.Canvas.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maui.Canvas.ViewModels
{
	public class ModuleDetailViewModel : INotifyPropertyChanged
	{
		public static int CurrentCourseId { get; set; }
		public static int CurrentModuleId { get; set; }

		private Module? module;

		public Module? Module => module;

		public string ModuleTitle => module == null ? "New Module" : $"Module {module.Id}";

		public ObservableCollection<ModuleContent> Content
		{
			get
			{
				if (module == null) return new ObservableCollection<ModuleContent>();
				return new ObservableCollection<ModuleContent>(module.Content);
			}
		}

		public void Load()
		{
			if (CurrentModuleId == 0)
			{
				module = new Module();
			}
			else
			{
				module = CourseServiceProxy.Current.GetModuleById(CurrentCourseId, CurrentModuleId)
					?? new Module();
			}
			Refresh();
		}

		public Module? Save()
		{
			var saved = CourseServiceProxy.Current.AddOrUpdateModule(CurrentCourseId, module);
			if (saved != null)
			{
				module = saved;
				CurrentModuleId = saved.Id;
			}
			return saved;
		}

		public void Refresh()
		{
			NotifyPropertyChanged("ModuleTitle");
			NotifyPropertyChanged("Content");
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}