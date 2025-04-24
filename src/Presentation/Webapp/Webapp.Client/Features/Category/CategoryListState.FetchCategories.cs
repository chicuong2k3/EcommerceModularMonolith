using TimeWarp.State;

namespace Webapp.Client.Features.Category;

partial class CategoryListState
{
    public static class FetchCategoriesActionSet
    {
        public sealed class Action : IAction
        {
            public Action()
            {
            }
        }
        public sealed class Handler : ActionHandler<Action>
        {
            private readonly CategoryService categoryService;
            public Handler(IStore store, CategoryService categoryService) : base(store)
            {
                this.categoryService = categoryService;
            }

            private CategoryListState CategoryListState => Store.GetState<CategoryListState>();
            public override async Task Handle(Action action, CancellationToken cancellationToken)
            {
                var categories = await categoryService.GetCategoriesAsync(cancellationToken);
                CategoryListState.Categories = new(categories);
            }
        }
    }
}