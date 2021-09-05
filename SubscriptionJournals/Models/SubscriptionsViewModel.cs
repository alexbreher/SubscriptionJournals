using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Models
{
    public class SubscriptionsViewModel
    {
        [Key]
        public int subscription_Id { get; set; }
        public int subscriptor { get; set; }
        public int subscribesTo { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime subscriptionDate { get; set; }

        [ForeignKey("subscribesTo")]
        public UsersViewModel Users { get; set; }
        
    }
    
}
