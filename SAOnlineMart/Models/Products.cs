using System.ComponentModel.DataAnnotations;

namespace SAOnlineMart.Models
{
    public class Products
    {
        [Key]
        public int productID { get; set; }

        public string productName { get; set; }
        public string productDescription { get; set; }
        public decimal productPrice { get; set; }
        public string productStatus { get; set; }
        public string productImage { get; set; }
        public string productCreator { get; set; }
    }
}
