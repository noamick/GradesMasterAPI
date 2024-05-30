using GradesMasterAPI.DB;
using GradesMasterAPI.DB.DbModels;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace GradesMasterAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentsController : ControllerBase
    {
        // GET: api/Student
        [HttpGet()]
        public IActionResult Get()
        {
            //Go to DB And Get All Data
            StudentsRepository studentRepo = new StudentsRepository();
            List<Student> students = studentRepo.GetAllStudents();
            return Ok(students);
        }

        // GET api/Students/5
        [HttpGet("{id}")]
        public IActionResult GetDetails(int id)
        {
            StudentsRepository studentRepo = new StudentsRepository();
            Student s = studentRepo.GetStudent(id);
            if (s != null) 
            {
                return Ok(s);
            }
            return NotFound("Student Not Found");
        }

        // POST api/<StudentsController>
        [HttpPost]
        public IActionResult Post([FromBody] Student studentInput)
        {
            StudentsRepository studentRepo = new StudentsRepository();
            string errors = studentRepo.InsertStudent(studentInput); 
            if(errors == string.Empty)
            {
                return Ok("Student Updated");
            }
            return BadRequest("Student Not Inserted:"+ errors);


        }

        // PUT api/<StudentsController>/5
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Student studentInput)
        {
            StudentsRepository studentRepo = new StudentsRepository();
            string result = studentRepo.UpdateStudent(id, studentInput);
            if (string.IsNullOrEmpty(result))
            {
                return Ok(new { message = "Student updated successfully" });
            }
            else
            {
                return BadRequest(new { error = result });
            }
        }


        // DELETE api/<StudentsController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
