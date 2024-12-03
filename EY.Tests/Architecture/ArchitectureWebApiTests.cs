using Microsoft.AspNetCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Architecture
{
    [TestFixture]
    public class ArchitectureWebApiTests : ArchitectureBaseTests
    {
        [Test]
        public void WebApiLayer_ShouldHaveDependenciesOrReferences()
        {
            var solutionReferences = WebApiAssembly.GetReferencedAssemblies()
                .Where(a => a.Name.StartsWith(SolutionPrefix));

            IReadOnlyList<string> expectedAssemblies =
                [ BusinessAssembly.Name(),
                    DomainAssembly.Name(),
                    InfrastructureAssembly.Name(),
                    SharedAssembly.Name() ];

            bool allReferenced = solutionReferences.All(r => expectedAssemblies.Contains(r.Name));

            allReferenced.Should().BeTrue();
            solutionReferences.Should().HaveCount(expectedAssemblies.Count);
        }

        [Test]
        public void WebApiLayer_Controllers_NameShouldEndWithController()
        {
            var result = Types.InAssembly(WebApiAssembly)
                .That()
                .ResideInNamespace("EY.API.Controllers")
                .Should()
                .HaveNameEndingWith("Controller")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void WebApiLayer_MiddlewaresNameSpace_NameShouldEndWithControllerOrHandler()
        {
            var result = Types.InAssembly(WebApiAssembly)
                .That()
                .ResideInNamespace("EY.API.Middlewares")
                .Should()
                .HaveNameEndingWith("Middleware")
                .Or()
                .HaveNameEndingWith("Handler")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }

        [Test]
        public void WebApiLayer_GlobalExceptionHandlers_NameShouldEndWithHandler()
        {
            var result = Types.InAssembly(WebApiAssembly)
                .That()
                .ImplementInterface(typeof(IExceptionHandler))
                .Should()
                .HaveNameEndingWith("Handler")
                .GetResult();

            result.IsSuccessful.Should().BeTrue();
        }
    }
}
