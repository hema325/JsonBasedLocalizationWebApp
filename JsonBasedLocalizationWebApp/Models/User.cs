using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace JsonBasedLocalizationWebApp.Models
{
    public class User
    {
        [DisplayName("Name")]
        [Required(ErrorMessage = "required")]
        public string Name { get; set; }

        [DisplayName("Address")]
        [Required(ErrorMessage = "required")]
        public string Address { get; set; }
    }
}
