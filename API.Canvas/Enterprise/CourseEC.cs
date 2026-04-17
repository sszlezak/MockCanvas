using API.Canvas.Database;
using Library.Canvas.Model;

namespace API.Canvas.Enterprise
{
	public class CourseEC
	{
		public CourseEC() { }

		public IEnumerable<Course> Items => FakeDatabase.Current.Courses;

		public Course? GetById(int id)
		{
			return FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == id);
		}

		public Course? AddOrUpdate(Course course)
		{
			if (course.Id == 0)
			{
				course.Id = NextKey;
				FakeDatabase.Current.Courses.Add(course);
			}
			else
			{
				var existing = FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == course.Id);
				if (existing != null)
				{
					var index = FakeDatabase.Current.Courses.IndexOf(existing);
					FakeDatabase.Current.Courses.RemoveAt(index);
					FakeDatabase.Current.Courses.Insert(index, course);
				}
			}

			return course;
		}

		public Course? Delete(int id)
		{
			var course = FakeDatabase.Current.Courses.FirstOrDefault(c => c.Id == id);
			if (course != null)
			{
				FakeDatabase.Current.Courses.Remove(course);
			}
			return course;
		}

		public int NextKey
		{
			get
			{
				if (Items.Any())
				{
					return Items.Select(c => c.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}