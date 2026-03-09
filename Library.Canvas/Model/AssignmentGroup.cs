namespace Library.Canvas.Model
{
	public class AssignmentGroup
	{
		public int Id { get; set; }

		public string Name { get; set; }

		// Used in next task (weights)
		public double Weight { get; set; }

		public List<Assignment> Assignments { get; set; } = new();
	}
}
