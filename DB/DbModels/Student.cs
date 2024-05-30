using System.ComponentModel.DataAnnotations;

namespace GradesMasterAPI.DB.DbModels
{
    public class Student
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string  LastName { get; set; }
        public DateTime DateBirth { get; set; }
        public string Gender { get; set; }
        public string PhoneNumber {  get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public DateTime EnrollmentDate { get; set; }

    }
}
