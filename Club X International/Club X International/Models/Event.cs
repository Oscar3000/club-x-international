using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Club_X_International.Models
{
    public class Event
    {
        public int EventID { get; set; }

        [Required]
        public string Title { get; set; }

        [Required, DisplayFormat(DataFormatString = "{0:MMMM dd, yyyy}", ApplyFormatInEditMode = true)]
        public DateTime EventTime { get; set; }

        public string EventPicture { get; set; }

        public string EventDescription { get; set; }
    }
}