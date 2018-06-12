using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace SpaceMe.Models
{
    public class Post
    {
        public int Id { get; set; }

        [Display(Name = "Main image")]
        public byte[] Image { get; set; }

        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [AllowHtml]
        public string Content { get; set; }

        [Required(ErrorMessage = "Date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Posted on")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PostedOn { get; set; } 
    }
}