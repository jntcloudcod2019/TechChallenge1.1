using Microsoft.EntityFrameworkCore;
using TechChallengeFIAP.Models;

namespace TechChallengeFIAP.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Contact> Contacts { get; set; }
    }
}
