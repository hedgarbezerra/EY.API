using EY.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    [TestFixture]
    public class ArchitectureDomainTests : ArchitectureBaseTests
    {
        [Test]
        public void DomainLayer_ShouldNotHaveDependenciesOrReferences()
        {
            var solutionReferences = DomainAssembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(SolutionPrefix));

            solutionReferences.Should().BeEmpty();
        }


        [Test]
        public void DomainLayer_Types_ShouldNotHaveDependencies()
        {
            var result = Types.InAssembly(DomainAssembly)
                .Should()
                .NotHaveDependencyOnAll(Assemblies.Names().ToArray())
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
        
        [Test]
        public void DomainLayer_ResultsShould_BeStatic()
        {
            var resultsTypes = Types.InAssembly(DomainAssembly)
                .That()
                .Inherit(typeof(Result))
                .Or()
                .Inherit(typeof(Result<>));

            var result = resultsTypes
                .Should()
                .HaveNameEndingWith("Results")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void DomainLayer_Interfaces_ShouldStartWithCaptalI()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .AreInterfaces()
                .Should()
                .HaveNameStartingWith("I")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void DomainLayer_Options_ShouldBeInModelOptionsNamespaceAndEndWithOptions()
        {
            var result = Types.InAssembly(DomainAssembly)
                .That()
                .ResideInNamespace("EY.Domain.Models.Options")
                .Should()
                .HaveNameEndingWith("Options")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void DomainLayer_InterfaceImplementations_ClassNameShouldEndWithSameNameOfInterfaceWithoutPrefix()
        {
            var classes = Types.InAssembly(DomainAssembly)
            .That()
            .AreClasses()
            .GetTypes();

            var result = classes.NamesEndsWithInterfaceName();

            result.success.Should().BeTrue();
            result.failedTypes.Should().BeEmpty();
        }
    }
}
