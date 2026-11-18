using System;
using System.Collections.Generic;
using CourseRegistration.Models;
using MySql.Data.MySqlClient;

namespace CourseRegistration.Repository
{
    public interface ICourseRepository
    {
        //obsolete, delete soon
        List<Course> Courses {get;set;}
        List<CoreGoal> Goals {get;set;}
        List<CourseOffering> Offerings {get;set;}
        
        //new methods
        public IEnumerable<Course> GetAllCourses();
        public Course GetCourseByName(string name);

        public Course InsertCourse(Course newCourse);
    }
    public class CourseRepository : ICourseRepository {
        public List<Course> Courses {get;set;}
        public List<CoreGoal> Goals {get;set;}
        public List<CourseOffering> Offerings {get;set;}

        private MySqlConnection _connection;


        //Add more data as needed 
       public CourseRepository()
        {
            string connectionString="server=localhost;userid=csci330user;password=csci330pass;database=courseregistration";
            _connection = new MySqlConnection(connectionString);
            _connection.Open();
        } // end constructor
        ~CourseRepository()
        {
            _connection.Close();
        }

        public IEnumerable<Course> GetAllCourses()
        {
            var statement = "Select * from Courses";
            var command = new MySqlCommand(statement,_connection);
            var results = command.ExecuteReader();

            List<Course> newList = new List<Course>(25);

            while (results.Read())
            {
                Course c = new Course
                {
                    Name = (string)results[0],
                    Title = (string)results[1],
                    Credits = (double)results[2],
                    Description = (string)results[3],
                };
                newList.Add(c);
            
            }
            results.Close();
            return newList;
        }

        public Course GetCourseByName(string name)
        {
            var statement = "Select * from Courses where Name=@newName";

            var command = new MySqlCommand(statement,_connection);
            command.Parameters.AddWithValue("@newName", name);
            var results = command.ExecuteReader();

            if(results.Read())
            {
                Course c = new Course
                {
                    Name = (string)results[0],
                    Title = (string)results[1],
                    Credits = (double)results[2],
                    Description = (string)results[3],
                }; 
                results.Close();
                return c;
            }
            results.Close();
            return null;
        }

        public Course InsertCourse(Course newCourse)
        {
            var statement = "INSERT into Courses (Name, Title,Credits,Description) values (@newName, @newTitle,@newCredits,@newDescription)";
            var command = new MySqlCommand(statement,_connection);
            command.Parameters.AddWithValue("@newName", newCourse.Name);
            command.Parameters.AddWithValue("@newTitle", newCourse.Title);
            command.Parameters.AddWithValue("@newCredits", newCourse.Credits);
            command.Parameters.AddWithValue("@newDescription", newCourse.Description);

            int result = command.ExecuteNonQuery();
            if (result == 1)
                return newCourse;
            else 
                return null;
        }

        public List<CourseOffering> getOfferingsByGoalIdAndSemester(String theGoalId, String semester) {
            CoreGoal theGoal=null;
            foreach(CoreGoal cg in Goals) {
                if(cg.Id.Equals(theGoalId)) {
                    theGoal=cg; break;

                }
            }
            if(theGoal==null) throw new Exception("Didn't find the goal");
            //search list of courses, then for each course, search offerings
            List<CourseOffering> courseOfferingsThatMeetGoal = new List<CourseOffering>();
            
            foreach(CourseOffering c in Offerings) {
                if(c.Semester.Equals(semester) 
                    && theGoal.Courses.Contains(c.TheCourse) ) 
                    {courseOfferingsThatMeetGoal.Add(c);}



            }//end for
            return courseOfferingsThatMeetGoal;
        }



    }
    
}
