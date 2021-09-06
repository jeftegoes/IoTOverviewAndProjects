using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Web.ViewModels
{
    public class OrderViewModel
    {
        public Guid OrderId { get; set; }
        [Display(Name = "Email")]
        public string UserEmail { get; set; }
        [Display(Name = "Image file")]
        public IFormFile File { get; set; }
        public string PictureUrl { get; set; }
        public int Status { get; set; }
        public byte[] ImageData { get; set; }
        public string ImageString { get; set; }
        public List<OrderDetailViewModel>  OrderDetails { get; set; }
    }
}