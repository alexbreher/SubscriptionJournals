using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals.Models
{
    public class LogInViewModel
    {
        [MaxLength(80)]
        [Required(ErrorMessage = "Required Email Address Field")]
        [DisplayName("E-mail")]
        [Column(TypeName = "nvarchar(80)")]
        public string email { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Required Password Field")]
        [DisplayName("Password")]
        [Column(TypeName = "nvarchar(100)")]
        public string Password { get; set; }
        [DisplayName("Remember Me")]
        public bool RememberMe { get; set; }
    }
}
