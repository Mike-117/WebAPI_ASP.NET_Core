using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Models
{
    public class Movie
    {
        //public int Id { get; set; }
        //[Required(ErrorMessage ="Name cannot be null or empty")] // this attribute prevents the client to pass a null value into the db
        //public string Name { get; set; }
        //[Required(ErrorMessage = "Language cannot be null or empty")]
        //public string Language { get; set; }
        //[Required(ErrorMessage = "Rating cannot be null or empty")]
        //public double Rating { get; set; }
        //[NotMapped]// the IFormFile type is not a valid SQL type, therefore we will not store the images inside the database; they will be stored in wwwroot; the db will store the image name in the db
        //public IFormFile Image { get; set; }
        //public string ImageUrl { get; set; } // This sets the image address such that it can be referenced from the database

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Language { get; set; }
        public string Duration { get; set; }
        public DateTime PlayingDate { get; set; }
        public DateTime PlayingTime { get; set; }
        public double TicketPrice { get; set; }
        public double Rating { get; set; }
        public string Genre { get; set; }
        public string TrailerUrl { get; set; }
        public string ImageUrl { get; set; }
        
        [NotMapped]
        public IFormFile Image { get; set; }

        public ICollection<Reservation> Reservations { get; set; } // this creates a one-to-many relationship btwn the user table and the reservation talbe in the db
    }
}
