using System.ComponentModel;
using Xunit;

namespace Mappium.UITest
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    [CollectionDefinition(nameof(MappiumTest), DisableParallelization = true)]
    public class MappiumTestCollection : ICollectionFixture<MappiumTest>
    {
    }
}
