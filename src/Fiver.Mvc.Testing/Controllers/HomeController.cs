using Microsoft.AspNetCore.Mvc;
using Fiver.Mvc.Testing.OtherLayers;
using System.Collections.Generic;
using System.Linq;
using Fiver.Mvc.Testing.Models.Home;

namespace Fiver.Mvc.Testing.Controllers
{
    public class HomeController : Controller
    {
        private readonly IMovieService service;

        public HomeController(IMovieService service)
        {
            this.service = service;
        }

        public IActionResult Index()
        {
            var model = service.GetMovies();

            var viewModel = ToViewModel(model);
            return View(viewModel);
        }

        public IActionResult Create()
        {
            var viewModel = new MovieViewModel();
            viewModel.IsNew = true;

            return View("CreateOrEdit", viewModel);
        }

        public IActionResult Edit(int id)
        {
            var model = service.GetMovie(id);
            if (model == null)
                return NotFound();

            var viewModel = ToViewModel(model);
            return View("CreateOrEdit", viewModel);
        }

        [HttpPost]
        public IActionResult Save(int id, MovieViewModel viewModel)
        {
            if (viewModel == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return View("CreateOrEdit", viewModel);

            var model = ToDomainModel(viewModel);
            if (viewModel.IsNew)
                service.AddMovie(model);
            else
                service.UpdateMovie(model);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var model = service.GetMovie(id);
            if (model == null)
                return NotFound();

            var viewModel = ToViewModel(model);
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult Delete(int id, MovieViewModel viewModel)
        {
            service.DeleteMovie(id);

            return RedirectToAction("Index");
        }

        #region " Mappings "

        private MovieViewModel ToViewModel(Movie model)
        {
            return new MovieViewModel
            {
                Id = model.Id,
                Title = model.Title,
                ReleaseYear = model.ReleaseYear,
                Summary = model.Summary
            };
        }

        private List<MovieInfoViewModel> ToViewModel(List<Movie> model)
        {
            return model.Select(item => new MovieInfoViewModel
            {
                Id = item.Id,
                Title = item.Title,
                ReleaseYear = item.ReleaseYear
            })
            .ToList();
        }

        private Movie ToDomainModel(MovieViewModel viewModel)
        {
            return new Movie
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                ReleaseYear = viewModel.ReleaseYear,
                Summary = viewModel.Summary
            };
        }

        #endregion
    }
}
