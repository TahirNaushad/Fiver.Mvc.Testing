using System.Collections.Generic;

namespace Fiver.Mvc.Testing.EF
{
    public interface IMovieRepository
    {
        void Delete(int id);
        MovieEntity GetItem(int id);
        List<MovieEntity> GetList();
        void Insert(MovieEntity entity);
        void Update(MovieEntity entity);
    }
}