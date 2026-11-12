using System;
using System.Collections.Generic;
using CourseRegistration.Models;
using CourseRegistration.Repository;

namespace CourseRegistration.Services
{
    public interface ICourseServices
    {
        public List<CourseOffering> getOfferingsByGoalIdAndSemester(String theGoalId, String semester);
        
        //Add more service functions here, as needed, for the project

        /* As a student, I want to see all available courses so that I know what my options are */
        public List<Course> getCourses();

        /* As a student, I want to see all course offerings by semester, so that I can choose from what's
           available to register for next semester */
        public List<CourseOffering> getCourseOfferingsBySemester(String semester);

        /* As a student I want to see all course offerings by semester and department so that I can
        choose major courses to register for */
        public List<CourseOffering> getCourseOfferingsBySemesterAndDept(String semester, String dept);

        /* Assignment #4 endpoints */
        public Course getCourseByName(String name);                       //null if not found
        public List<Course> getCoursesByDepartment(String dept);
        public bool addCourse(Course course);                             //false if name already exists
        public bool updateCourse(String name, Course updated);            //false if not found
        public bool deleteCourse(String name);                            //false if not found
        public List<CoreGoal> getGoalsByCourseName(String name);          //null if course not found
        public List<CourseOffering> getOfferingsByCourseNameAndSemester(String name, String semester); //null if course not found

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