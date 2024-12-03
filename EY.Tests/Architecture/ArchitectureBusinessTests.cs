using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    [TestFixture]
    public class ArchitectureBusinessTests : ArchitectureBaseTests
    {
        [Test]
        public void BusinessLayer_ShouldHaveTwoReferences()
        {
            var solutionReferences = BusinessAssembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(SolutionPrefix));

            IReadOnlyList<string> expectedAssemblies =
                [ DomainAssembly.Name(),
                    SharedAssembly.Name() ];

            bool allReferenced = solutionReferences.All(r => expectedAssemblies.Contains(r.Name));

            allReferenced.Should().BeTrue();
            solutionReferences.Should().HaveCount(expectedAssemblies.Count);
        }

        [Test]
        public void BusinessLayer_CountriesTypes_ShouldStartWithCountries()
        {
            var result = Types.InAssembly(BusinessAssembly)
                .That()
                .ResideInNamespaceStartingWith("EY.Business.Countries")
                .Should()
                .HaveNameStartingWith("Countries")
                .Or()
                .HaveNameStartingWith("Country")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void BusinessLayer_IpAddressesTypes_ShouldStartWithCountries()
        {
            var result = Types.InAssembly(BusinessAssembly)
                .That()
                .ResideInNamespaceStartingWith("EY.Business.IpAddresses")
                .Should()
                .HaveNameStartingWith("IpAddresses")
                .Or()
                .HaveNameStartingWith("IpAddress")
                .Or()
                .HaveNameStartingWith("Ip")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
