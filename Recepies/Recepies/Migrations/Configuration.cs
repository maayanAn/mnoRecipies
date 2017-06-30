using Recepies.Models;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;

namespace Recepies.Migrations
{
    internal sealed class Configuration : DbMigrationsConfiguration<DbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(Recepies.Models.DbContext context)
        {
            var dataUs = (from n in context.Users select n);
            context.Users.RemoveRange(dataUs);
            var dataIn = (from n in context.Ingredients select n);
            context.Ingredients.RemoveRange(dataIn);
            var data = (from n in context.Recipies select n);
            context.Recipies.RemoveRange(data);
            context.SaveChanges();

            context.Users.Add(new User() { Id = 1, FullName = "omri vardi", IsManager = false, Password = "123" });
            context.Users.Add(new User() { Id = 2, FullName = "noam etzion", IsManager = true, Password = "123" });
            context.Users.Add(new User() { Id = 3, FullName = "maayan angel", IsManager = true, Password = "123" });

            context.Ingredients.Add(new Ingredient() {Id = 1, Name = "Tomato", Calories = 12, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 2, Name = "Salt", Calories = 12, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 3, Name = "Orange", Calories = 12, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 4, Name = "Water", Calories = 12, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 5, Name = "Milk", Calories = 10, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 6, Name = "Choclate", Calories = 10, Iron = 14,Protein =15, sugar=40 });
            context.Ingredients.Add(new Ingredient() {Id = 7, Name = "Sugar", Calories = 8, Iron = 14,Protein =15, sugar=40 });

            context.Recipies.Add(new Recipe() { Id = 1, Name = "Spagetti", Context = "Cook all the ingredients together", Difficulty = Difficulty.Normal, Ingredients = new List<Ingredient> { context.Ingredients.Find(1), context.Ingredients.Find(2) }, PreparationTimeInMinutes = 10, User = context.Users.Find(1)});
            context.Recipies.Add(new Recipe() { Id = 2, Name = "Pasta", Context = "Cook the dry ingrediants first", Difficulty = Difficulty.Hard, Ingredients = new List<Ingredient> { context.Ingredients.Find(1), context.Ingredients.Find(2) }, PreparationTimeInMinutes = 80, User = context.Users.Find(2)});
            context.Recipies.Add(new Recipe() { Id = 3, Name = "Salad", Context = "Cook all the ingredients together", Difficulty = Difficulty.Simple, Ingredients = new List<Ingredient> { context.Ingredients.Find(5), context.Ingredients.Find(3) }, PreparationTimeInMinutes = 10, User = context.Users.Find(3)});
            context.Recipies.Add(new Recipe() { Id = 4, Name = "Hot Dog", Context = "Cook all the ingredients together", Difficulty = Difficulty.Simple, Ingredients = new List<Ingredient> { context.Ingredients.Find(1), context.Ingredients.Find(2) }, PreparationTimeInMinutes = 10, User = context.Users.Find(1)});
            context.Recipies.Add(new Recipe() { Id = 5, Name = "Fish & chips", Context = "Cook all the ingredients together", Difficulty = Difficulty.Simple, Ingredients = new List<Ingredient> { context.Ingredients.Find(1), context.Ingredients.Find(6) }, PreparationTimeInMinutes = 45, User = context.Users.Find(1)});
            context.Recipies.Add(new Recipe() { Id = 6, Name = "Sushi", Context = "Cook all the ingredients together", Difficulty = Difficulty.Simple, Ingredients = new List<Ingredient> { context.Ingredients.Find(1), context.Ingredients.Find(2) }, PreparationTimeInMinutes = 10, User = context.Users.Find(1)});
            context.Recipies.Add(new Recipe() { Id = 7, Name = "Rice", Context = "Cook all the ingredients together", Difficulty = Difficulty.Chefs, Ingredients = new List<Ingredient> { context.Ingredients.Find(7), context.Ingredients.Find(2) }, PreparationTimeInMinutes = 30, User = context.Users.Find(3)});
            context.Recipies.Add(new Recipe() { Id = 8, Name = "Pizza", Context = "Cook all the ingredients together", Difficulty = Difficulty.Normal, Ingredients = new List<Ingredient> { context.Ingredients.Find(5), context.Ingredients.Find(4) }, PreparationTimeInMinutes = 20, User = context.Users.Find(2)});

            context.SaveChanges();
        }
    }
}
