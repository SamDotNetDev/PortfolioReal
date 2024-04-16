using Microsoft.EntityFrameworkCore;
using Portfolio.Models;

namespace Portfolio.Data
{
    public class PortfolioDbContext : DbContext
    {
        public PortfolioDbContext(DbContextOptions<PortfolioDbContext> options)
            :base(options) { }

        public DbSet<CV> CvLink {  get; set; }
        public DbSet<Informations> InfoAboutMe { get; set; }
        public DbSet<RecentWorks> Works { get; set; } 
    }
}
