using Microsoft.EntityFrameworkCore;
using TechChallengeFIAP.Models;

namespace TechChallengeFIAP.Models
{
    public class ContactDbContext : DbContext
    {
        public ContactDbContext(DbContextOptions<ContactDbContext> options)
            : base(options)
        {
        }

        //Propriedade de acesso 
        public DbSet<Contact> Contacts { get; set; }
    }
}
