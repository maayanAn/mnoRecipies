using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recepies.Models
{
    public class Recipe
    {
        public int Id{ get; set; }


        [Required(ErrorMessage = "Please enter a name")]
        public string Name { get; set; }
        public ICollection<Ingredient> Ingredients { get; set; }

        [Required(ErrorMessage = "Please specify difficulty")]
        public Difficulty Difficulty { get; set; }

        [Required(ErrorMessage = "Please enter preparation time")]
        [Display(Name = "Preparation time in minutes")]
        [Range(1, 5000)]
        public int PreparationTimeInMinutes { get; set; }
        public User User { get; set; }

        [Display(Name = "Recipe context")]
        [Required(ErrorMessage = "Please enter the recipe")]
        public string Context{ get; set; }
    }
}