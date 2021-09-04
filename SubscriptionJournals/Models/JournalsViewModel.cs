using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Models
{
    public class JournalsViewModel
    {
        [Key]
        public int journal_Id { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Required name field")]
        [Column(TypeName = "nvarchar(100)")]
        [DisplayName("Journal name")]
        public string name { get; set; }
        public int author { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime creationDate { get; set; }
        public string path { get; set; }
        [Required(ErrorMessage = "Required pdf file")]
        [NotMapped]
        public IFormFile pdf { get; set; }

        [ForeignKey("author")]
        public UsersViewModel Users { get; set; }
    }
}
