namespace Catalog.IntegrationTests.Abstractions;

[CollectionDefinition(nameof(IntegrationTestCollection))]
public class IntegrationTestCollection : ICollectionFixture<IntegrationTestWebAppFactory>
{

}
