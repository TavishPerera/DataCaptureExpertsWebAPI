using System;
using System.Collections.Generic;

namespace DataCaptureExpertsWebAPI.Models
{
    public partial class Product
    {
        public Product()
        {
            Orders = new HashSet<Orders>();
        }

        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal UnitPrice { get; set; }
        public Guid SupplierId { get; set; }
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual Supplier Supplier { get; set; } = null!;
        public virtual ICollection<Orders> Orders { get; set; }
    }
}
