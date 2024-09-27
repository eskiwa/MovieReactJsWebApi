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
    [ApiController]
    [Route("api/[controller]")]

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
                var movieList = _context.Movies.Include(x => x.Persons).Skip(pageIndex * pageSize).Take(pageSize).Select(x => new MovieListViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Persons = x.Persons.Select(z => new ActorViewModel
                    {
                        Id = z.Id,
                        Name = z.Name,
                        DateOfBirth = z.DateOfBirth

                    }).ToList(),
                    CoverImage = x.CoverImage,
                    Language = x.Language,
                    ReleaseDate = x.ReleaseDate
                }).ToList();


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
                var movie = _context.Movies.Include(x => x.Persons).Where(x => x.Id == id).Select(x => new MovieDetailsViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    Persons = x.Persons.Select(z => new ActorViewModel
                    {
                        Id = z.Id,
                        Name = z.Name,
                        DateOfBirth = z.DateOfBirth

                    }).ToList(),
                    CoverImage = x.CoverImage,
                    Language = x.Language,
                    ReleaseDate = x.ReleaseDate
                }).FirstOrDefault();
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
                if (ModelState.IsValid)
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

                    var responseData = new MovieDetailsViewModel
                    {
                        Id = postedModel.Id,
                        Title = postedModel.Title,
                        Description = postedModel.Description,
                        Persons = postedModel.Persons.Select(z => new ActorViewModel
                        {
                            Id = z.Id,
                            Name = z.Name,
                            DateOfBirth = z.DateOfBirth

                        }).ToList(),
                        CoverImage = postedModel.CoverImage,
                        Language = postedModel.Language,
                        ReleaseDate = postedModel.ReleaseDate
                    };
                    response.Status = true;
                    response.Message = "Success";
                    response.Data = responseData;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Validation Failed";

                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = "Something went wrong!";

                return BadRequest(response);
            }
        }


        [HttpPut]
        public IActionResult Put(CreateMovieViewModel model)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned";
                        return BadRequest(response);
                    }
                    var actors = _context.Persons.Where(x => model.Persons.Contains(x.Id)).ToList();
                    if (actors.Count != model.Persons.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actor assigned";
                        return BadRequest(response);
                    }
                    var movieDetails = _context.Movies.Include(x => x.Persons).Where(x => x.Id == model.Id).FirstOrDefault();

                    if (movieDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie Record";
                        return BadRequest(response);
                    }

                    movieDetails.Title = model.Title;
                    movieDetails.ReleaseDate = model.ReleaseDate;
                    movieDetails.Language = model.Language;
                    movieDetails.CoverImage = model.CoverImage;
                    movieDetails.Description = model.Description;

                    //find removed actors persons
                    var removedPersons = movieDetails.Persons.Where(x => !model.Persons.Contains(x.Id)).ToList();
                    foreach (var person in removedPersons)
                    {
                        movieDetails.Persons.Remove(person);
                    }
                    //find added actors person
                    var addedPersons = actors.Except(movieDetails.Persons).ToList();
                    foreach (var person in addedPersons)
                    {
                        movieDetails.Persons.Add(person);
                    }
                    _context.SaveChanges();
                    var responseData = new MovieDetailsViewModel
                    {
                        Id = movieDetails.Id,
                        Title = movieDetails.Title,
                        Description = movieDetails.Description,
                        Persons = movieDetails.Persons.Select(z => new ActorViewModel
                        {
                            Id = z.Id,
                            Name = z.Name,
                            DateOfBirth = z.DateOfBirth

                        }).ToList(),
                        CoverImage = movieDetails.CoverImage,
                        Language = movieDetails.Language,
                        ReleaseDate = movieDetails.ReleaseDate
                    };
                    response.Status = true;
                    response.Message = "Updated Successfuly";
                    response.Data = movieDetails;
                    return Ok(response);
                }
                else
                {
                    response.Status = false;
                    response.Message = "Validation Failed";

                    return BadRequest(response);
                }
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