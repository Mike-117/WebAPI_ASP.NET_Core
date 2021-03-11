using CinemaAPI.Data;
using CinemaAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private CinemaDbContext _dbContext;

        public ReservationsController(CinemaDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET api/<ReservationsController>
        [Authorize]
        [HttpPost]
        public IActionResult Post([FromBody] Reservation reservationObject)
        {
            reservationObject.ReservationTime = DateTime.Now; // this sets the value of ReservationTime automatically
            _dbContext.Reservations.Add(reservationObject);
            _dbContext.SaveChanges();
            return StatusCode(StatusCodes.Status201Created);
        }

        // GET api/<ReservationsController>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult GetReservations()
        {
            var reservations = from reservation in _dbContext.Reservations
                                   // these 2 joins assign the foreign keys to the reservation table
                               join user in _dbContext.Users on reservation.UserId equals user.Id
                               join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                               select new
                               {
                                   Id = reservation.Id,
                                   ReservationTime = reservation.ReservationTime,
                                   UserName = user.Name,
                                   MovieName = movie.Name,
                               };

            return Ok(reservations);
        }

        // GET api/<ReservationsController>/1
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public IActionResult GetReservationDetail(int id)
        {
            var reservationResult = (from reservation in _dbContext.Reservations
                                         // these 2 joins assign the foreign keys to the reservation table
                                     join user in _dbContext.Users on reservation.UserId equals user.Id
                                     join movie in _dbContext.Movies on reservation.MovieId equals movie.Id
                                     where reservation.Id == id // this limits return to that record whose id was passed
                                     select new
                                     {
                                         Id = reservation.Id,
                                         ReservationTime = reservation.ReservationTime,
                                         UserName = user.Name,
                                         Phone = reservation.Phone,
                                         MovieName = movie.Name,
                                         Qty = reservation.Qty,
                                         Price = reservation.Price,
                                         PlayingDate = movie.PlayingDate,
                                         PlayingTime = movie.PlayingTime
                                     }).FirstOrDefault(); // this will limit the result to a single record

            return Ok(reservationResult);
        }

        // DELETE api/<MoviesController>/5
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = _dbContext.Reservations.Find(id);
            if (reservation == null)
            {
                return NotFound("The record you seek does not exist");
            }
            else
            {
                _dbContext.Reservations.Remove(reservation);
                _dbContext.SaveChanges();
                return Ok("Record successfully deleted");
            }

        }
    }
}
