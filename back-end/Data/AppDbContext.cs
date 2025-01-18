using back_end.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace back_end.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<ManualModel> Manuals { get; set; }
        public DbSet<UserModel> Users { get; set; }
    }
}
