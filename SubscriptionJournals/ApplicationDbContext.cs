using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SubscriptionJournals.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SubscriptionJournals
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<UsersViewModel> Users { get; set; }
        public DbSet<JournalsViewModel> Journals { get; set; }
        public DbSet<SubscriptionsViewModel> Subscriptions { get; set; }
    }
}
