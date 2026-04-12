using Library.Canvas.Model;

namespace Maui.Canvas.ViewModels
{
	public class SubmissionDetailViewModel
	{
		public static Assignment? CurrentAssignment { get; set; }
		public static Submission? CurrentSubmission { get; set; }

		public string AssignmentInfo => CurrentAssignment == null
			? ""
			: $"{CurrentAssignment.Name} ({CurrentAssignment.AvailablePoints} pts available)";

		public Submission? Submission => CurrentSubmission;
	}
}