using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Recepies.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        public string Password{ get; set; }
        public bool IsManager { get; set; }

        [Required(ErrorMessage = "Please enter a name")]
        [Display(Name = "Full name")]
        public string FullName{ get; set; }
        public virtual ICollection<Recipe> Recipies{get; set;}
    }
}