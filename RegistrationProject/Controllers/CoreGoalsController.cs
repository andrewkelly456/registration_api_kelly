using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CourseRegistration.Models;
using CourseRegistration.Services;

namespace CourseRegistration.Controllers
{
    [ApiController]
    [Route("coregoals")]
    public class CoreGoalsController : ControllerBase
    {
        private ICourseServices _courseServices;

        public CoreGoalsController(ICourseServices courseServices)
        {
            _courseServices = courseServices;
        }

        // GET /coregoals
        [HttpGet]
        public IActionResult GetAllCoreGoals()
        {
            try
            {
                return Ok(_courseServices.GetAllCoreGoals());
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // GET /coregoals/CG1                      -> the goal only
        // GET /coregoals/CG1?includeCourses=true  -> the goal with its courses
        [HttpGet("{id}")]
        public IActionResult GetCoreGoalById(string id, [FromQuery] bool includeCourses = false)
        {
            try
            {
                CoreGoal goal = includeCourses
                    ? _courseServices.GetCoreGoalWithCoursesById(id)
                    : _courseServices.GetCoreGoalById(id);
                if (goal == null) return NotFound($"Core goal '{id}' not found");
                return Ok(goal);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // GET /coregoals/CG1/courses
        [HttpGet("{id}/courses")]
        public IActionResult GetCoursesForCoreGoal(string id)
        {
            try
            {
                List<Course> courses = _courseServices.GetCoursesForCoreGoalById(id);
                if (courses == null) return NotFound($"Core goal '{id}' not found");
                return Ok(courses);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // POST /coregoals   (JSON body, optional "courses" list of existing courses)
        [HttpPost]
        public IActionResult AddCoreGoal([FromBody] CoreGoal goal)
        {
            try
            {
                if (goal == null || string.IsNullOrWhiteSpace(goal.Id))
                    return BadRequest("A core goal with an Id is required");
                if (!_courseServices.addCoreGoal(goal))
                    return Conflict($"Core goal '{goal.Id}' already exists, or one of its courses does not exist");
                return Created($"coregoals/{goal.Id}", goal);
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // PUT /coregoals/CG1   (JSON body)
        [HttpPut("{id}")]
        public IActionResult UpdateCoreGoal(string id, [FromBody] CoreGoal goal)
        {
            try
            {
                if (goal == null) return BadRequest("Request body is required");
                if (!_courseServices.updateCoreGoal(id, goal)) return NotFound($"Core goal '{id}' not found");
                return Ok(_courseServices.GetCoreGoalById(id));
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // PUT /coregoals/CG1/courses   (JSON body: an existing course, e.g. {"name":"ARTD 105"})
        [HttpPut("{id}/courses")]
        public IActionResult AddCourseToCoreGoal(string id, [FromBody] Course course)
        {
            try
            {
                if (course == null || string.IsNullOrWhiteSpace(course.Name))
                    return BadRequest("A course with a Name is required");
                if (_courseServices.GetCoreGoalById(id) == null) return NotFound($"Core goal '{id}' not found");
                if (_courseServices.GetCourseByName(course.Name) == null) return NotFound($"Course '{course.Name}' not found");
                if (!_courseServices.addCourseToCoreGoal(id, course.Name))
                    return Conflict($"Course '{course.Name}' already meets core goal '{id}'");
                return Ok(_courseServices.GetCoreGoalWithCoursesById(id));
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }

        // DELETE /coregoals/CG1
        [HttpDelete("{id}")]
        public IActionResult DeleteCoreGoal(string id)
        {
            try
            {
                if (!_courseServices.deleteCoreGoal(id)) return NotFound($"Core goal '{id}' not found");
                return NoContent();
            }
            catch (Exception) { return StatusCode(500, "Internal server error"); }
        }
    }
}
