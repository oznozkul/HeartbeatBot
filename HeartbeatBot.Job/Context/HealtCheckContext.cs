using Microsoft.EntityFrameworkCore;
using HeartbeatBot.Job.Models;

namespace HeartbeatBot.Job.Context
{
    public class HealtCheckContext : DbContext
    {
        public DbSet<App> Apps { get; set; } 
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=HealtCheckDb.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<App>().ToTable("Apps");
            modelBuilder.Entity<OutboxMessage>()
                .HasOne(m => m.Application)
                .WithMany()
                .HasForeignKey(m => m.ApplicationId);
        }
    }
}
