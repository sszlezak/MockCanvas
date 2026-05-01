using Library.Canvas.Model;

namespace API.Canvas.Database
{
	public class FakeDatabase
	{
		private List<Student> students;
		private List<Course> courses;

		private FakeDatabase()
		{
			students = new List<Student>
			{
				new Student { Id = 1, Name = "Alice Johnson", Code = "aj22a", Classification = "Junior" },
				new Student { Id = 2, Name = "Bob Martinez", Code = "bm23b", Classification = "Sophomore" },
				new Student { Id = 3, Name = "Carla Nguyen", Code = "cn21c", Classification = "Senior" },
				new Student { Id = 4, Name = "Derek Patel", Code = "dp24d", Classification = "Freshman" },
				new Student { Id = 5, Name = "Emily Rivera", Code = "er22e", Classification = "Junior" }
			};

			var oop = new Course
			{
				Id = 1,
				Code = "COP3330",
				Name = "Object-Oriented Programming",
				Description = "Principles of OOP using C#.",
				Semester = "Fall 2025",
				Section = "01",
				Roster = new List<Student> { students[0], students[1], students[2] },
				Announcements = new List<string>
				{
					"Homework 2 deadline extended to next Friday.",
					"No class on Monday — enjoy the holiday."
				}
			};

			oop.Assignments.Add(new Assignment
			{
				Id = 1,
				Name = "Homework 1: Classes & Objects",
				Description = "Write a class that models a bank account.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(-7)
			});
			oop.Assignments.Add(new Assignment
			{
				Id = 2,
				Name = "Homework 2: Inheritance",
				Description = "Build a shape hierarchy using inheritance.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(7)
			});
			oop.Assignments.Add(new Assignment
			{
				Id = 3,
				Name = "Midterm",
				Description = "In-class midterm exam.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(14)
			});

			var oopModule = new Module { Id = 1 };
			oopModule.Content.Add(new ModulePage { Id = 1, Title = "Welcome", Content = "Welcome to OOP." });
			oop.Modules.Add(oopModule);

			var calc = new Course
			{
				Id = 2,
				Code = "MAC2311",
				Name = "Calculus I",
				Description = "Limits, derivatives, and basic integration.",
				Semester = "Fall 2025",
				Section = "02",
				Roster = new List<Student> { students[0], students[3], students[4] }
			};

			calc.Assignments.Add(new Assignment
			{
				Id = 1,
				Name = "Problem Set 1: Limits",
				Description = "Chapter 2 problems 1-20.",
				AvailablePoints = 50,
				DueDate = DateTime.Now.AddDays(3)
			});

			var calcModule = new Module { Id = 1 };
			calcModule.Content.Add(new ModulePage { Id = 1, Title = "Intro to Limits", Content = "A limit describes the value a function approaches." });
			calc.Modules.Add(calcModule);

			courses = new List<Course> { oop, calc };
		}

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