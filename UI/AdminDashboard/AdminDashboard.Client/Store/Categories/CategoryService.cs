using Catalog.ApiContracts.Requests;
using Catalog.ApiContracts.Responses;

namespace AdminDashboard.Client.Store.Categories;

public class CategoryService : ICategoryService
{
    private List<CategoryResponse> categories = new()
    {
        new CategoryResponse
        {
            Id = new Guid("0CD4A533-7835-4C14-A3B1-79F955135EAA"),
            Name = "Category 1"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A94055940537"),
            Name = "Category 2"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A90055940537"),
            Name = "Category 3"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A91055940537"),
            Name = "Category 4"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A92055940537"),
            Name = "Category 5"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A84055940537"),
            Name = "Category 6"
        },
        new CategoryResponse
        {
            Id = new Guid("66BEDBF2-894F-4A85-A669-A93055940537"),
            Name = "Category 7"
        }
    };

    public Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var newId = Guid.NewGuid();
        while (true)
        {
            if (!categories.Any(c => c.Id == newId))
            {
                break;
            }
            newId = Guid.NewGuid();
        }
        var category = new CategoryResponse
        {
            Id = newId,
            Name = request.Name
        };
        categories.Add(category);
        return Task.FromResult(category);
    }

    public Task DeleteCategoryAsync(Guid categoryId)
    {
        categories.RemoveAll(c => c.Id == categoryId);
        return Task.CompletedTask;
    }

    public Task<IEnumerable<CategoryResponse>> GetCategoriesAsync()
    {
        Task.Delay(2000);
        return Task.FromResult(categories.AsEnumerable());
    }

    public Task<CategoryResponse> GetParentCategoryAsync(Guid categoryId)
    {
        var parentCategory = new CategoryResponse
        {
            Id = new Guid("66BEDBF1-894F-4A85-A669-A94055940537"),
            Name = "Category 2"
        };
        return Task.FromResult(parentCategory);
    }

    public Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request)
    {
        var category = categories.FirstOrDefault(c => c.Id == categoryId);
        if (category != null)
        {
            category.Name = request.NewName;

        }
        return Task.FromResult(category);
    }
}
