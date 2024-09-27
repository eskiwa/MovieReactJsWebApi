using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MovieAPIDemo.Models;

namespace MovieAPIDemo.Entity
{
    public class MovieListViewModel
    {

        public int Id { get; set; }
        public string Title { get; set; }
        //List of actors    
        public List<ActorViewModel> Persons { get; set; }
        public string Language { get; set; }
        public DateTime ReleaseDate { get; set; }
        public string CoverImage { get; set; }
    }
}