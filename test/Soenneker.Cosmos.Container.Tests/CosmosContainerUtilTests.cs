using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Tests.FixturedUnit;
using Xunit;


namespace Soenneker.Cosmos.Container.Tests;

[Collection("Collection")]
public class CosmosContainerUtilTests : FixturedUnitTest
{
    private readonly ICosmosContainerUtil _util;

    public CosmosContainerUtilTests(Fixture fixture, ITestOutputHelper output) : base(fixture, output)
    {
        _util = Resolve<ICosmosContainerUtil>(true);
    }

    [Fact]
    public void Default()
    {

    }
}
