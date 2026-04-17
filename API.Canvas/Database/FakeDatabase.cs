using Library.Canvas.Model;

namespace API.Canvas.Database
{
	public class FakeDatabase
	{
		private List<Student> students;
		private List<Course> courses;

		private FakeDatabase()
		{
			// Seed students
			students = new List<Student>
			{
				new Student { Id = 1, Name = "Alice Johnson", Code = "aj22a", Classification = "Junior" },
				new Student { Id = 2, Name = "Bob Martinez", Code = "bm23b", Classification = "Sophomore" },
				new Student { Id = 3, Name = "Carla Nguyen", Code = "cn21c", Classification = "Senior" },
				new Student { Id = 4, Name = "Derek Patel", Code = "dp24d", Classification = "Freshman" },
				new Student { Id = 5, Name = "Emily Rivera", Code = "er22e", Classification = "Junior" }
			};

			// Seed courses with rosters referencing the same student objects
			courses = new List<Course>
			{
				new Course
				{
					Id = 1,
					Code = "COP3330",
					Name = "Object-Oriented Programming",
					Description = "Principles of OOP using C#.",
					Semester = "Fall 2025",
					Section = "01",
					Roster = new List<Student>
					{
						students[0], students[1], students[2]   // Alice, Bob, Carla
                    }
				},
				new Course
				{
					Id = 2,
					Code = "MAC2311",
					Name = "Calculus I",
					Description = "Limits, derivatives, and basic integration.",
					Semester = "Fall 2025",
					Section = "02",
					Roster = new List<Student>
					{
						students[0], students[3], students[4]   // Alice, Derek, Emily
                    }
				}
			};
		}

		// Singleton — same pattern as your professor's FakeDatabase.
		private static FakeDatabase? instance;
		public static FakeDatabase Current
		{
			get
			{
				if (instance == null)
				{
					instance = new FakeDatabase();
				}
				return instance;
			}
		}

		public List<Student> Students => students;
		public List<Course> Courses => courses;
	}
}