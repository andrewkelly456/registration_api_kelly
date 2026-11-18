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
          //finish this method during the tutorial 
          //use the repo to get the data from the database (data store)
         List<CoreGoal> theGoals = _repo.Goals;
         List<CourseOffering> theOfferings = _repo.Offerings;

         //Complete any other required functionality/business logic to satisfy the requirement
         CoreGoal theGoal=null;
         foreach(CoreGoal cg in theGoals) {
         if(cg.Id.Equals(theGoalId)) {
            theGoal=cg; break;

            }
         }
         if(theGoal==null) throw new Exception("Didn't find the goal");
            //search list of courses, then for each course, search offerings
      List<CourseOffering> courseOfferingsThatMeetGoal = new List<CourseOffering>();

      foreach(CourseOffering c in theOfferings) {
     if(c.Semester.Equals(semester) 
        && theGoal.Courses.Contains(c.TheCourse) ) 
     {
        courseOfferingsThatMeetGoal.Add(c);
     }
 
      }
      return courseOfferingsThatMeetGoal;
        }

        
        //Add more service functions here, as needed, for the project

        /* As a student, I want to see all available courses so that I know what my options are */
        public List<Course> GetAllCourses() {
            List<Course> theCourses = _repo.GetAllCourses().ToList<Course>();
            return theCourses;
        }

        /* As a student, I want to see all course offerings by semester, so that I can choose from what's
           available to register for next semester */
        public List<CourseOffering> getCourseOfferingsBySemester(String semester) {
            List<CourseOffering> offeringsInSemester = new List<CourseOffering>();
            foreach(CourseOffering o in _repo.Offerings) {
                if(o.Semester.Equals(semester)) {
                    offeringsInSemester.Add(o);
                }
            }
            return offeringsInSemester;
        }

        /* As a student I want to see all course offerings by semester and department so that I can 
        choose major courses to register for */
        public List<CourseOffering> getCourseOfferingsBySemesterAndDept(String semester, String dept) {
            List<CourseOffering> offerings = new List<CourseOffering>();
            foreach(CourseOffering o in _repo.Offerings) {
                if(o.Semester.Equals(semester)
                    && o.TheCourse.Department.Equals(dept, StringComparison.OrdinalIgnoreCase)) {
                    offerings.Add(o);
                }
            }
            return offerings;
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
            List<Course> result = new List<Course>();
            foreach(Course c in _repo.Courses) {
                if(c.Department.Equals(dept, StringComparison.OrdinalIgnoreCase)) result.Add(c);
            }
            result.Sort();
            return result;
        }

        public bool addCourse(Course course) {
            if(GetCourseByName(course.Name) != null) return false;
            return _repo.InsertCourse(course) != null;
        }

        public bool updateCourse(String name, Course updated) {
            Course existing = GetCourseByName(name);
            if(existing == null) return false;
            //modify in place (not replace) so goals/offerings still point at the same object
            existing.Title = updated.Title;
            existing.Credits = updated.Credits;
            existing.Description = updated.Description;
            return true;
        }

        public bool deleteCourse(String name) {
            Course existing = GetCourseByName(name);
            if(existing == null) return false;
            _repo.Courses.Remove(existing);
            _repo.Offerings.RemoveAll(o => o.TheCourse == existing);
            foreach(CoreGoal g in _repo.Goals) g.Courses.Remove(existing);
            return true;
        }

        public List<CoreGoal> getGoalsByCourseName(String name) {
            Course course = GetCourseByName(name);
            if(course == null) return null;
            List<CoreGoal> result = new List<CoreGoal>();
            foreach(CoreGoal g in _repo.Goals) {
                if(g.Courses.Contains(course)) result.Add(g);
            }
            return result;
        }

        public List<CourseOffering> getOfferingsByCourseNameAndSemester(String name, String semester) {
            Course course = GetCourseByName(name);
            if(course == null) return null;
            List<CourseOffering> result = new List<CourseOffering>();
            foreach(CourseOffering o in _repo.Offerings) {
                if(o.TheCourse == course && o.Semester.Equals(semester, StringComparison.OrdinalIgnoreCase)) result.Add(o);
            }
            return result;
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
