using Microsoft.EntityFrameworkCore;

namespace BeltExam.Models
{
    public class MyContext : DbContext
    {
        public MyContext(DbContextOptions options): base(options) {}

        public DbSet<User> Users {get; set;}
        public DbSet<Campaign> Campaigns {get; set;}
        public DbSet<Particpant> Particpants {get; set;}
    }
}