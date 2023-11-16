using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace xkelenton.Models
{
    public class FeedbackViewModels
    {
        //https://stackoverflow.com/questions/27465286/creating-simple-viewmodel-to-show-two-tables-of-data-in-one-view-in-mvc-5
        public List<Feedback> Feedbacks { get; set; }
        public double AverageRatingScore { get; set; }
    }
}