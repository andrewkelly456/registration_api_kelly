using System;
using System.Collections.Generic;
using System.Linq;
using CourseRegistration.Models;
using MySql.Data.MySqlClient;

namespace CourseRegistration.Repository
{
    public interface ICourseRepository
    {
        public IEnumerable<Course> GetAllCourses();
        public Course GetCourseByName(string name);

        public Course InsertCourse(Course newCourse);
        public Course UpdateCourseByName(string name, Course updated);   //null if not found
        public bool DeleteCourseByName(string name);                     //false if not found

        public IEnumerable<CoreGoal> GetAllCoreGoals();
        public CoreGoal GetCoreGoalById(string id);                      //null if not found, Courses empty
        public CoreGoal GetCoreGoalWithCoursesById(string id);           //null if not found
        public IEnumerable<Course> GetCoursesForCoreGoalById(string id);
        public CoreGoal InsertCoreGoal(CoreGoal newGoal);                //also inserts newGoal.Courses links
        public bool UpdateCoreGoal(string id, CoreGoal modifiedGoal);
        public bool AddCoursetoCoreGoal(string id, Course newCourse);
        public bool DeleteCoreGoal(string id);

        public IEnumerable<CoreGoal> GetCoreGoalsByCourseName(string courseName);
        public IEnumerable<CourseOffering> GetOfferingsBySemester(string semester);
    }
    public class CourseRepository : ICourseRepository {
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

        public Course UpdateCourseByName(string name, Course updated)
        {
            var statement = "UPDATE Courses SET Title=@newTitle, Credits=@newCredits, Description=@newDescription WHERE Name=@name";
            var command = new MySqlCommand(statement,_connection);
            command.Parameters.AddWithValue("@name", name);
            command.Parameters.AddWithValue("@newTitle", updated.Title);
            command.Parameters.AddWithValue("@newCredits", updated.Credits);
            command.Parameters.AddWithValue("@newDescription", updated.Description);

            int result = command.ExecuteNonQuery();
            if (result == 1)
            {
                updated.Name = name;
                return updated;
            }
            return null;
        }

        public bool DeleteCourseByName(string name)
        {
            var statement = "DELETE FROM Courses WHERE Name=@name";
            var command = new MySqlCommand(statement,_connection);
            command.Parameters.AddWithValue("@name", name);

            return command.ExecuteNonQuery() == 1;
        }

        /* ---- CoreGoals ---- */

        public IEnumerable<CoreGoal> GetAllCoreGoals()
        {
            var command = new MySqlCommand("SELECT Id, Name, Description FROM CoreGoals", _connection);
            var results = command.ExecuteReader();

            List<CoreGoal> goals = new List<CoreGoal>();
            while (results.Read())
            {
                goals.Add(new CoreGoal
                {
                    Id = (string)results[0],
                    Name = results.IsDBNull(1) ? null : (string)results[1],
                    Description = results.IsDBNull(2) ? null : (string)results[2],
                    Courses = new List<Course>()
                });
            }
            results.Close();
            return goals;
        }

        public CoreGoal GetCoreGoalById(string id)
        {
            var command = new MySqlCommand("SELECT Id, Name, Description FROM CoreGoals WHERE Id=@id", _connection);
            command.Parameters.AddWithValue("@id", id);
            var results = command.ExecuteReader();

            CoreGoal goal = null;
            if (results.Read())
            {
                goal = new CoreGoal
                {
                    Id = (string)results[0],
                    Name = results.IsDBNull(1) ? null : (string)results[1],
                    Description = results.IsDBNull(2) ? null : (string)results[2],
                    Courses = new List<Course>()
                };
            }
            results.Close();
            return goal;
        }

        public CoreGoal GetCoreGoalWithCoursesById(string id)
        {
            CoreGoal goal = GetCoreGoalById(id);
            if (goal == null) return null;
            goal.Courses = GetCoursesForCoreGoalById(id).ToList();
            return goal;
        }

        public IEnumerable<Course> GetCoursesForCoreGoalById(string id)
        {
            var statement = "SELECT c.Name, c.Title, c.Credits, c.Description FROM Courses c " +
                            "JOIN CoreGoalCourses cgc ON cgc.CourseName = c.Name WHERE cgc.GoalId=@id";
            var command = new MySqlCommand(statement, _connection);
            command.Parameters.AddWithValue("@id", id);
            var results = command.ExecuteReader();

            List<Course> courses = new List<Course>();
            while (results.Read())
            {
                courses.Add(new Course
                {
                    Name = (string)results[0],
                    Title = (string)results[1],
                    Credits = (double)results[2],
                    Description = (string)results[3],
                });
            }
            results.Close();
            return courses;
        }

        // Inserts the goal and, if newGoal.Courses is set, the CoreGoalCourses links (all or nothing)
        public CoreGoal InsertCoreGoal(CoreGoal newGoal)
        {
            using (var tx = _connection.BeginTransaction())
            {
                try
                {
                    var command = new MySqlCommand("INSERT INTO CoreGoals (Id, Name, Description) VALUES (@id, @name, @desc)", _connection, tx);
                    command.Parameters.AddWithValue("@id", newGoal.Id);
                    command.Parameters.AddWithValue("@name", newGoal.Name);
                    command.Parameters.AddWithValue("@desc", newGoal.Description);
                    if (command.ExecuteNonQuery() != 1) { tx.Rollback(); return null; }

                    if (newGoal.Courses != null)
                    {
                        foreach (Course c in newGoal.Courses)
                        {
                            var link = new MySqlCommand("INSERT INTO CoreGoalCourses (GoalId, CourseName) VALUES (@id, @course)", _connection, tx);
                            link.Parameters.AddWithValue("@id", newGoal.Id);
                            link.Parameters.AddWithValue("@course", c.Name);
                            link.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return newGoal;
                }
                catch (MySqlException)
                {
                    tx.Rollback();
                    return null;   //duplicate id, unknown course, etc.
                }
            }
        }

        public bool UpdateCoreGoal(string id, CoreGoal modifiedGoal)
        {
            var command = new MySqlCommand("UPDATE CoreGoals SET Name=@name, Description=@desc WHERE Id=@id", _connection);
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@name", modifiedGoal.Name);
            command.Parameters.AddWithValue("@desc", modifiedGoal.Description);
            return command.ExecuteNonQuery() == 1;
        }

        public bool AddCoursetoCoreGoal(string id, Course newCourse)
        {
            try
            {
                var command = new MySqlCommand("INSERT INTO CoreGoalCourses (GoalId, CourseName) VALUES (@id, @course)", _connection);
                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@course", newCourse.Name);
                return command.ExecuteNonQuery() == 1;
            }
            catch (MySqlException)
            {
                return false;   //already linked, or goal/course doesn't exist
            }
        }

        // Deletes the goal's CoreGoalCourses rows and then the goal itself
        public bool DeleteCoreGoal(string id)
        {
            using (var tx = _connection.BeginTransaction())
            {
                try
                {
                    var links = new MySqlCommand("DELETE FROM CoreGoalCourses WHERE GoalId=@id", _connection, tx);
                    links.Parameters.AddWithValue("@id", id);
                    links.ExecuteNonQuery();

                    var goal = new MySqlCommand("DELETE FROM CoreGoals WHERE Id=@id", _connection, tx);
                    goal.Parameters.AddWithValue("@id", id);
                    bool deleted = goal.ExecuteNonQuery() == 1;

                    if (deleted) tx.Commit(); else tx.Rollback();
                    return deleted;
                }
                catch (MySqlException)
                {
                    tx.Rollback();
                    return false;
                }
            }
        }

        public IEnumerable<CoreGoal> GetCoreGoalsByCourseName(string courseName)
        {
            var statement = "SELECT g.Id, g.Name, g.Description FROM CoreGoals g " +
                            "JOIN CoreGoalCourses cgc ON cgc.GoalId = g.Id WHERE cgc.CourseName=@course";
            var command = new MySqlCommand(statement, _connection);
            command.Parameters.AddWithValue("@course", courseName);
            var results = command.ExecuteReader();

            List<CoreGoal> goals = new List<CoreGoal>();
            while (results.Read())
            {
                goals.Add(new CoreGoal
                {
                    Id = (string)results[0],
                    Name = results.IsDBNull(1) ? null : (string)results[1],
                    Description = results.IsDBNull(2) ? null : (string)results[2],
                    Courses = new List<Course>()
                });
            }
            results.Close();
            return goals;
        }

        public IEnumerable<CourseOffering> GetOfferingsBySemester(string semester)
        {
            var statement = "SELECT c.Name, c.Title, c.Credits, c.Description, o.Semester, o.Section " +
                            "FROM CourseOfferings o JOIN Courses c ON o.Course = c.Name WHERE o.Semester=@semester";
            var command = new MySqlCommand(statement, _connection);
            command.Parameters.AddWithValue("@semester", semester);
            var results = command.ExecuteReader();

            List<CourseOffering> offerings = new List<CourseOffering>();
            while (results.Read())
            {
                offerings.Add(new CourseOffering
                {
                    TheCourse = new Course
                    {
                        Name = (string)results[0],
                        Title = (string)results[1],
                        Credits = (double)results[2],
                        Description = (string)results[3],
                    },
                    Semester = (string)results[4],
                    Section = (string)results[5],
                });
            }
            results.Close();
            return offerings;
        }

    }
    
}
