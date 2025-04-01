using EY.Infrastructure.DataAccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EY.Tests.Architecture;

[TestFixture]
public class ArchitectureInfrastructureTests : ArchitectureBaseTests
{
    [Test]
    public void InfrastructureLayer_ShouldNotHaveDependenciesOrReferences()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .Should()
            .NotHaveDependencyOnAll(Assemblies.Names().ToArray())
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void InfrastructureLayer_Interfaces_ShouldStartWithCaptalI()
    {
        var result = Types.InAssembly(InfrastructureAssembly)
            .That()
            .AreInterfaces()
            .Should()
            .HaveNameStartingWith("I")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }


    [Test]
    public void InfrastructureLayer_RepositoryImplementations_NameShouldEndWithRepository()
    {
        var classesPredicate = Types.InAssembly(InfrastructureAssembly)
            .That()
            .AreClasses()
            .And()
            .Inherit(typeof(BaseRepository<>));

        var result = classesPredicate.Should()
            .NotBeGeneric()
            .And()
            .HaveNameEndingWith("Repository")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Test]
    public void InfrastructureLayer_EntityConfigurations_NameShouldEndWithEntityConfiguration()
    {
        var classesPredicate = Types.InAssembly(InfrastructureAssembly)
            .That()
            .AreClasses()
            .And()
            .ImplementInterface(typeof(IEntityTypeConfiguration<>));

        var result = classesPredicate.Should()
            .NotBeGeneric()
            .And()
            .HaveNameEndingWith("EntityTypeConfiguration")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}