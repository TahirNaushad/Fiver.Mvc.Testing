using Fiver.Mvc.Testing.Models.Movies;
using Fiver.Mvc.Testing.OtherLayers;
using Fiver.Mvc.Testing.Tests.Lib;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using Moq;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;

namespace Fiver.Mvc.Testing.Tests.Integration
{
    public class MoviesControllerIntegration : IntegrationTestsBase<Startup>
    {
        [Fact(DisplayName = "Get_retruns_Ok")]
        public async Task Get_retruns_Ok_status_code()
        {
            // Arrange

            // Act
            var response = await this.Client.GetAsync("api/movies");

            // Assert
            Assert.Equal(expected: HttpStatusCode.OK, actual: response.StatusCode);

            var outputModel = response.ContentAsType<List<MovieOutputModel>>();
            Assert.Equal(expected: 2, actual: outputModel.Count);
        }

        [Fact(DisplayName = "Get_with_invalid_id_returns_NotFound")]
        public async Task Get_with_invalid_id_returns_NotFound()
        {
            // Arrange

            // Act
            var response = await this.Client.GetAsync("api/movies/100");

            // Assert
            Assert.Equal(expected: HttpStatusCode.NotFound, actual: response.StatusCode);
        }

        [Fact(DisplayName = "Create_with_invalid_input_model_returns_Unprocessable")]
        public async Task Create_with_invalid_input_model_returns_Unprocessable()
        {
            // Arrange
            var inputModel = new MovieInputModel { };
            var content = new StringContent(JsonConvert.SerializeObject(inputModel),
                                                Encoding.UTF8, "application/json");

            // Act
            var response = await this.Client.PostAsync("api/movies", content);

            // Assert
            Assert.Equal(expected: 422, actual: (int)response.StatusCode);
        }

        [Fact(DisplayName = "Create_with_valid_model_returns_CreatedAtRoute")]
        public async Task Create_with_valid_model_returns_CreatedAtRoute()
        {
            // Arrange
            var inputModel = new MovieInputModel { Title = "Spider Man", ReleaseYear = 2017, Summary = "Ok movie" };
            var content = new StringContent(JsonConvert.SerializeObject(inputModel),
                                                Encoding.UTF8, "application/json");

            // Act
            var response = await this.Client.PostAsync("api/movies", content);

            // Assert
            Assert.Equal(expected: HttpStatusCode.Created, actual: response.StatusCode);

            var outputModel = response.ContentAsType<MovieOutputModel>();
            Assert.Equal(expected: "Spider Man", actual: outputModel.Title);
        }

        [Fact(DisplayName = "Delete_with_valid_Id_returns_NoContent")]
        public async Task Delete_with_valid_Id_returns_NoContent()
        {
            // Arrange
            
            // Act
            var response = await this.Client.DeleteAsync("api/movies/1");

            // Assert
            Assert.Equal(expected: HttpStatusCode.NoContent, actual: response.StatusCode);
        }
        
        #region " Setup "

        protected override void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IMovieService, TestMovieService>();
        }

        #endregion
    }
}
