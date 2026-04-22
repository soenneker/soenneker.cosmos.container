using Soenneker.Cosmos.Container.Abstract;
using Soenneker.Tests.HostedUnit;

namespace Soenneker.Cosmos.Container.Tests;

[ClassDataSource<Host>(Shared = SharedType.PerTestSession)]
public class CosmosContainerUtilTests : HostedUnitTest
{
    private readonly ICosmosContainerUtil _util;

    public CosmosContainerUtilTests(Host host) : base(host)
    {
        _util = Resolve<ICosmosContainerUtil>(true);
    }

    [Test]
    public void Default()
    {

    }
}
