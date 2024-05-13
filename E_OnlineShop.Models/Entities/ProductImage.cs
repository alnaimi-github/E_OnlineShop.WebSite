using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_OnlineShop.Models.Entities
{
    public class ProductImage
    {

        public int Id { get; set; }
        [Required]
        public string ImageUrl { get; set; } = string.Empty;
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product? Product { get; set; }
    }
}
