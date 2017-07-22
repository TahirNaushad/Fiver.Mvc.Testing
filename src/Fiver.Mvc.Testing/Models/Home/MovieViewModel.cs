using System.ComponentModel.DataAnnotations;

namespace Fiver.Mvc.Testing.Models.Home
{
    public class MovieViewModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int ReleaseYear { get; set; }
        public string Summary { get; set; }

        public bool IsNew { get; set; }
    }
}
