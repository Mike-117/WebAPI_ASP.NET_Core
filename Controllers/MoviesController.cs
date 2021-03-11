using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private CinemaDbContext _dbContext; // create an instance of CinemaDbContext, because this is the class that interacts with the db behind the scenes

        public MoviesController(CinemaDbContext dbContext) // controller contructor passes dbContext as a parameter
        {
            _dbContext = dbContext; // this allows the MoviesController to access everything in the CinemaDbContext
        }

        // GET: api/<MoviesController>
        //[HttpGet]
        //public IEnumerable<Movie> Get() // This Get returns all the items in the database
        //{
        //    return _dbContext.Movies; // This calls the Movies property which is present in the DbContext class, which returns the list of Movies in the db
        //}
        
        [Authorize]
        [HttpGet("[action]")] // we have to add [action] because the method doesn't start with Get
        public IActionResult AllMovies(string sort, int? pageNumber, int? pageSize) // IActionResult replaces the IEnumerable in order to give the HTTP status code as the result of Get; AllMovies() is just a Get method
        {
            var currentPageNumber = pageNumber ?? 1;
            var currentPageSize = pageSize ?? 5;
            var movies = from movie in _dbContext.Movies // we only select a few desired properties to be returned
            select new
            {
                Id = movie.Id,
                Name = movie.Name,
                Language = movie.Language,
                Duration = movie.Duration,
                Rating = movie.Rating,
                Genre = movie.Genre,
                ImageUrl = movie.ImageUrl
            };

            switch (sort) // This will change the output order at the discretion of the client
            {
                case "desc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderByDescending(m => m.Rating));// The skip and take algorithm is employed to limit the number of items returned per page
                case "asc":
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize).OrderBy(m => m.Rating));
                default:
                    return Ok(movies.Skip((currentPageNumber - 1) * currentPageSize).Take(currentPageSize)); 
            }

        }

        // GET api/<MoviesController>/1
        [Authorize]
        [HttpGet("[action]/{id}")]
        // public Movie Get(int id) // This Get returns just one item with a specific id
        public IActionResult MovieDetail (int id) //IActionResult replaces movie for HTTP code
        {
            var movie = _dbContext.Movies.Find(id); // defines a variable as the specific movie from the db
            // This accounts for changes to a record whose id is not in the db
            if (movie == null)
            {
                return NotFound("The record you seek does not exist");
            }
            return Ok(movie);
        }
        
        [Authorize]
        [HttpGet("[action]")]
        public IActionResult FindMovies(string movieName)
        {
            var movies = from movie in _dbContext.Movies
                         where movie.Name.StartsWith(movieName) // This checks whether the movies starts with the keyword which is passed, thus providing basic search functionality
                         select new
                         {
                             Id = movie.Id,
                             Name = movie.Name,
                             ImageUrl = movie.ImageUrl
                         };
            return Ok(movies);

        }

        // // POST api/<MoviesController>
        // [HttpPost]
        //// public void Post([FromBody] Movie movieObject)
        // public IActionResult Post([FromBody] Movie movieObject) // IActionResult replaces void in order to give an HTTP status code 
        // {
        //     _dbContext.Movies.Add(movieObject);
        //     _dbContext.SaveChanges();  // once added, this tells the db to save the changes
        //     return StatusCode(StatusCodes.Status201Created); // When the record has been created successfully, the server returns the 201 status code
        // }

        // New Post method includes the ability to post images along with the other movie data
        [Authorize(Roles ="Admin")] // only the admin will be able to access Post
        [HttpPost]
        public IActionResult Post([FromForm] Movie movieObject) // FromForm replaces FromBody because the added image requires more than JSON formatting
        {
            var guid = Guid.NewGuid(); // This assigns each new image a unique name, so as to avoid problems handling multiple images with the same name
            var filePath = Path.Combine("wwwroot", guid + ".jpg"); //the Path.Combine method turns strings into a filepath
            if (movieObject.Image != null)
            {
                var fileStream = new FileStream(filePath, FileMode.Create); // FileStream class is used to manage files
                movieObject.Image.CopyTo(fileStream);  // This copies the content of the uploaded file to the targeted stream
            }
            // this makes the newly created filePath the ImageUrl property in order to have a reference in the db
            movieObject.ImageUrl = filePath.Remove(0, 7); // Remove(0,7) excludes "wwwroot" from ImageUrl, so that a future Get request will return the image

            _dbContext.Movies.Add(movieObject);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);

        }

        //// PUT api/<MoviesController>/5
        //[HttpPut("{id}")]
        //public IActionResult Put(int id, [FromBody] Movie movieObject) // IActionResult replaces void
        //{
        //    var movie = _dbContext.Movies.Find(id);
        //    // This accounts for changes to a record whose id is not in the db
        //    if (movie == null) {
        //        return NotFound("The record you seek does not exist");
        //    }
        //    else
        //    {
        //        movie.Name = movieObject.Name;     // We have to account for each property's changes
        //        movie.Language = movieObject.Language;
        //        movie.Rating = movieObject.Rating;
        //        _dbContext.SaveChanges();
        //        return Ok("Record Updated Successfully");  // This message lets you know about the success of the operation
        //    }
        //}

        // New Put method includes the ability to update images along with the other movie data
        [Authorize(Roles ="Admin")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromForm] Movie movieObject)
        {
            var movie = _dbContext.Movies.Find(id);
            if (movie == null)
            {
                return NotFound("The record you seek does not exist");
            }
            else
            {
                var guid = Guid.NewGuid();
                var filePath = Path.Combine("wwwroot", guid + ".jpg");
                if (movieObject.Image != null)
                {
                    var fileStream = new FileStream(filePath, FileMode.Create);
                    movieObject.Image.CopyTo(fileStream);
                    movie.ImageUrl = filePath.Remove(0, 7); // this line is added so that if the image we pass is not null we will add a new image; we use movie instead of movieObject b/c we want to update the property that is already in the database
                }

                movie.Name = movieObject.Name;
                movie.Description = movieObject.Description;
                movie.Language = movieObject.Language;
                movie.Duration = movieObject.Duration;
                movie.PlayingDate = movieObject.PlayingDate;
                movie.PlayingTime = movieObject.PlayingTime;
                movie.TicketPrice = movieObject.TicketPrice;
                movie.Rating = movieObject.Rating;
                movie.Genre = movieObject.Genre;
                movie.TrailerUrl = movieObject.TrailerUrl;
                               
                _dbContext.SaveChanges();
                return Ok("Record Updated Successfully");
            }
        }

        // DELETE api/<MoviesController>/5
        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var movie = _dbContext.Movies.Find(id);
            // This accounts for changes to a record whose id is not in the db
            if (movie == null)
            {
                return NotFound("The record you seek does not exist");
            }
            else
            {
                _dbContext.Movies.Remove(movie);
                _dbContext.SaveChanges();
                return Ok("Record successfully deleted");
            }

        }
    }
}
