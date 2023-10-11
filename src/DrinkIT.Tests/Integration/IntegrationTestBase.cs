using DrinkIT.Tests.Infrastructure;

namespace DrinkIT.Tests.Integration
{
    public abstract class IntegrationTestBase : IClassFixture<WebAppWithDBFactory>, IAsyncLifetime
    {
        protected readonly WebAppWithDBFactory factory;

        protected IntegrationTestBase(WebAppWithDBFactory factory) => this.factory = factory;

        public Task InitializeAsync() => this.factory.InicializeDBAsync();
        public Task DisposeAsync() => this.factory.DisposeDBAsync();
    }
}
