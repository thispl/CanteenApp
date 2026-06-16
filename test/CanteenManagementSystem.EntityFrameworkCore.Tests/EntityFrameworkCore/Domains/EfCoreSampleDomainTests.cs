using CanteenManagementSystem.Samples;
using Xunit;

namespace CanteenManagementSystem.EntityFrameworkCore.Domains;

[Collection(CanteenManagementSystemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<CanteenManagementSystemEntityFrameworkCoreTestModule>
{

}
