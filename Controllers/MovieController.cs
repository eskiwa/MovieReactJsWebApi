using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entity;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly MovieDbContext _context;
        public MovieController(MovieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                var movieCount = _context.Movies.Count();
                var movieList = _context.Movies.Include(x => x.Persons).Skip(pageIndex * pageSize).Take(pageSize).ToList();


                response.Status = true;
                response.Message = "Success";
                response.Data = new { Movies = movieList, Count = movieCount };
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong!";

                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetMovieById(int id)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                var movie = _context.Movies.Include(x => x.Persons).Where(x => x.Id == id).FirstOrDefault();
                if (movie == null)
                {
                    response.Status = false;
                    response.Message = "Record not exist!";
                    return BadRequest(response);
                }

                response.Status = true;
                response.Message = "Success";
                response.Data = movie;
                return Ok(response);
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong!";

                return BadRequest(response);
            }
        }

        [HttpPost]
        public IActionResult Post(CreateMovieViewModel model)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                var actors = _context.Persons.Where(x => model.Persons.Contains(x.Id)).ToList();
                if (actors.Count != model.Persons.Count)
                {
                    response.Status = false;
                    response.Message = "Invalid Actor assigned";
                    return BadRequest(response);
                }

                var postedModel = new Movie()
                {
                    Title = model.Title,
                    ReleaseDate = model.ReleaseDate,
                    Language = model.Language,
                    CoverImage = model.CoverImage,
                    Description = model.Description,
                    Persons = actors
                };

                _context.Movies.Add(postedModel);
                _context.SaveChanges();

                response.Status = true;
                response.Message = "Success";
                response.Data = postedModel;

            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong!";

                return BadRequest(response);
            }
        }
    }
}