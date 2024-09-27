using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MovieAPIDemo.Data;
using MovieAPIDemo.Entity;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PersonController : ControllerBase
    {
        private readonly MovieDbContext _context;
        public PersonController(MovieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                var personCount = _context.Persons.Count();
                var movieList = _context.Persons.Skip(pageIndex * pageSize).Take(pageSize).Select(x => new ActorViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateOfBirth = x.DateOfBirth
                }).ToList();


                response.Status = true;
                response.Message = "Success";
                response.Data = new { Person = movieList, Count = personCount };
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
        public IActionResult GetPersonById(int id)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                var person = _context.Persons.Where(x => x.Id == id).FirstOrDefault();
                if (person == null)
                {
                    response.Status = false;
                    response.Message = "Record not exist!";
                    return BadRequest(response);
                }

                var personData = new ActorDetailsViewModel
                {
                    Id = person.Id,
                    DateOfBirth = person.DateOfBirth,
                    Name = person.Name,
                    Movie = _context.Movies.Where(x => x.Persons.Contains(person)).Select(x => x.Title).ToArray()
                };

                response.Status = true;
                response.Message = "Success";
                response.Data = personData;
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
        public IActionResult Post(ActorViewModel model)
        {
            BaseResponseEntity response = new BaseResponseEntity();

            try
            {
                if (ModelState.IsValid)
                {


                    var postedModel = new Person()
                    {
                        Name = model.Name,
                        DateOfBirth = model.DateOfBirth

                    };

                    _context.Persons.Add(postedModel);
                    _context.SaveChanges();

                    model.Id = postedModel.Id;

                    response.Status = true;
                    response.Message = "Success";
                    response.Data = model;
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