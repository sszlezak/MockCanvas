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

		public Module? AddOrUpdateModule(int courseId, Module? module)
		{
			if (module == null) return null;

			var course = GetById(courseId);
			if (course == null) return null;

			if (module.Id == 0)
			{
				module.Id = course.Modules.Any() ? course.Modules.Max(a => a.Id) + 1 : 1;
				course.Modules.Add(module);
				return module;
			}

			var existing = course.Modules.FirstOrDefault(a => a.Id == module.Id);
			if (existing != null)
			{
				var index = course.Modules.IndexOf(existing);
				course.Modules.RemoveAt(index);
				course.Modules.Insert(index, module);
			}
			else
			{
				course.Modules.Add(module);
			}
			return module;
		}

		public void DeleteModule(int courseId, Module? module)
		{
			if (module == null) return;
			var course = GetById(courseId);
			if (course == null) return;

			module.Content.Clear();
			course.Modules.Remove(module);
		}

		public Module? GetModuleById(int courseId, int moduleId)
		{
			var course = GetById(courseId);
			if (course == null || moduleId == 0) return null;
			return course.Modules.FirstOrDefault(m => m.Id == moduleId);
		}

		// Add or update a piece of content within a specific module of a course
		public ModuleContent? AddOrUpdateModuleContent(int courseId, int moduleId, ModuleContent? content)
		{
			if (content == null) return null;
			var module = GetModuleById(courseId, moduleId);
			if (module == null) return null;

			if (content.Id == 0)
			{
				content.Id = module.Content.Any() ? module.Content.Max(c => c.Id) + 1 : 1;
				module.Content.Add(content);
				return content;
			}

			var existing = module.Content.FirstOrDefault(c => c.Id == content.Id);
			if (existing != null)
			{
				var index = module.Content.IndexOf(existing);
				module.Content.RemoveAt(index);
				module.Content.Insert(index, content);
			}
			else
			{
				module.Content.Add(content);
			}
			return content;
		}

		public void DeleteModuleContent(int courseId, int moduleId, ModuleContent? content)
		{
			if (content == null) return;
			var module = GetModuleById(courseId, moduleId);
			if (module == null) return;
			module.Content.Remove(content);
		}

		// Look up one piece of content; Used by the content detail views
		public ModuleContent? GetModuleContentById(int courseId, int moduleId, int contentId)
		{
			var module = GetModuleById(courseId, moduleId);
			if (module == null || contentId == 0) return null;
			return module.Content.FirstOrDefault(c => c.Id == contentId);
		}

		public void AddStudentToCourse(int courseId, int studentId) {
			var stu = StudentServiceProxy.Current.GetById(studentId);
			var course = GetById(courseId);
			if (course == null) return;
			if (stu == null) return;

			if (!course.Roster.Contains(stu))
			{
				course.Roster.Add(stu);
			}
		}

		public void RemoveStudentFromCourse(int courseId, int studentId) {
			var stu = StudentServiceProxy.Current.GetById(studentId);
			var course = GetById(courseId);
			if (course == null) return;
			if (stu == null) return;
			course.Roster.Remove(stu);
		}

		// Export a course's roster as CSV text.
		// Returns a string like:
		//   Id,Name,Code,Classification
		//   1,Alice Johnson,aj22a,Junior
		//   2,Bob Martinez,bm23b,Sophomore
		public string ExportRosterAsCsv(int courseId)
		{
			var course = GetById(courseId);
			if (course == null) return "";

			var sb = new System.Text.StringBuilder();
			sb.AppendLine("Id,Name,Code,Classification");

			foreach (var student in course.Roster)
			{
				// Wrap Name in quotes in case it contains commas.
				sb.AppendLine($"{student.Id},\"{student.Name}\",{student.Code},{student.Classification}");
			}

			return sb.ToString();
		}

		// Import students from CSV text into a course's roster.
		// Idempotent: skips students already enrolled.
		// Non-destructive: doesn't remove students not in the file.
		// If a student Id from the CSV exists in StudentServiceProxy,
		// the existing student is used. Otherwise a new student is created.
		public int ImportRosterFromCsv(int courseId, string csvText)
		{
			var course = GetById(courseId);
			if (course == null) return 0;

			var lines = csvText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			int imported = 0;

			// Skip the header row (line 0).
			for (int i = 1; i < lines.Length; i++)
			{
				var line = lines[i].Trim();
				if (string.IsNullOrEmpty(line)) continue;

				// Simple CSV parse — handle quoted names.
				var parts = ParseCsvLine(line);
				if (parts.Count < 4) continue;

				if (!int.TryParse(parts[0], out int studentId)) continue;

				// Check if this student already exists in the university.
				var student = StudentServiceProxy.Current.GetById(studentId);

				if (student == null)
				{
					// Student doesn't exist — create them.
					student = new Student
					{
						Id = studentId,
						Name = parts[1],
						Code = parts[2],
						Classification = parts[3]
					};
					StudentServiceProxy.Current.AddOrUpdate(student);
				}

				// Idempotent: only add if not already enrolled.
				if (!course.Roster.Any(r => r.Id == student.Id))
				{
					course.Roster.Add(student);
					imported++;
				}
			}

			return imported;
		}

		// Simple CSV line parser that handles quoted fields.
		// "Alice Johnson",aj22a -> ["Alice Johnson", "aj22a"]
		private List<string> ParseCsvLine(string line)
		{
			var result = new List<string>();
			bool inQuotes = false;
			var current = new System.Text.StringBuilder(); // an efficient way to build a long string piece by piece

			for (int i = 0; i < line.Length; i++)
			{
				char c = line[i];

				if (c == '"')
				{
					inQuotes = !inQuotes;
				}
				else if (c == ',' && !inQuotes)
				{
					result.Add(current.ToString().Trim());
					current.Clear();
				}
				else
				{
					current.Append(c);
				}
			}

			result.Add(current.ToString().Trim());
			return result;
		}
	}
}