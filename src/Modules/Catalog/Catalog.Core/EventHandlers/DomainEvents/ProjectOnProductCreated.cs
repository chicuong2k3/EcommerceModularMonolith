//using Catalog.Core.Events;
//using Catalog.Core.Repositories;
//using Shared.Abstractions.Application;

//namespace Catalog.Core.EventHandlers.DomainEvents;

//internal class ProjectOnProductCreated : DomainEventHandler<ProductCreated>
//{
//    private readonly IProductRepository productRepository;
//    private readonly IMongoProductRepository mongoProductRepository;

//    public ProjectOnProductCreated(
//        IProductRepository productRepository,
//        IMongoProductRepository mongoProductRepository)
//    {
//        this.productRepository = productRepository;
//        this.mongoProductRepository = mongoProductRepository;
//    }
//    public override async Task Handle(ProductCreated domainEvent, CancellationToken cancellationToken = default)
//    {
//        var product = await productRepository.GetByIdAsync(domainEvent.ProductId, cancellationToken);

//        if (product == null)
//        {
//            throw new ArgumentException($"Product with id '{domainEvent.ProductId}' not found");
//        }

//        await mongoProductRepository.AddAsync(product, cancellationToken);
//    }
//}
