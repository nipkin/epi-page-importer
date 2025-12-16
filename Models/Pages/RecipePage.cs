using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace EpiPageImporter.Models.Pages
{
    [ContentType(
       DisplayName = "Recipe Page",
       GUID = "b3f1ba55-3b63-4ac9-ba9a-3e5ae5345cc3",
       Description = "A page representing a recipe imported from an external source.")]
    public class RecipePage : BasePageData
    {
        [Display(
            Name = "External Recipe ID", 
            Order = 1, 
            GroupName = SystemTabNames.Settings)]
        public virtual int ExternalRecipeId { get; set; }

        [Display(
            Name = "Recipe Name", 
            GroupName = SystemTabNames.Content, 
            Order = 10)]
        public virtual string? RecipeName { get; set; }

        [Display(
            Name = "Ingredients", 
            GroupName = SystemTabNames.Content, 
            Order = 20)]
        public virtual IList<string>? Ingredients { get; set; }

        [Display(
            Name = "Instructions", 
            GroupName = SystemTabNames.Content, 
            Order = 30)]
        public virtual XhtmlString? Instructions { get; set; }

        [Display(
            Name = "Prep Time (Minutes)", 
            GroupName = SystemTabNames.Content, 
            Order = 40)]
        public virtual int PrepTimeMinutes { get; set; }

        [Display(
            Name = "Cook Time (Minutes)", 
            GroupName = SystemTabNames.Content, 
            Order = 50)]
        public virtual int CookTimeMinutes { get; set; }

        [Display(
            Name = "Servings", 
            GroupName = SystemTabNames.Content, 
            Order = 60)]
        public virtual int Servings { get; set; }

        [Display(
            Name = "Difficulty", 
            GroupName = SystemTabNames.Content, 
            Order = 70)]
        public virtual string? Difficulty { get; set; }

        [Display(
            Name = "Cuisine", 
            GroupName = SystemTabNames.Content, 
            Order = 80)]
        public virtual string? Cuisine { get; set; }

        [Display(
            Name = "Calories Per Serving", 
            GroupName = SystemTabNames.Content, 
            Order = 90)]
        public virtual int CaloriesPerServing { get; set; }

        [Display(
            Name = "Tags", 
            GroupName = SystemTabNames.Content, 
            Order = 100)]
        public virtual IList<string>? Tags { get; set; }

        [Display(
            Name = "User Id", 
            GroupName = SystemTabNames.Content, 
            Order = 110)]
        public virtual int UserId { get; set; }

        [UIHint(UIHint.Image)]
        [Display(
            Name = "Image", 
            GroupName = SystemTabNames.Content, 
            Order = 120)]
        public virtual ContentReference? Image { get; set; }

        [Display(
            Name = "Rating", 
            GroupName = SystemTabNames.Content, 
            Order = 130)]
        public virtual double Rating { get; set; }

        [Display(
            Name = "Review Count", 
            GroupName = SystemTabNames.Content, 
            Order = 140)]
        public virtual int ReviewCount { get; set; }

        [Display(
            Name = "Meal Type", 
            GroupName = SystemTabNames.Content, 
            Order = 150)]
        public virtual IList<string>? MealType { get; set; }
    }
}
