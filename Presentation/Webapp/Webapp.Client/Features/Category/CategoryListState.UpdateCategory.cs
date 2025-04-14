namespace Webapp.Client.Features.Category;

partial class CategoryListState
{
    public static class UpdateCategoryActionSet
    {
        public sealed class Action : IAction
        {
            public Guid CategoryId { get; }
            public UpdateCategoryRequest Request { get; }
            public Action(Guid categoryId, UpdateCategoryRequest request)
            {
                CategoryId = categoryId;
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
            public override async Task Handle(Action action, CancellationToken cancellationToken)
            {
                var category = CategoryListState.Categories.FirstOrDefault(c => c.Id == action.CategoryId);
                if (category != null)
                {
                    await categoryService.UpdateCategoryAsync(action.CategoryId, action.Request, cancellationToken);

                }
            }
        }
    }
}
