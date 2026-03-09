namespace Library.Canvas.Model
{
	public class Student : User
	{
		public string? Classification { get; set; }

		public string? DisplayName // for student picker
		{
			get
			{
				return $"{Name} ({Code})";
			}
		}
	}
}