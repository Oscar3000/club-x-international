using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Club_X_International.Models
{
    public class Blog
    {
        public int BlogID { get; set; }

        [Required]
        public string Title { get; set; }

        [Display(Name ="Picture")]
        public string BlogPicture { get; set; }

        [Display(Name ="Blog Content"),AllowHtml]
        public string BlogContent { get; set; }

        [DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}", ApplyFormatInEditMode = true), Display(Name = "Written Date")]
        public DateTime WrittenDate { get; set; }

        public int WriterID { get; set; }

        public Writer writer { get; set; }

        [Display(Name = "Writer's Name")]
        public string Name { get; set; }
    }
}