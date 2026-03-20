using EpiPageImporter.Business.Models.Dtos;
using EpiPageImporter.Business.Services;
using EpiPageImporter.Models.Pages;

namespace EpiPageImporter.Business.Mapping;

public class RecipePageMapper(
    CuisineService cuisineService,
    RecipeImageService imageService)
{
    private readonly CuisineService _cuisineService = cuisineService;
    private readonly RecipeImageService _imageService = imageService;

    public async Task MapAsync(RecipeDto dto, RecipePage page)
    {
        page.Name = dto.Name ?? $"Recipe {dto.Id}";
        page.RecipeName = dto.Name;
        page.Ingredients = dto.Ingredients ?? [];
        page.Instructions = new XhtmlString(string.Join("<br/>", dto.Instructions ?? []));
        page.PrepTimeMinutes = dto.PrepTimeMinutes;
        page.CookTimeMinutes = dto.CookTimeMinutes;
        page.Servings = dto.Servings;
        page.Difficulty = dto.Difficulty;
        page.Cuisine = dto.Cuisine;
        page.CaloriesPerServing = dto.CaloriesPerServing;
        page.Tags = dto.Tags ?? [];
        page.MealType = dto.MealType ?? [];
        page.Rating = dto.Rating;
        page.ReviewCount = dto.ReviewCount;
        page.UserId = dto.UserId;

        _cuisineService.AssignCuisineCategory(page, dto.Cuisine);
        page.Image = await _imageService.ImportAsync(dto.Image, page.Image ?? ContentReference.EmptyReference);
    }
}
