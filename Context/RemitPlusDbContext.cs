using Microsoft.EntityFrameworkCore;
using Remitplus_Authentication.Model;

namespace Remitplus_Authentication.Context
{
    public class RemitPlusDbContext : DbContext
    {
        public RemitPlusDbContext(DbContextOptions<RemitPlusDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ApplicationUserApiKeys> ApplicationUserApiKeys { get; set; }
        public DbSet<ApplicationUserIpWhitelist>  IpWhitelists { get; set; }
        public DbSet<IPBlackList> IPBlackLists { get; set; }
        public DbSet<WhitelistedIpLog> WhitelistedIpLog { get; set; }
    }

}
