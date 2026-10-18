// MapsterExtensionsTests.cs

using Yggdrasil.Extensions;

namespace Yggdrasil.Tests.Extensions;

public class MapsterExtensionsTests{
    public class Source{
        public List<string>? Tags { get; set; }
    }

    public class Destination{
        public List<string> Tags { get; set; } = ["default"];
    }

    [Fact]
    public void Patch_NullValue_KeepsDestinationDefault(){
        var source = new Source { Tags = null };

        var result = source.Patch<Destination>();
        Assert.Equal(new[] { "default" }, result.Tags);
    }
}
