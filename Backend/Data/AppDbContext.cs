using Backend.Model;
using Backend.Models;  // ← namespace صحيح
using Microsoft.EntityFrameworkCore;

namespace Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<utilisateur> utilisateur { get; set; }
        public DbSet<reservation> Reservations { get; set; }
        public DbSet<Circuit> Circuits { get; set; }  // ← الآن صحيح
    }
}