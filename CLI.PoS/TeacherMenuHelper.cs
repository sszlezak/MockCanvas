using Library.PoS.Model;
using Library.PoS.Services;

namespace CLI.PoS
{
	internal class TeacherMenuHelper
	{
		private readonly CourseMenuHelper courseMenuHelper = new CourseMenuHelper();

		// Always read/write courses via the proxy (single source of truth)
		private List<Course> Courses => CourseServiceProxy.Current.Courses;

		public void PrintTeacherMenu()
		{
			bool running = true;

			while (running)
			{
				Console.WriteLine("\nTeacher Menu:");
				Console.WriteLine("1. Add new course");
				Console.WriteLine("2. Browse courses");
				Console.WriteLine("3. Select a student to proxy (later)");
				Console.WriteLine("4. Delete a course");
				Console.WriteLine("5. Copy a course");
				Console.WriteLine("0. Exit to main menu");

				var choice = Console.ReadLine();
				if (!int.TryParse(choice, out int choiceInt))
				{
					Console.WriteLine("Invalid input.");
					continue;
				}

				switch (choiceInt)
				{
					case 1:
						AddCourse();
						break;
					case 2:
						BrowseCourses();
						break;
					case 3:
						Console.WriteLine("Proxy student not implemented yet.");
						break;
					case 4:
						DeleteCourse();
						break;
					case 5:
						CopyCourse();
						break;
					case 0:
						running = false;
						break;
					default:
						Console.WriteLine("Unknown choice.");
						break;
				}
			}
		}

		private void AddCourse()
		{
			Console.Write("Course Name: ");
			var name = Console.ReadLine() ?? "";

			Console.Write("Course Code: ");
			var code = Console.ReadLine() ?? "";

			Console.Write("Course Description: ");
			var description = Console.ReadLine() ?? "";

			Console.Write("Semester (ex: Fall 2026): ");
			var semester = Console.ReadLine() ?? "";

			Console.Write("Section (ex: 001, A, B2): ");
			var section = Console.ReadLine() ?? "";

			var course = new Course
			{
				// Let proxy manage stable ID assignment if your AddOrUpdate does that,
				// but since it only assigns when Id==0, we keep Id=0 here.
				Id = 0,
				Name = name,
				Code = code,
				Description = description,
				Semester = semester,
				Section = section,

				// Ensure lists exist (avoid null surprises)
				Roster = new List<Student>(),
				Modules = new List<Module>(),
				Assignments = new List<Assignment>(),
				AssignmentGroups = new List<AssignmentGroup>()
			};

			CourseServiceProxy.Current.AddOrUpdate(course);

			Console.WriteLine($"Course added successfully. New Id = {course.Id}");
		}

		private void BrowseCourses()
		{
			if (!Courses.Any())
			{
				Console.WriteLine("No courses available.");
				return;
			}

			while (true)
			{
				Console.WriteLine("\nCourse Browser");
				Console.WriteLine("1. View courses grouped by semester");
				Console.WriteLine("2. View all courses");
				Console.WriteLine("3. Filter by semester");
				Console.WriteLine("4. Select a course");
				Console.WriteLine("0. Back");

				Console.Write("Choice: ");
				var choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						DisplayGroupedBySemester();
						break;
					case "2":
						DisplayAllCourses(Courses);
						break;
					case "3":
						FilterBySemester();
						break;
					case "4":
						SelectCourseFromList(Courses);
						break;
					case "0":
						return;
					default:
						Console.WriteLine("Invalid option.");
						break;
				}
			}
		}

		private void FilterBySemester()
		{
			Console.Write("Enter semester (ex: Fall 2025): ");
			var semester = Console.ReadLine() ?? "";

			var filtered = Courses
				.Where(c => string.Equals(c.Semester, semester, StringComparison.OrdinalIgnoreCase))
				.ToList();

			if (!filtered.Any())
			{
				Console.WriteLine("No courses found for that semester.");
				return;
			}

			DisplayAllCourses(filtered);
		}

		private void SelectCourseFromList(List<Course> list)
		{
			DisplayAllCourses(list);

			Console.Write("\nEnter the course Id to select: ");
			if (!int.TryParse(Console.ReadLine(), out int courseId))
			{
				Console.WriteLine("Invalid input.");
				return;
			}

			var selectedCourse = list.FirstOrDefault(c => c.Id == courseId);
			if (selectedCourse == null)
			{
				Console.WriteLine("Course not found.");
				return;
			}

			courseMenuHelper.PrintCourseMenu(selectedCourse, isTeacher: true);
		}

		private void DeleteCourse()
		{
			if (!Courses.Any())
			{
				Console.WriteLine("No courses available to delete.");
				return;
			}

			DisplayAllCourses(Courses);

			Console.Write("Enter the Id of the course to delete: ");
			if (!int.TryParse(Console.ReadLine(), out int id))
			{
				Console.WriteLine("Invalid input.");
				return;
			}

			var course = Courses.FirstOrDefault(c => c.Id == id);
			if (course == null)
			{
				Console.WriteLine("Course not found.");
				return;
			}

			CourseServiceProxy.Current.Delete(course);
			Console.WriteLine($"Course '{course.Name}' deleted (students not affected).");
		}

		private void DisplayAllCourses(List<Course> list)
		{
			Console.WriteLine("\nAvailable Courses:");
			foreach (var course in list)
			{
				Console.WriteLine($"{course.Id}: {course.Name} ({course.Code}) - {course.Semester} - Section {course.Section}");
			}
		}

		private void DisplayGroupedBySemester()
		{
			Console.WriteLine("\nCourses by Semester:");

			var grouped = Courses
				.GroupBy(c => c.Semester ?? "")
				.OrderBy(g => g.Key);

			foreach (var group in grouped)
			{
				Console.WriteLine($"\n=== {group.Key} ===");
				foreach (var course in group.OrderBy(c => c.Code))
				{
					Console.WriteLine($"{course.Id}: {course.Name} ({course.Code}) - Section {course.Section}");
				}
			}
		}

		private void CopyCourse()
		{
			if (!Courses.Any())
			{
				Console.WriteLine("No courses available to copy.");
				return;
			}

			DisplayAllCourses(Courses);

			Console.Write("Enter course Id to copy: ");
			if (!int.TryParse(Console.ReadLine(), out int id))
			{
				Console.WriteLine("Invalid input.");
				return;
			}

			var original = Courses.FirstOrDefault(c => c.Id == id);
			if (original == null)
			{
				Console.WriteLine("Course not found.");
				return;
			}

			var copy = DeepCopyCourse(original);

			// Add via proxy so it receives a stable new Id
			copy.Id = 0;
			CourseServiceProxy.Current.AddOrUpdate(copy);

			Console.WriteLine($"Course copied as '{copy.Name}' with Id {copy.Id}.");
		}

		/// <summary>
		/// Deep copy all course content except roster + student submissions.
		/// </summary>
		private Course DeepCopyCourse(Course original)
		{
			var newCourse = new Course
			{
				// Id is assigned by proxy; set to 0 later
				Id = 0,
				Name = original.Name + " (Copy)",
				Code = original.Code + "_COPY",
				Description = original.Description,
				Semester = original.Semester,
				Section = original.Section,

				// EXCLUDE roster
				Roster = new List<Student>(),

				Modules = new List<Module>(),
				Assignments = new List<Assignment>(),
				AssignmentGroups = new List<AssignmentGroup>()
			};

			// 1) Copy Modules + their ModuleContent items
			foreach (var module in original.Modules)
			{
				var newModule = new Module
				{
					Id = module.Id,
					Content = new List<ModuleContent>()
				};

				foreach (var item in module.Content)
				{
					newModule.Content.Add(item.Clone());
				}

				newCourse.Modules.Add(newModule);
			}

			// 2) Copy Assignments (but no submissions)
			foreach (var assignment in original.Assignments)
			{
				var newAssignment = new Assignment
				{
					Id = assignment.Id,
					Name = assignment.Name,
					Description = assignment.Description,
					AvailablePoints = assignment.AvailablePoints,
					DueDate = assignment.DueDate,
					GroupId = assignment.GroupId,

					// EXCLUDE submissions
					Submissions = new List<Submission>()
				};

				newCourse.Assignments.Add(newAssignment);
			}

			// 3) Copy Assignment Groups and link to copied assignments
			foreach (var group in original.AssignmentGroups)
			{
				var newGroup = new AssignmentGroup
				{
					Id = group.Id,
					Name = group.Name,
					Weight = group.Weight,
					Assignments = new List<Assignment>()
				};

				foreach (var oldAssignment in group.Assignments)
				{
					var copied = newCourse.Assignments.FirstOrDefault(a => a.Id == oldAssignment.Id);
					if (copied != null)
						newGroup.Assignments.Add(copied);
				}

				newCourse.AssignmentGroups.Add(newGroup);
			}

			return newCourse;
		}
	}
}