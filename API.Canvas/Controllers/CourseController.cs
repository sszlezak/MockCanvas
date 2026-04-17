using API.Canvas.Enterprise;
using Library.Canvas.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Canvas.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CourseController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Course> Get()
		{
			return new CourseEC().Items;
		}

		[HttpGet("{id}")]
		public Course? GetById(int id)
		{
			return new CourseEC().GetById(id);
		}

		[HttpPost]
		public Course? AddOrUpdate([FromBody] Course course)
		{
			return new CourseEC().AddOrUpdate(course);
		}

		[HttpDelete("{id}")]
		public Course? Delete(int id)
		{
			return new CourseEC().Delete(id);
		}
	}
}