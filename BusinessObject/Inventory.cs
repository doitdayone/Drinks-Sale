using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Inventory
    {
        [Key, ForeignKey("Product")]
        public int ProductID {  get; set; }
        public int QuantityInStock { get; set; }
        public int Sales { get; set; }
        public virtual Product Product { get; set; }
    }
}
