using Library.Canvas.Model;
using Library.Canvas.Utility;
using Newtonsoft.Json;

namespace Library.Canvas.Services
{
	public class StudentServiceProxy
	{
		// The local list is now a CACHE of what the server holds.
		// It gets populated from the API on startup.
		private List<Student> students;

		public List<Student> Students => students;

		private static StudentServiceProxy? instance;
		private static object instanceLock = new object();

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

		private StudentServiceProxy()
		{
			// Instead of hardcoded seed data, fetch from the API.
			var stringFromAPI = new WebRequestHandler().Get("/Student").Result;
			students = JsonConvert.DeserializeObject<List<Student>>(stringFromAPI, JsonHelper.Settings)
			?? new List<Student>();
		}

		public int NextKey => Students.Any() ? Students.Max(s => s.Id) + 1 : 1;

		public Student? GetById(int id)
		{
			if (id == 0) return null;
			return students.FirstOrDefault(s => s.Id == id);
		}

		public Student? AddOrUpdate(Student? student)
		{
			if (student == null) return null;

			// Send to the API. The server assigns the Id if new.
			var stringFromAPI = new WebRequestHandler().Post("/Student", student).Result;
			var studentFromAPI = JsonConvert.DeserializeObject<Student>(stringFromAPI, JsonHelper.Settings);
			if (studentFromAPI == null) return student;

			var existing = students.FirstOrDefault(s => s.Id == studentFromAPI.Id);
			if (existing != null)
			{
				var index = students.IndexOf(existing);
				students.RemoveAt(index);
				students.Insert(index, studentFromAPI);
			}
			else
			{
				students.Add(studentFromAPI);
			}

			return studentFromAPI;
		}

		public void Delete(Student? student)
		{
			if (student == null) return;

			// Tell the API to delete. Server handles cascade.
			new WebRequestHandler().Delete($"/Student/{student.Id}").Wait();

			students.Remove(student);
		}
	}
}