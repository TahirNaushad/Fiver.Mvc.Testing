using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Fiver.Mvc.Testing.EF
{
    public class MovieRepository : IMovieRepository
    {
        private readonly Database context;

        public MovieRepository(Database context)
        {
            this.context = context;
        }

        public List<MovieEntity> GetList()
        {
            return this.context.Movies.ToList();
        }

        public MovieEntity GetItem(int id)
        {
            return this.context.Movies.Where(m => m.Id == id).FirstOrDefault();
        }

        public void Insert(MovieEntity entity)
        {
            this.context.Movies.Add(entity);
            this.context.SaveChanges();
        }

        public void Update(MovieEntity entity)
        {
            this.context.Movies.Update(entity);
            this.context.SaveChanges();
        }

        public void Delete(int id)
        {
            var entity = this.context.Movies.Where(m => m.Id == id).FirstOrDefault();
            this.context.Movies.Remove(entity);
            this.context.SaveChanges();
        }
    }
}
