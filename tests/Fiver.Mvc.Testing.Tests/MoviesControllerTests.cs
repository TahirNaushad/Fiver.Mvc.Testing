using Fiver.Mvc.Testing.Controllers;
using Fiver.Mvc.Testing.Lib;
using Fiver.Mvc.Testing.Models.Movies;
using Fiver.Mvc.Testing.OtherLayers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Fiver.Mvc.Testing.Tests
{
    public class MoviesControllerTests
    {
        [Fact(DisplayName = "Get_retruns_OkObjectResult_and_model")]
        public void Get_retruns_Ok_result_and_model()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovies()).Returns(new List<Movie>());

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Get();

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var outputModel = Assert.IsType<List<MovieOutputModel>>(okObjectResult.Value);
        }

        [Fact(DisplayName = "Get_with_invalid_Id_returns_NotFound")]
        public void Get_with_invalid_Id_returns_NotFound()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovie(It.IsAny<int>())).Returns((Movie)null);

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Get(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Get_with_valid_Id_returns_OkObjectResult_and_model")]
        public void Get_with_valid_Id_returns_OkObjectResult_and_model()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovie(It.IsAny<int>()))
                .Returns(new Movie
                {
                    Id = 1,
                    Title = "Spectre",
                    ReleaseYear = 2015,
                    Summary = "007 against spectre"
                });

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Get(1);

            // Assert
            var okObjectResult = Assert.IsType<OkObjectResult>(result);
            var outputModel = Assert.IsType<MovieOutputModel>(okObjectResult.Value);
            Assert.Equal(expected: "Spectre", actual: outputModel.Title);
            Assert.Equal(expected: 2015, actual: outputModel.ReleaseYear);
            Assert.Equal(expected: "007 against spectre", actual: outputModel.Summary);
        }

        [Fact(DisplayName = "Create_with_empty_input_model_returns_BadRequest")]
        public void Create_with_empty_input_model_returns_BadRequest()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Create(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact(DisplayName = "Create_with_invalid_model_state_returns_Unprocessable_and_model_state")]
        public void Create_with_invalid_model_state_returns_Unprocessable_and_model_state()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new MoviesController(mockService.Object);
            sut.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = sut.Create(new MovieInputModel());

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(unprocessableResult.Value);
        }

        [Fact(DisplayName = "Create_with_valid_model_calls_AddMovie_and_returns_CreatedAtRoute")]
        public void Create_with_valid_model_calls_AddMovie_and_returns_CreatedAtRoute()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Create(new MovieInputModel());

            // Assert
            mockService.Verify(service =>
                service.AddMovie(It.IsAny<Movie>()), Times.Once);

            var createAtRouteResult = Assert.IsType<CreatedAtRouteResult>(result);
            Assert.Equal(expected: "GetMovie", actual: createAtRouteResult.RouteName);
        }

        [Fact(DisplayName = "Update_with_empty_input_model_returns_BadRequest")]
        public void Update_with_empty_input_model_returns_BadRequest()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Update(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact(DisplayName = "Update_with_Id_not_matching_input_model_returns_BadRequest")]
        public void Update_with_Id_not_matching_input_model_returns_BadRequest()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Update(1, new MovieInputModel { Id = 2 });

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact(DisplayName = "Update_with_invalid_Id_returns_NotFound")]
        public void Update_with_invalid_Id_returns_NotFound()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.MovieExists(It.IsAny<int>())).Returns(false);

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Update(0, new MovieInputModel());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Update_with_invalid_model_state_returns_Unprocessable_and_model_state")]
        public void Update_with_invalid_model_state_returns_Unprocessable_and_model_state()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.MovieExists(It.IsAny<int>())).Returns(true);

            var sut = new MoviesController(mockService.Object);
            sut.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = sut.Update(1, new MovieInputModel() { Id = 1 });

            // Assert
            var unprocessableResult = Assert.IsType<UnprocessableObjectResult>(result);
            var modelState = Assert.IsType<SerializableError>(unprocessableResult.Value);
        }

        [Fact(DisplayName = "Update_with_valid_model_calls_UpdateMovie_and_returns_NoContent")]
        public void Update_with_valid_model_calls_UpdateMovie_and_returns_NoContent()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.MovieExists(It.IsAny<int>())).Returns(true);

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Update(1, new MovieInputModel() { Id = 1 });

            // Assert
            mockService.Verify(service =>
                service.UpdateMovie(It.IsAny<Movie>()), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact(DisplayName = "Delete_with_invalid_Id_returns_NotFound")]
        public void Delete_with_invalid_Id_returns_NotFound()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.MovieExists(It.IsAny<int>())).Returns(false);

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Delete(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Delete_with_valid_Id_calls_DeleteMovie_and_returns_NoContent")]
        public void Delete_with_valid_Id_calls_DeleteMovie_and_returns_NoContent()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.MovieExists(It.IsAny<int>())).Returns(true);

            var sut = new MoviesController(mockService.Object);

            // Act
            var result = sut.Delete(1);

            // Assert
            mockService.Verify(service =>
                service.DeleteMovie(It.IsAny<int>()), Times.Once);

            Assert.IsType<NoContentResult>(result);
        }
    }
}
