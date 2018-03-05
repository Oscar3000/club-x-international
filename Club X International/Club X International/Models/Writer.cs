using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Club_X_International.Models
{
    public class Writer
    {
        public int WriterID { get; set; }

        [Required, StringLength(300), Index(IsUnique = true)]
        public string Name { get; set; }

        [Display(Name="Picture")]
        public string WriterPic { get; set; }

        public string Description { get; set; }

        public ICollection<Blog> Blogs { get; set; }
    }
}