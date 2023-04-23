using System;
using System.Collections.Generic;

namespace DataCaptureExpertsWebAPI.Models
{
    public partial class Orders
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public int OrderStatus { get; set; }
        public int OrderType { get; set; }
        public Guid OrderBy { get; set; }
        public DateTime OrderedOn { get; set; }
        public DateTime? ShippedOn { get; set; }
        public bool IsActive { get; set; }

        public virtual Customer OrderByNavigation { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
}
