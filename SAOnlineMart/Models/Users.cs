using System.ComponentModel.DataAnnotations;

namespace SAOnlineMart.Models
{
    public class Users
    {
        [Key]
        public int userID { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string password { get; set; }
    }
}
