using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace PeerReviewApp.Models
{
    public class Term
    {
        [Key]
        public int TermId { get; set; }
        public string TermName { get; set; }
        public int? InstitutionId { get; set; }
        public DateTime TermBeginDate { get; set; }
        public DateTime TermEndDate { get; set; }

        public virtual ICollection<Class> Classes { get; set; }
    }
}
