using CLI.PoS;
using Library.PoS.Model;

namespace MockCanvas
{
	internal class Program
	{
		static void Main(string[] args)
		{

			var studentMenuHelper = new StudentMenuHelper();
			var teacherMenuHelper = new TeacherMenuHelper();
			bool appRunning = true;

			while (appRunning)
			{
				Console.WriteLine("Choose one of the following:");
				Console.WriteLine("1. Teacher");
				Console.WriteLine("2. Student");
				Console.WriteLine("3. Exit");

				var choice = Console.ReadLine();
				if (int.TryParse(choice, out int choiceInt))
				{
					switch (choiceInt)
					{
						case 1:
							Console.WriteLine("Teacher Menu");
							teacherMenuHelper.PrintTeacherMenu();
							break;
						case 2:
							Student student = new Student();
							Console.WriteLine("Student Menu");
							studentMenuHelper.PrintStudentMenu(student);
							break;
						case 3:
							appRunning = false;
							break;
						default:
							Console.WriteLine("ERROR: Unknown User Type");
							break;
					}
				}
			}
		}
	}
}