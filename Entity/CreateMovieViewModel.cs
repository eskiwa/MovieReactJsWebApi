using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Entity
{
    public class CreateMovieViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //List of actors
        public List<int> Persons { get; set; }
        public string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImage { get; set; }
    }
}