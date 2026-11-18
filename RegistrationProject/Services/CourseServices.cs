using System;
using System.Collections.Generic;
using CourseRegistration.Models;
using CourseRegistration.Repository;
using System.Linq;

namespace CourseRegistration.Services
{
    public class CourseServices : ICourseServices
    {
        private CourseRepository _repo = new CourseRepository();


        //As a student, I want to search for course offerings that meet core goals 
        // so that I can register easily for courses that meet my program requirements
        public List<CourseOffering> getOfferingsByGoalIdAndSemester(String theGoalId, String semester) {
            CoreGoal theGoal = _repo.GetCoreGoalWithCoursesById(theGoalId);
            if(theGoal == null) throw new Exception("Didn't find the goal");

            List<CourseOffering> result = new List<CourseOffering>();
            foreach(CourseOffering o in _repo.GetOfferingsBySemester(semester)) {
                if(theGoal.Courses.Any(c => c.Name == o.TheCourse.Name)) result.Add(o);
            }
            return result;
        }

        /* As a student, I want to see all available courses so that I know what my options are */
        public List<Course> GetAllCourses() {
            List<Course> theCourses = _repo.GetAllCourses().ToList<Course>();
            return theCourses;
        }

        /* As a student, I want to see all course offerings by semester, so that I can choose from what's
           available to register for next semester */
        public List<CourseOffering> getCourseOfferingsBySemester(String semester) {
            return _repo.GetOfferingsBySemester(semester).ToList();
        }

        /* As a student I want to see all course offerings by semester and department so that I can 
        choose major courses to register for */
        public List<CourseOffering> getCourseOfferingsBySemesterAndDept(String semester, String dept) {
            return _repo.GetOfferingsBySemester(semester)
                .Where(o => o.TheCourse.Department.Equals(dept, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /* ---- Assignment #4 ---- */

        public Course GetCourseByName(string name)
        {
            Course c = _repo.GetCourseByName(name);
            return c;
        }

        public Course AddCoures(Course newCourse)
        {
            return _repo.InsertCourse(newCourse);
        }

        public List<Course> getCoursesByDepartment(String dept) {
            List<Course> result = _repo.GetAllCourses()
                .Where(c => c.Department.Equals(dept, StringComparison.OrdinalIgnoreCase))
                .ToList();
            result.Sort();
            return result;
        }

        public bool addCourse(Course course) {
            if(GetCourseByName(course.Name) != null) return false;
            return _repo.InsertCourse(course) != null;
        }

        public bool updateCourse(String name, Course updated) {
            if(GetCourseByName(name) == null) return false;
            return _repo.UpdateCourseByName(name, updated) != null;
        }

        public bool deleteCourse(String name) {
            if(GetCourseByName(name) == null) return false;
            return _repo.DeleteCourseByName(name);
        }

        public List<CoreGoal> getGoalsByCourseName(String name) {
            if(GetCourseByName(name) == null) return null;
            return _repo.GetCoreGoalsByCourseName(name).ToList();
        }

        public List<CourseOffering> getOfferingsByCourseNameAndSemester(String name, String semester) {
            if(GetCourseByName(name) == null) return null;
            return _repo.GetOfferingsBySemester(semester)
                .Where(o => o.TheCourse.Name == name)
                .ToList();
        }

        /* ---- CoreGoal CRUD ---- */

        public List<CoreGoal> GetAllCoreGoals() {
            return _repo.GetAllCoreGoals().ToList();
        }

        public CoreGoal GetCoreGoalById(string id) {
            return _repo.GetCoreGoalById(id);
        }

        public CoreGoal GetCoreGoalWithCoursesById(string id) {
            return _repo.GetCoreGoalWithCoursesById(id);
        }

        public List<Course> GetCoursesForCoreGoalById(string id) {
            if(_repo.GetCoreGoalById(id) == null) return null;
            return _repo.GetCoursesForCoreGoalById(id).ToList();
        }

        public bool addCoreGoal(CoreGoal goal) {
            if(_repo.GetCoreGoalById(goal.Id) != null) return false;
            return _repo.InsertCoreGoal(goal) != null;
        }

        public bool updateCoreGoal(string id, CoreGoal modified) {
            if(_repo.GetCoreGoalById(id) == null) return false;
            return _repo.UpdateCoreGoal(id, modified);
        }

        public bool addCourseToCoreGoal(string id, string courseName) {
            if(_repo.GetCoreGoalById(id) == null) return false;
            Course course = _repo.GetCourseByName(courseName);
            if(course == null) return false;
            return _repo.AddCoursetoCoreGoal(id, course);
        }

        public bool deleteCoreGoal(string id) {
            if(_repo.GetCoreGoalById(id) == null) return false;
            return _repo.DeleteCoreGoal(id);
        }

        /* As a student I want to see all courses that meet a core goal, so that I can plan out
           my courses over the next few semesters and choose core courses that make sense for me */

        /* As a student I want to find a course that meets two different core goals, so that I can
        "feed two birds with one seed" (save time by taking one class that will fulfill two 
          requirements */

        /* As a freshman adviser, I want to see all the core goals which do not have any course offerings 
           for a given semester, so that I can work with departments to get some courses offered
           that students can take to meet those goals */

        
     }
}
