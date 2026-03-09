using Library.Canvas.Model;

namespace Library.Canvas.Services
{
	public class CourseServiceProxy
	{
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
			Courses = new List<Course>();
		}

		public List<Course> Courses { get; set; }

		public int NextCourseId =>
			Courses.Any() ? Courses.Max(c => c.Id) + 1 : 1;

		public Course AddOrUpdate(Course course)
		{
			if (course.Id == 0)
			{
				course.Id = NextCourseId;
				Courses.Add(course);
			}

			return course;
		}

		public void Delete(Course course)
		{
			Courses.Remove(course);
		}
	}
}