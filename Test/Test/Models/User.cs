using SQLite;

namespace Test.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Fullname { get; set; }
        public string Password { get; set; }
        public string CollegeDepartment { get; set; } 
        public string RoomNumber { get; set; }        
        public string StudentId { get; set; }       
    }
}
