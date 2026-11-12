using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CourseRegistration.Models;
using CourseRegistration.Services;

namespace CourseRegistration.Controllers
{
    [ApiController]
    //[Route("[controller]")]
    [Route("courses")]
    public class CoursesController : ControllerBase
    {
        
        private ICourseServices _courseServices;

        public CoursesController(ICourseServices courseServices)
        {
            
            _courseServices = courseServices;
        }

        [HttpGet]
        public IActionResult getCourses()
        {
            try
            {
                IEnumerable<Course> list = _courseServices.getCourses();

                if(list !=null) return Ok(list);
                else return BadRequest();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        // GET /courses/ARTD 105
        [HttpGet("{name}")]
        public IActionResult getCourseByName(string name)
        {
            try
            {
                Course c = _courseServices.getCourseByName(name);
                if (c == null) return NotFound($"Course '{name}' not found");
                return Ok(c);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // GET /courses/search?dept=ARTS
        [HttpGet("search")]
        public IActionResult searchCoursesByDepartment([FromQuery] string dept)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dept)) return BadRequest("dept query parameter is required");
                return Ok(_courseServices.getCoursesByDepartment(dept));
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // POST /courses   (JSON body)
        [HttpPost]
        public IActionResult addCourse([FromBody] Course course)
        {
            try
            {
                if (course == null || string.IsNullOrWhiteSpace(course.Name))
                    return BadRequest("A course with a Name is required");
                if (!_courseServices.addCourse(course))
                    return Conflict($"Course '{course.Name}' already exists");
                return Created($"courses/{course.Name}", course);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // PUT /courses/ARTD 105   (JSON body)
        [HttpPut("{name}")]
        public IActionResult updateCourse(string name, [FromBody] Course course)
        {
            try
            {
                if (course == null) return BadRequest("Request body is required");
                if (!_courseServices.updateCourse(name, course)) return NotFound($"Course '{name}' not found");
                return Ok(_courseServices.getCourseByName(name));
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // DELETE /courses/ARTD 105
        [HttpDelete("{name}")]
        public IActionResult deleteCourse(string name)
        {
            try
            {
                if (!_courseServices.deleteCourse(name)) return NotFound($"Course '{name}' not found");
                return NoContent();
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // GET /courses/ARTD 105/goals
        [HttpGet("{name}/goals")]
        public IActionResult getGoalsForCourse(string name)
        {
            try
            {
                var goals = _courseServices.getGoalsByCourseName(name);
                if (goals == null) return NotFound($"Course '{name}' not found");
                return Ok(goals);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // GET /courses/ARTD 105/offerings?semester=Fall 2021
        [HttpGet("{name}/offerings")]
        public IActionResult getOfferingsForCourse(string name, [FromQuery] string semester)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(semester)) return BadRequest("semester query parameter is required");
                var offerings = _courseServices.getOfferingsByCourseNameAndSemester(name, semester);
                if (offerings == null) return NotFound($"Course '{name}' not found");
                return Ok(offerings);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
