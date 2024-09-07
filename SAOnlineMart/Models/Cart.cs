using System.ComponentModel.DataAnnotations;

namespace SAOnlineMart.Models
{
    public class Cart
    {
        [Key]
        public int cartID { get; set; }

        public string productName { get; set; }
        public decimal productPrice { get; set; }
        public int productQuantity { get; set; }
    }
}
