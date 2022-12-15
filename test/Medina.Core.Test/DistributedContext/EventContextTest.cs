namespace Medina.Core.Test.DistributedContext;

using Model;

using System.Threading.Tasks;

using Xunit;

public sealed class EventContextTest
{
    [Fact]
    public async Task SimplePublishEventTest()
    {
        var context = new Core.DistributedContext();
        var model = new TestEventModel();
        
        await context.PublishEventAsync(model);
    }
}