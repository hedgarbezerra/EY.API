using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    [TestFixture]
    public class ArchitectureSharedTests : ArchitectureBaseTests
    {
        [Test]
        public void SharedLayer_ShouldHaveDomainProjectReferenceAlone()
        {
            var solutionReferences = SharedAssembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(SolutionPrefix));

            solutionReferences.Should().HaveCount(1);
            solutionReferences.First().Name.Should().NotBeNull().And.Be(DomainAssembly.Name());
        }

        [Test]
        public void SharedLayer_Attributes_NameShouldEndWithAttribute()
        {
            var result = Types.InAssembly(SharedAssembly)
                .That()
                .ResideInNamespaceStartingWith("EY.Shared.Attributes")
                .Should()
                .HaveNameEndingWith("Attributes")
                .Or()
                .HaveNameEndingWith("Attribute")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void SharedLayer_Extensions_NameShouldEndWithExtension()
        {
            var result = Types.InAssembly(SharedAssembly)
                .That()
                .ResideInNamespaceStartingWith("EY.Shared.Extensions")
                .Should()
                .HaveNameEndingWith("Extensions")
                .Or()
                .HaveNameEndingWith("Extension")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
