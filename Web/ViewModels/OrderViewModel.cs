using System;
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
        public string ImageUrl { get; set; }
        public string StatusString { get; set; }
        public byte[] ImageData { get; set; }
    }
}