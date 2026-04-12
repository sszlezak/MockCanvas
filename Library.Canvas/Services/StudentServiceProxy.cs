using Library.Canvas.Model;

namespace Library.Canvas.Services
{
	public class StudentServiceProxy
	{
		private List<Student> students;

		// Read-only access for anyone who needs to browse or search students
		public List<Student> Students => students;

		private static StudentServiceProxy? instance;
		private static object instanceLock = new object(); // one thread at a time can create the singleton instance

		public static StudentServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new StudentServiceProxy();
					}
				}
				return instance;
			}
		}

		private StudentServiceProxy() // accessible by Current property, but not from outside the class
		{
			// temp seed data
			students = new List<Student>
			{
				new Student { Id = 1, Name = "Alice Johnson", Code = "aj22a", Classification = "Junior" },
				new Student { Id = 2, Name = "Bob Martinez",  Code = "bm23b", Classification = "Sophomore" },
				new Student { Id = 3, Name = "Carla Nguyen",  Code = "cn21c", Classification = "Senior" },
				new Student { Id = 4, Name = "Derek Patel",   Code = "dp24d", Classification = "Freshman" },
				new Student { Id = 5, Name = "Emily Rivera",  Code = "er22e", Classification = "Junior" },
			};
		}

		// NextKey: auto-assigns the next available Id when adding a new student
		public int NextKey => Students.Any() ? Students.Max(s => s.Id) + 1 : 1; // if students is empty, start at 1

		public Student? GetById(int id)
		{
			if (id == 0) return null;
			return students.FirstOrDefault(s => s.Id == id); // default is null if not found
		}

		public Student? AddOrUpdate(Student? student)
		{
			if (student == null) return null;

			if (student.Id == 0) // If the student's Id is 0, treat as new: assign an Id and add
			{
				student.Id = NextKey;
				students.Add(student);
				return student;
			}

			// If the Id already exists in the list, replace that entry
			var existing = students.FirstOrDefault(s => s.Id == student.Id);
			if (existing != null)
			{
				var index = students.IndexOf(existing);
				students.RemoveAt(index);
				students.Insert(index, student);
			}
			else
			{
				students.Add(student); // If the Id is set but not in the list, add with the given Id
			}

			return student;
		}

		public void Delete(Student? student)
		{
			if (student == null) return;

			// Cascade to course rosters and submissions
			foreach (var course in CourseServiceProxy.Current.Courses)
			{
				course.Roster.RemoveAll(s => s.Id == student.Id); // Remove from every course roster they appear in

				foreach (var assignment in course.Assignments) // Remove all their submissions from every assignment
				{
					assignment.Submissions.RemoveAll(sub => sub.StudentId == student.Id);
				}
			}

			students.Remove(student);
		}
	}
}