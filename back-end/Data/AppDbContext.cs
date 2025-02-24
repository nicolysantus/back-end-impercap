using back_end.Models;
using Microsoft.EntityFrameworkCore;

namespace back_end.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ManualModel> Manuals { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<LoginModel> logins { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LoginModel>()
                .HasNoKey(); 
        }
    }
}