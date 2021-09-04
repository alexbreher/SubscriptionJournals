using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Models
{
    public class UsersViewModel
    {
        [Key]
        public int user_Id { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Required Alias Field")]
        [DisplayName("User Alias")]
        [Column(TypeName = "nvarchar(50)")]
        public string user { get; set; }

        [MaxLength(50)]
        [Required(ErrorMessage = "Required Name Field")]
        [DisplayName("User Name")]
        [Column(TypeName = "nvarchar(50)")]
        public string name { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Required Password Field")]
        [DisplayName("Password")]
        [Column(TypeName = "nvarchar(100)")]
        public string Password { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Required Last Name Field")]
        [DisplayName("Last name")]
        [Column(TypeName = "nvarchar(80)")]
        public string lastName { get; set; }

        [MaxLength(80)]
        [Required(ErrorMessage = "Required Email Address Field")]
        [DisplayName("E-mail")]
        [Column(TypeName = "nvarchar(80)")]
        public string email { get; set; }

        [MaxLength(80)]
        [Column(TypeName = "nvarchar(80)")]
        public string token { get; set; }
        [DisplayName("User Since")]

        [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime creationDate { get; set; }
        [DisplayName("Followers")]
        public int followers { get; set; }
        [DisplayName("Following")]
        public int following { get; set; }
        [DisplayName("Published Journals")]
        public int journalsPublished { get; set; }
        [NotMapped]
        [MaxLength(100)]
        [DisplayName("New Password")]
        public string NewPassword { get; set; }
        public List<JournalsViewModel> Journals { get; set; }
        public List<SubscriptionsViewModel> Subscriptions { get; set; }
    }
}
