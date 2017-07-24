using Fiver.Mvc.Testing.EF;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace Fiver.Mvc.Testing.Tests
{
    public class MovieRepositoryTests
    {
        [Fact(DisplayName = "GetList_returns_correct_count")]
        public void GetList_returns_correct_count()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "GetList_returns_correct_count");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);

            // Act
            var result = repo.GetList();

            // Assert
            Assert.Equal(expected: 3, actual: result.Count);
        }

        [Fact(DisplayName = "GetItem_with_invalid_Id_returns_null")]
        public void GetItem_with_invalid_Id_returns_null()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "GetItem_with_invalid_Id_returns_null");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);

            // Act
            var result = repo.GetItem(100);

            // Assert
            Assert.Null(result);
        }

        [Fact(DisplayName = "GetItem_with_valid_Id_returns_correct_item")]
        public void GetItem_with_valid_Id_returns_correct_item()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "GetItem_with_valid_Id_returns_correct_item");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);

            // Act
            var result = repo.GetItem(1);

            // Assert
            Assert.Equal(expected: 1, actual: result.Id);
            Assert.Equal(expected: "Never Say Never Again", actual: result.Title);
        }

        [Fact(DisplayName = "Insert_adds_entity")]
        public void Insert_adds_entity()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "Insert_adds_entity");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);
            var entity = new MovieEntity
            {
                Id = 4,
                Title = "A View to Kill",
                ReleaseYear = 1979,
                Summary = "Roger Moore's last movie"
            };

            // Act
            repo.Insert(entity);

            // Assert
            Assert.Equal(expected: 4, actual: context.Movies.Count());
        }

        [Fact(DisplayName = "Update_changes_entity")]
        public void Update_changes_entity()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "Update_changes_entity");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);
            var entity = repo.GetItem(2);
            entity.ReleaseYear = 2971;
            
            // Act
            repo.Update(entity);

            // Assert
            var item = repo.GetItem(2);
            Assert.Equal(expected: 2971, actual: item.ReleaseYear);
        }

        [Fact(DisplayName = "Delete_removes_entity")]
        public void Delete_removes_entity()
        {
            // Arrange
            var builder = new DbContextOptionsBuilder<Database>();
            builder.UseInMemoryDatabase(databaseName: "Delete_removes_entity");

            var context = new Database(builder.Options);
            InitDbContext(context);

            var repo = new MovieRepository(context);

            // Act
            repo.Delete(2);

            // Assert
            Assert.Equal(expected: 2, actual: context.Movies.Count());
        }

        #region " Temp Data "

        private void InitDbContext(Database context)
        {
            context.Movies.Add(new MovieEntity { Id = 1, Title = "Never Say Never Again", ReleaseYear = 1983, Summary = "A SPECTRE agent has stolen two American nuclear warheads, and James Bond must find their targets before they are detonated." });
            context.Movies.Add(new MovieEntity { Id = 2, Title = "Diamonds Are Forever ", ReleaseYear = 1971, Summary = "A diamond smuggling investigation leads James Bond to Las Vegas, where he uncovers an evil plot involving a rich business tycoon." });
            context.Movies.Add(new MovieEntity { Id = 3, Title = "You Only Live Twice ", ReleaseYear = 1967, Summary = "Agent 007 and the Japanese secret service ninja force must find and stop the true culprit of a series of spacejackings before nuclear war is provoked." });
            context.SaveChanges();
        }

        #endregion
    }
}
