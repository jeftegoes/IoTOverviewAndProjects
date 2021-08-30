using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ordering.Models
{
    public class Order
    {
        [Key]
        public Guid OrderId { get; set; }
        public string PictureUrl { get; set; }
        public byte[] ImageData { get; set; }
        public string UserEmail { get; set; }
        public Status Status { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}