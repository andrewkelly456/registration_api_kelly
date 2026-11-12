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
    [Route("[controller]")]
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
    }
}
