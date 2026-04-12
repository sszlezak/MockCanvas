using Library.Canvas.Model;

namespace Library.Canvas.Services
{
	public class CourseServiceProxy
	{
		private List<Course> courses;

		public List<Course> Courses => courses;

		private static CourseServiceProxy? instance;
		private static object instanceLock = new object();

		public static CourseServiceProxy Current
		{
			get
			{
				lock (instanceLock)
				{
					if (instance == null)
					{
						instance = new CourseServiceProxy();
					}
				}
				return instance;
			}
		}

		private CourseServiceProxy()
		{
			// Grab references to seeded students so rosters reuse them
			var alice = StudentServiceProxy.Current.GetById(1)!;
			var bob = StudentServiceProxy.Current.GetById(2)!;
			var carla = StudentServiceProxy.Current.GetById(3)!;
			var derek = StudentServiceProxy.Current.GetById(4)!;
			var emily = StudentServiceProxy.Current.GetById(5)!;

			// Build course: COP3330
			var oop = new Course
			{
				Id = 1,
				Code = "COP3330",
				Name = "Object-Oriented Programming",
				Description = "Principles of OOP using C#.",
				Semester = "Fall 2025",
				Section = "01",
				Roster = new List<Student> { alice, bob, carla }
			};

			// Assignment groups let the teacher weight different kinds of work.
			// Weight 0.6 = homework is 60% of the grade, Tests weight 0.4 = 40%.
			var oopHwGroup = new AssignmentGroup { Id = 1, Name = "Homework", Weight = 0.6 };
			var oopTestGroup = new AssignmentGroup { Id = 2, Name = "Tests", Weight = 0.4 };
			oop.AssignmentGroups.Add(oopHwGroup);
			oop.AssignmentGroups.Add(oopTestGroup);

			// A couple of assignments. One already has graded submissions so you
			// can see the grading UI in action immediately.
			var oopHw1 = new Assignment
			{
				Id = 1,
				Name = "Homework 1: Classes & Objects",
				Description = "Write a class that models a bank account.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(-7),
				GroupId = oopHwGroup.Id,
				Submissions = new List<Submission>
				{
					new Submission { Id = 1, StudentId = alice.Id, AssignmentId = 1,
						Content = "Alice's bank account code...", SubmissionDate = DateTime.Now.AddDays(-8),
						PointsEarned = 92, Grade = 92, Comment = "Nice work.", Feedback = "Nice work." },
					new Submission { Id = 2, StudentId = bob.Id, AssignmentId = 1,
						Content = "Bob's bank account code...", SubmissionDate = DateTime.Now.AddDays(-8),
						PointsEarned = 78, Grade = 78, Comment = "Missing deposit method.", Feedback = "Missing deposit method." }
				}
			};
			var oopHw2 = new Assignment
			{
				Id = 2,
				Name = "Homework 2: Inheritance",
				Description = "Build a shape hierarchy using inheritance.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(7),
				GroupId = oopHwGroup.Id
			};
			var oopTest1 = new Assignment
			{
				Id = 3,
				Name = "Midterm",
				Description = "In-class midterm exam.",
				AvailablePoints = 100,
				DueDate = DateTime.Now.AddDays(14),
				GroupId = oopTestGroup.Id
			};
			oop.Assignments.Add(oopHw1);
			oop.Assignments.Add(oopHw2);
			oop.Assignments.Add(oopTest1);

			// A module with mixed content types (page, assignment reference, file).
			// This exercises your polymorphic ModuleContent classes.
			var oopModule1 = new Module { Id = 1 };
			oopModule1.Content.Add(new ModulePage { Id = 1, Title = "Welcome", Content = "Welcome to OOP." });
			oopModule1.Content.Add(new ModuleAssignment { Id = 2, Title = "Homework 1 (Linked)", Assignment = oopHw1 });
			oopModule1.Content.Add(new ModuleFile { Id = 3, Title = "Syllabus", FileName = "syllabus.pdf", FilePath = "/docs/syllabus.pdf" });
			oop.Modules.Add(oopModule1);

			// Build the second course: MAC2311 (Calculus I)
			var calc = new Course
			{
				Id = 2,
				Code = "MAC2311",
				Name = "Calculus I",
				Description = "Limits, derivatives, and basic integration.",
				Semester = "Fall 2025",
				Section = "02",
				Roster = new List<Student> { alice, derek, emily }   // Alice is in both courses on purpose.
			};

			var calcHwGroup = new AssignmentGroup { Id = 3, Name = "Problem Sets", Weight = 0.7 };
			var calcQzGroup = new AssignmentGroup { Id = 4, Name = "Quizzes", Weight = 0.3 };
			calc.AssignmentGroups.Add(calcHwGroup);
			calc.AssignmentGroups.Add(calcQzGroup);

			var calcPs1 = new Assignment
			{
				Id = 4,
				Name = "Problem Set 1: Limits",
				Description = "Chapter 2 problems 1-20.",
				AvailablePoints = 50,
				DueDate = DateTime.Now.AddDays(3),
				GroupId = calcHwGroup.Id
			};
			calc.Assignments.Add(calcPs1);

			var calcModule1 = new Module { Id = 2 };
			calcModule1.Content.Add(new ModulePage
			{
				Id = 4,
				Title = "Intro to Limits",
				Content = "A limit describes the value a function approaches."
			});
			calc.Modules.Add(calcModule1);

			// Finally, add both courses to the list
			courses = new List<Course> { oop, calc };
		}

		public int NextKey => Courses.Any() ? Courses.Max(c => c.Id) + 1 : 1;

		public Course? GetById(int id)
		{
			if (id == 0) return null;
			return courses.FirstOrDefault(c => c.Id == id);
		}

		public Course? AddOrUpdate(Course? course)
		{
			if (course == null) return null;

			if (course.Id == 0)
			{
				course.Id = NextKey;
				courses.Add(course);
				return course;
			}

			var existing = courses.FirstOrDefault(c => c.Id == course.Id);
			if (existing != null)
			{
				var index = courses.IndexOf(existing);
				courses.RemoveAt(index);
				courses.Insert(index, course);
			}
			else
			{
				courses.Add(course);
			}

			return course;
		}

		// Removing the Course removes everything it held, no cascade needed
		public void Delete(Course? course)
		{
			if (course == null) return;
			courses.Remove(course);
		}

		// Add or update an assignment within a specific course
		public Assignment? AddOrUpdateAssignment(int courseId, Assignment? assignment)
		{
			if (assignment == null) return null;

			var course = GetById(courseId);
			if (course == null) return null;

			if (assignment.Id == 0)
			{
				// New assignment - assign next id based on all assignments in this course
				assignment.Id = course.Assignments.Any() ? course.Assignments.Max(a => a.Id) + 1 : 1;
				course.Assignments.Add(assignment);
				return assignment;
			}

			var existing = course.Assignments.FirstOrDefault(a => a.Id == assignment.Id);
			if (existing != null)
			{
				var index = course.Assignments.IndexOf(existing);
				course.Assignments.RemoveAt(index);
				course.Assignments.Insert(index, assignment);
			}
			else
			{
				course.Assignments.Add(assignment);
			}
			return assignment;
		}

		// Delete an assignment from a course, and also delete all submissions for that assignment
		public void DeleteAssignment(int courseId, Assignment? assignment)
		{
			if (assignment == null) return;
			var course = GetById(courseId);
			if (course == null) return;

			// Removing the assignment removes submissions automatically
			assignment.Submissions.Clear(); // for clarity, but not strictly necessary
			course.Assignments.Remove(assignment);
		}

		// Helper for the detail view: look up one assignment by course+id
		public Assignment? GetAssignmentById(int courseId, int assignmentId)
		{
			var course = GetById(courseId);
			if (course == null || assignmentId == 0) return null;
			return course.Assignments.FirstOrDefault(a => a.Id == assignmentId);
		}
	}
}