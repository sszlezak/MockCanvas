using API.Canvas.Enterprise;
using Library.Canvas.Model;
using Microsoft.AspNetCore.Mvc;

namespace API.Canvas.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class StudentController : ControllerBase
	{
		[HttpGet]
		public IEnumerable<Student> Get()
		{
			return new StudentEC().Items;
		}

		[HttpGet("{id}")]
		public Student? GetById(int id)
		{
			return new StudentEC().GetById(id);
		}

		[HttpPost]
		public Student? AddOrUpdate([FromBody] Student student)
		{
			return new StudentEC().AddOrUpdate(student);
		}

		[HttpDelete("{id}")]
		public Student? Delete(int id)
		{
			return new StudentEC().Delete(id);
		}
	}
}