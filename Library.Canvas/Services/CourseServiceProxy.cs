using Library.Canvas.Model;
using Library.Canvas.Utility;
using Newtonsoft.Json;

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
			// Fetch courses from the API instead of hardcoding.
			var stringFromAPI = new WebRequestHandler().Get("/Course").Result;
			courses = JsonConvert.DeserializeObject<List<Course>>(stringFromAPI)
				?? new List<Course>();
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

			var stringFromAPI = new WebRequestHandler().Post("/Course", course).Result;
			var courseFromAPI = JsonConvert.DeserializeObject<Course>(stringFromAPI);

			if (courseFromAPI == null) return course;

			var existing = courses.FirstOrDefault(c => c.Id == courseFromAPI.Id);
			if (existing != null)
			{
				var index = courses.IndexOf(existing);
				courses.RemoveAt(index);
				courses.Insert(index, courseFromAPI);
			}
			else
			{
				courses.Add(courseFromAPI);
			}

			return courseFromAPI;
		}

		public void Delete(Course? course)
		{
			if (course == null) return;
			new WebRequestHandler().Delete($"/Course/{course.Id}").Wait();
			courses.Remove(course);
		}

		// --- Everything below operates on the LOCAL cached course. ---
		// These don't call the API yet. When you add Assignment/Module
		// controllers in the future, you'd update these the same way
		// we updated the top-level methods above.

		public Assignment? AddOrUpdateAssignment(int courseId, Assignment? assignment)
		{
			if (assignment == null) return null;
			var course = GetById(courseId);
			if (course == null) return null;

			if (assignment.Id == 0)
			{
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

		public void DeleteAssignment(int courseId, Assignment? assignment)
		{
			if (assignment == null) return;
			var course = GetById(courseId);
			if (course == null) return;
			assignment.Submissions.Clear();
			course.Assignments.Remove(assignment);
		}

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

		public ModuleContent? GetModuleContentById(int courseId, int moduleId, int contentId)
		{
			var module = GetModuleById(courseId, moduleId);
			if (module == null || contentId == 0) return null;
			return module.Content.FirstOrDefault(c => c.Id == contentId);
		}

		public void AddStudentToCourse(int courseId, int studentId)
		{
			var stu = StudentServiceProxy.Current.GetById(studentId);
			var course = GetById(courseId);
			if (course == null) return;
			if (stu == null) return;

			if (!course.Roster.Contains(stu))
			{
				course.Roster.Add(stu);
			}
		}

		public void RemoveStudentFromCourse(int courseId, int studentId)
		{
			var stu = StudentServiceProxy.Current.GetById(studentId);
			var course = GetById(courseId);
			if (course == null) return;
			if (stu == null) return;
			course.Roster.Remove(stu);
		}

		public string ExportRosterAsCsv(int courseId)
		{
			var course = GetById(courseId);
			if (course == null) return "";

			var sb = new System.Text.StringBuilder();
			sb.AppendLine("Id,Name,Code,Classification");

			foreach (var student in course.Roster)
			{
				sb.AppendLine($"{student.Id},\"{student.Name}\",{student.Code},{student.Classification}");
			}

			return sb.ToString();
		}

		public int ImportRosterFromCsv(int courseId, string csvText)
		{
			var course = GetById(courseId);
			if (course == null) return 0;

			var lines = csvText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
			int imported = 0;

			for (int i = 1; i < lines.Length; i++)
			{
				var line = lines[i].Trim();
				if (string.IsNullOrEmpty(line)) continue;

				var parts = ParseCsvLine(line);
				if (parts.Count < 4) continue;

				if (!int.TryParse(parts[0], out int studentId)) continue;

				var student = StudentServiceProxy.Current.GetById(studentId);

				if (student == null)
				{
					student = new Student
					{
						Id = studentId,
						Name = parts[1],
						Code = parts[2],
						Classification = parts[3]
					};
					StudentServiceProxy.Current.AddOrUpdate(student);
				}

				if (!course.Roster.Any(r => r.Id == student.Id))
				{
					course.Roster.Add(student);
					imported++;
				}
			}

			return imported;
		}

		private List<string> ParseCsvLine(string line)
		{
			var result = new List<string>();
			bool inQuotes = false;
			var current = new System.Text.StringBuilder();

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