using System.ComponentModel.DataAnnotations;

namespace SAOnlineMart.Models
{
    public class Orders
    {
        [Key]
        public int orderID { get; set; }

        public double orderTotal { get; set; }
        public string orderAddress { get; set; }
        public int userID { get; set; }
        public DateTime orderDate { get; set; } = DateTime.Now;
    }
}
