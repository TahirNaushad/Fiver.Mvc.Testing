using Fiver.Mvc.Testing.Controllers;
using Fiver.Mvc.Testing.Models.Home;
using Fiver.Mvc.Testing.OtherLayers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace Fiver.Mvc.Testing.Tests.Unit
{
    public class HomeControllerTests
    {
        [Fact(DisplayName = "Index_returns_ViewResult_and_model")]
        public void Index_returns_ViewResult_and_model()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovies()).Returns(new List<Movie>());

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<List<MovieInfoViewModel>>(viewResult.Model);
        }

        [Fact(DisplayName = "Create_returns_ViewResult_and_model_with_IsNew_true")]
        public void Create_returns_ViewResult_and_model_with_IsNew_true()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Create();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected: "CreateOrEdit", actual: viewResult.ViewName);

            var viewModel = Assert.IsType<MovieViewModel>(viewResult.Model);
            Assert.True(viewModel.IsNew);
        }

        [Fact(DisplayName = "Edit_with_invalid_Id_returns_NotFound")]
        public void Edit_with_invalid_Id_returns_NotFound()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovie(It.IsAny<int>())).Returns((Movie)null);

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Edit(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Edit_with_valid_Id_returns_ViewResult_and_model")]
        public void Edit_with_valid_Id_returns_ViewResult_and_model()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovie(It.IsAny<int>()))
                .Returns(new Movie
                {
                    Id = 1, Title = "Spectre", ReleaseYear = 2015, Summary = "007 against spectre"
                });

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Edit(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.Equal(expected: "CreateOrEdit", actual: viewResult.ViewName);

            var viewModel = Assert.IsType<MovieViewModel>(viewResult.Model);
            Assert.Equal(expected: "Spectre", actual: viewModel.Title);
            Assert.Equal(expected: 2015, actual: viewModel.ReleaseYear);
            Assert.Equal(expected: "007 against spectre", actual: viewModel.Summary);
        }

        [Fact(DisplayName = "Save_with_empty_view_model_returns_BadRequest")]
        public void Save_with_empty_view_model_returns_BadRequest()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Save(1, null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }
        
        [Fact(DisplayName = "Save_with_invalid_model_state_returns_ViewResult_and_model")]
        public void Save_with_invalid_model_state_returns_ViewResult_and_model()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new HomeController(mockService.Object);
            sut.ModelState.AddModelError("Title", "Title is required");

            // Act
            var result = sut.Save(1, new MovieViewModel());

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<MovieViewModel>(viewResult.Model);
        }

        [Fact(DisplayName = "Save_with_new_model_calls_AddMovie_and_returns_RedirectToAction")]
        public void Save_with_new_model_calls_AddMovie_and_returns_RedirectToAction()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Save(1, new MovieViewModel() { IsNew = true });

            // Assert
            mockService.Verify(service => 
                service.AddMovie(It.IsAny<Movie>()), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(expected: "Index", actual: redirectResult.ActionName);
        }

        [Fact(DisplayName = "Save_with_existing_model_calls_UpdateMovie_and_returns_RedirectToAction")]
        public void Save_with_existing_model_calls_UpdateMovie_and_returns_RedirectToAction()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Save(1, new MovieViewModel() { IsNew = false });

            // Assert
            mockService.Verify(service =>
                service.UpdateMovie(It.IsAny<Movie>()), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(expected: "Index", actual: redirectResult.ActionName);
        }
        
        [Fact(DisplayName = "Delete_with_invalid_Id_returns_NotFound")]
        public void Delete_with_invalid_Id_returns_NotFound()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();
            mockService.Setup(service => service.GetMovie(It.IsAny<int>())).Returns((Movie)null);

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Delete(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact(DisplayName = "Delete_with_valid_Id_returns_ViewResult_and_model")]
        public void Delete_with_valid_Id_returns_ViewResult_and_model()
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

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Delete(1);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var viewModel = Assert.IsType<MovieViewModel>(viewResult.Model);
            Assert.Equal(expected: "Spectre", actual: viewModel.Title);
            Assert.Equal(expected: 2015, actual: viewModel.ReleaseYear);
            Assert.Equal(expected: "007 against spectre", actual: viewModel.Summary);
        }

        [Fact(DisplayName = "Delete_POST_with_valid_Id_calls_DeleteMovie_and_returns_RedirectToAction")]
        public void Delete_POST_with_valid_Id_calls_DeleteMovie_and_returns_RedirectToAction()
        {
            // Arrange
            var mockService = new Mock<IMovieService>();

            var sut = new HomeController(mockService.Object);

            // Act
            var result = sut.Delete(1, new MovieViewModel());

            // Assert
            mockService.Verify(service =>
                service.DeleteMovie(It.IsAny<int>()), Times.Once);

            var redirectResult = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal(expected: "Index", actual: redirectResult.ActionName);
        }
    }
}
