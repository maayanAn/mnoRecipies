using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recepies.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter name")]
        public string Name { get; set; }
        public double Calories { get; set; }
        public double Protein { get; set; }
        public double sugar { get; set; }
        public double Iron { get; set; }
        public virtual ICollection<Recipe> RelatedRecipies{get; set;}
    }
}