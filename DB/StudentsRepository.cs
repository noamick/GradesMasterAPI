using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;
using GradesMasterAPI.DB.DbModels;
using System.Security.Cryptography;
using System.Data.SqlClient;

namespace GradesMasterAPI.DB
{
    public class StudentsRepository
    {
        private readonly string _connectionString;
        public StudentsRepository()
        {
            _connectionString = "Data Source = (localdb)\\MSSQLLocalDB; Initial Catalog = GradeMasterDb; Integrated Security = True; Connect Timeout = 30"; 
        }

        public StudentsRepository(string connctionString)
        {
            this._connectionString = connctionString;
        }

        public List<Student> GetAllStudents()
        {
            List<Student> students = new List<Student>();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                conn.Open();

                var command = new SqlCommand("SELECT * FROM Student", conn);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        students.Add(new Student
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            DateBirth = reader.GetDateTime(3),
                            Gender = reader.GetString(4),
                            PhoneNumber = reader.GetString(5),
                            Adress = reader.GetString(6),
                            Email = reader.GetString(7),
                            EnrollmentDate = reader.GetDateTime(8),
                        });
                    }
                }

            }

            return students;
        
        }

        public string InsertStudent(Student student)
        {
            string errors = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "INSERT INTO Student (FirstName ,LastName,DateBirth,Gender,PhoneNumber,Adress,Email,EnrollmentDate) " +
                                   "VALUES (@FirstName ,@LastName,@DateBirth,@Gender,@PhoneNumber,@Adress,@Email,@EnrollmentDate)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {

                        cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", student.LastName);
                        cmd.Parameters.AddWithValue("@DateBirth", student.DateBirth);
                        cmd.Parameters.AddWithValue("@Gender", student.Gender);
                        cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Adress", student.Adress);
                        cmd.Parameters.AddWithValue("@Email", student.Email);
                        cmd.Parameters.AddWithValue("@EnrollmentDate", student.EnrollmentDate);

                        conn.Open();
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            errors = "Insert Not Commited";
                        }

                    }

                }
                return errors;

            }
            catch (Exception ex)
            {
                errors ="Exception:"+ex.Message;
                return errors;
            }
        }

        public string UpdateStudent(int studentId, Student student)
        {
            string errors = "";
            try
            {
                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    string query = "UPDATE Student SET FirstName = @FirstName, LastName = @LastName, DateBirth = @DateBirth, " +
                                   "Gender = @Gender, PhoneNumber = @PhoneNumber, Adress = @Adress, Email = @Email, " +
                                   "EnrollmentDate = @EnrollmentDate WHERE ID = @ID";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@FirstName", student.FirstName);
                        cmd.Parameters.AddWithValue("@LastName", student.LastName);
                        cmd.Parameters.AddWithValue("@DateBirth", student.DateBirth);
                        cmd.Parameters.AddWithValue("@Gender", student.Gender);
                        cmd.Parameters.AddWithValue("@PhoneNumber", student.PhoneNumber);
                        cmd.Parameters.AddWithValue("@Adress", student.Adress);  // Corrected spelling
                        cmd.Parameters.AddWithValue("@Email", student.Email);
                        cmd.Parameters.AddWithValue("@EnrollmentDate", student.EnrollmentDate);
                        cmd.Parameters.AddWithValue("@ID",student.ID );  // Using the studentId parameter

                        conn.Open();
                        int affectedRows = cmd.ExecuteNonQuery();

                        if (affectedRows == 0)
                        {
                            errors = "Update Not Commited";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors = ex.Message;
            }

            return errors;
        }

        public Student? GetStudent(int id)
        {
            Student student = null;
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {

                conn.Open();

                var command = new SqlCommand("SELECT * FROM Student WHERE ID = @id", conn);
                command.Parameters.AddWithValue("@ID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {

                        student = new Student()
                        {
                            ID = reader.GetInt32(0),
                            FirstName = reader.GetString(1),
                            LastName = reader.GetString(2),
                            DateBirth = reader.GetDateTime(3),
                            Gender = reader.GetString(4),
                            PhoneNumber = reader.GetString(5),
                            Adress = reader.GetString(6),
                            Email = reader.GetString(7),
                            EnrollmentDate = reader.GetDateTime(8),
                        };
                    }
                    
                }

            }

            return student;

        }


    }
}
