using CLOD.ProjectWork.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CLOD.ProjectWork.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Wallet> Wallets { get; set; } // Aggiungi questa linea per configurare Wallet come DbSet nel contesto
    }
}
