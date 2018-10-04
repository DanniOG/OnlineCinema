using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using StreamingService.Models;

namespace StreamingService.Controllers
{

    public class MovieController : ApiController
    {
        // GET: api/Movie/Get
        public IEnumerable<MovieData> Get()
        {
            List<MovieData> moviesList = new List<MovieData>();

            using (MoviesDB db = new MoviesDB())
            {
                List<Movies> moviesFromDb = db.Movies.ToList();
                List<Genres> genres = db.Genres1.ToList();
                List<StreamSources> streamSources = db.StreamSources1.ToList();
                foreach (var item in moviesFromDb)
                {
                    MovieData movie = new MovieData();
                    movie.Id = item.Id;
                    movie.Title = item.Title;
                    movie.Cast = item.Cast;
                    movie.Country = item.Country;
                    movie.CoverPath = item.CoverPath;
                    movie.Dabing = item.Dabing;
                    movie.Description = item.Description;
                    movie.Director = item.Director;
                    movie.Lenght = item.Lenght;
                    movie.OriginalTitle = item.OriginalTitle != null ? item.OriginalTitle : item.Title;
                    movie.ReleaseDate = item.ReleaseDate;
                    foreach (var genre in genres.Where(n => n.MovieId == item.Id))
                    {
                        movie.Genres.Add(genre.GenreType);
                    }
                    foreach (var source in streamSources.Where(n => n.MovieId == item.Id))
                    {
                        movie.StreamSources.Add(source.Source);
                    }
                    moviesList.Add(movie);
                }
            }
            return moviesList;
        }

        // GET: api/Movie/Get/5
        public MovieData Get(Guid id)
        {
            MovieData movie = new MovieData();
            using (MoviesDB db = new MoviesDB())
            {
                Movies movieFromDb = db.Movies.Where(x => x.Id == id).FirstOrDefault();
                List<Genres> genres = db.Genres1.Where(x => x.MovieId == id).ToList();
                List<StreamSources> streamSources = db.StreamSources1.Where(x => x.MovieId == id).ToList();
                movie.Id = movieFromDb.Id;
                movie.Title = movieFromDb.Title;
                movie.Cast = movieFromDb.Cast;
                movie.Country = movieFromDb.Country;
                movie.CoverPath = movieFromDb.CoverPath;
                movie.Dabing = movieFromDb.Dabing;
                movie.Description = movieFromDb.Description;
                movie.Director = movieFromDb.Director;
                movie.Lenght = movieFromDb.Lenght;
                movie.OriginalTitle = movieFromDb.OriginalTitle != null ? movieFromDb.OriginalTitle : movieFromDb.Title;
                movie.ReleaseDate = movieFromDb.ReleaseDate;
                foreach (var genre in genres.Where(n => n.MovieId == movieFromDb.Id))
                {
                    movie.Genres.Add(genre.GenreType);
                }
                foreach (var source in streamSources.Where(n => n.MovieId == movieFromDb.Id))
                {
                    movie.StreamSources.Add(source.Source);
                }
            }
            
            return movie;
        }

        // POST: api/Movie
        public void Post([FromBody]string value)
        {
        }

        // PUT: api/Movie/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/Movie/5
        public void Delete(int id)
        {
        }
    }
}
