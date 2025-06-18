using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject
{
    public class Discount
    {
        public int DiscountID { get; set; }
        public int ProductID { get; set; }
        public string DiscountName {  get; set; }
        public int DiscountPercent { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EndAt { get; set;}
        public virtual Product Product { get; set; }
    }
}
