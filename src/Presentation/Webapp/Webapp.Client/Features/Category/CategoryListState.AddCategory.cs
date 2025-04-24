using TimeWarp.State;

namespace Webapp.Client.Features.Category;

partial class CategoryListState
{
    public static class AddCategoryActionSet
    {
        public sealed class Action : IAction
        {
            public CreateCategoryRequest Request { get; }
            public Action(CreateCategoryRequest request)
            {
                Request = request;
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

            public override Task Handle(Action action, CancellationToken cancellationToken)
            {
                var newCategory = categoryService.CreateCategoryAsync(action.Request, cancellationToken);
                if (newCategory != null)
                {
                    CategoryListState.Categories.Add(newCategory);
                }
            }
        }
    }
}
