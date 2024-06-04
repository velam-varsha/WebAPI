using Microsoft.EntityFrameworkCore;
using WebAPIDemo.Models;

namespace WebAPIDemo.data
{
    public class ApplicationDbContext: DbContext
    {
        // ApplicationDbContext represents the in-memory database, we should let it know where the actual database is, so create a constructor
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }

        public DbSet<Shirt> Shirts { get; set; }  //the DbContext class represents a database and DbSet is generally a table, the properties in the Shirt represents different columns in the database table
        protected override void OnModelCreating(ModelBuilder modelBuilder) // this method means that when the in-memory database is being created, the developer will have a chance to configure the database 
        {
            base.OnModelCreating(modelBuilder);
            // data seeding
            modelBuilder.Entity<Shirt>().HasData(
                new Shirt { ShirtId = 1, Brand = "My Brand", Color = "Blue", Gender = "Men", Price = 30, Size = 10 },
                new Shirt { ShirtId = 2, Brand = "My Brand", Color = "Black", Gender = "Men", Price = 35, Size = 12 },
                new Shirt { ShirtId = 3, Brand = "Your Brand", Color = "Pink", Gender = "Women", Price = 28, Size = 8 },
                new Shirt { ShirtId = 4, Brand = "Your Brand", Color = "Yellow", Gender = "Women", Price = 30, Size = 9 },
                new Shirt { ShirtId = 5, Brand = "Your Brand", Color = "Yellow", Gender = "Women", Price = 30, Size = 10 }
            );
        }
    }
}
