using System.ComponentModel.DataAnnotations;

namespace SAOnlineMart.Models
{
    public class Admin
    {
        [Key]
        public int adminID { get; set; }

        public string firstName { get; set; }
        public string lastName { get; set; }
        public string position { get; set; }
        public int userID { get; set; }
    }
}
