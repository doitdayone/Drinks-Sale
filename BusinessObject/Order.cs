using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Order
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }


        public int CustomerID { get; set; }


        public DateTime CreateAt { get; set; }


        public decimal Amount { get; set; }


        public decimal Tax { get; set; }

        public virtual Customer Customer { get; set; } = null!;
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
