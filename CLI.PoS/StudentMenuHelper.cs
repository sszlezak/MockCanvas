using Library.PoS.Model;
using Library.PoS.Services;

namespace CLI.PoS
{
    internal class StudentMenuHelper
    {
		private CourseMenuHelper courseMenuHelper = new CourseMenuHelper();

		public void PrintStudentMenu(Student student)
        {
			bool running = true;

			while (running)
			{
				Console.WriteLine("\nStudent Menu:");
				Console.WriteLine("1. See Course Menu");
					// var grades = course.GetStudentAssignmentGrades(student.Id);
					// var avg = course.GetStudentAverage(student.Id);
					// for opt. 2
				Console.WriteLine("0. Exit to main menu");

				var choice = Console.ReadLine();
				if (int.TryParse(choice, out int choiceInt))
				{
					switch (choiceInt)
					{
						case 1:
							ShowStudentCourses(student);
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
		}

		private void ShowStudentCourses(Student student) {
			var allCourses = CourseServiceProxy.Current.Courses;
			var myCourses = allCourses.Where(c => c.Roster.Contains(student)).ToList();
			// Find all courses the student is enrolled in
			if (myCourses.Count == 0)
			{
				Console.WriteLine("You are not enrolled in any courses.");
				return;
			}

			Console.WriteLine("Your courses:");
			foreach (var course in myCourses)
			{
				Console.WriteLine($"{course.Id}: {course.Name} ({course.Code})");
			}

			Console.Write("Enter course Id to open: ");
			if (!int.TryParse(Console.ReadLine(), out int courseId)) return;

			var selectedCourse = myCourses.FirstOrDefault(c => c.Id == courseId);
			if (selectedCourse == null)
			{
				Console.WriteLine("Course not found.");
				return;
			}

			// Enter course menu as student
			courseMenuHelper.PrintCourseMenu(selectedCourse, isTeacher: false);
		}
	}
}