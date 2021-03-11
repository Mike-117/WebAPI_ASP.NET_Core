using CinemaAPI.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Data
{
    public class CinemaDbContext : DbContext // DbContext is the class that works with the database
    {
        // the DbContext class cannot do any work without an instance of DbContextOptions class, this conveys info of what db to use, what configuration string, etc.
        
        public CinemaDbContext (DbContextOptions<CinemaDbContext> options) : base(options) // create the instance of DbContextOptions by passing it as a parameter to the CinemaDbContext contructor; CinemaDbContext is the argument for the generic parameter; options is the name of the parameter; options is passed to the base class constructor
        {
                    
        }

        public DbSet<Movie> Movies { get; set; } // At runtime, EF will map this DbSet property to the database; it will create a data table named Movies
        public DbSet<User> Users { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

    }
}
