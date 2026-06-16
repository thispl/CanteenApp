using CanteenManagementSystem.Samples;
using Xunit;

namespace CanteenManagementSystem.EntityFrameworkCore.Applications;

[Collection(CanteenManagementSystemTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<CanteenManagementSystemEntityFrameworkCoreTestModule>
{

}
