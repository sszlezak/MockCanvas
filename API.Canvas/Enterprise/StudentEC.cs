using API.Canvas.Database;
using Library.Canvas.Model;

namespace API.Canvas.Enterprise
{
	public class StudentEC
	{
		public StudentEC() { }

		public IEnumerable<Student> Items => FakeDatabase.Current.Students;

		public Student? GetById(int id)
		{
			return FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == id);
		}

		public Student? AddOrUpdate(Student student)
		{
			if (student.Id == 0)
			{
				student.Id = NextKey;
				FakeDatabase.Current.Students.Add(student);
			}
			else
			{
				var existing = FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == student.Id);
				if (existing != null)
				{
					var index = FakeDatabase.Current.Students.IndexOf(existing);
					FakeDatabase.Current.Students.RemoveAt(index);
					FakeDatabase.Current.Students.Insert(index, student);
				}
			}

			return student;
		}

		public Student? Delete(int id)
		{
			var student = FakeDatabase.Current.Students.FirstOrDefault(s => s.Id == id);
			if (student != null)
			{
				// Cascade: remove from all course rosters
				foreach (var course in FakeDatabase.Current.Courses)
				{
					course.Roster.RemoveAll(s => s.Id == id);
				}

				FakeDatabase.Current.Students.Remove(student);
			}
			return student;
		}

		public int NextKey
		{
			get
			{
				if (Items.Any())
				{
					return Items.Select(s => s.Id).Max() + 1;
				}
				return 1;
			}
		}
	}
}