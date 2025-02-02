using System.ComponentModel.DataAnnotations;

namespace OnlineMarket.API.ViewModels
{
    public class UploadImageModel
    {
        [Required]
        [Display(Name = "Picture")]
        public IFormFile Picture { get; set; }

    }
}
