using System;
using System.Collections.Generic;

namespace DataCaptureExpertsWebAPI.Models
{
    public partial class Supplier
    {
        public Supplier()
        {
            Products = new HashSet<Product>();
        }

        public Guid SupplierId { get; set; }
        public string SupplierName { get; set; } = null!;
        public DateTime CreatedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
