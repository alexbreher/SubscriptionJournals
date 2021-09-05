using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Models
{
    public class IndexFollowersViewModel
    {
        public int journal_Id { get; set; }
        [DisplayName("Journal Title")]
        public string name { get; set; }
        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        [DisplayName("Creation Date")]
        public DateTime creationDate { get; set; }
        [DisplayName("Author")]
        public int author { get; set; }
        public int subscriptor { get; set; }
        public int subscribesTo { get; set; }
        public int user_Id { get; set; }
        [DisplayName("Author")]
        public string user { get; set; }
        [DisplayName("Author")]
        public UsersViewModel Users { get; set; }
    }
}
