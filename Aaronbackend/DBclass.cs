using AaronBackend.Models;
using Microsoft.EntityFrameworkCore;


namespace Aaronbackend
{
    public class DBclass : DbContext
    {
        public DBclass(DbContextOptions<DBclass> options) : base(options) { }


        public DbSet<Vehicle> Vehicles { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
