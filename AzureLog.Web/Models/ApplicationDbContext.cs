using System.Data.Entity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace AzureLog.Web.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<StorageAccount> StorageAccounts { get; set; }
    }
}