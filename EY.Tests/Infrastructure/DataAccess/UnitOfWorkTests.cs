using EY.Domain.Contracts;
using EY.Domain.Countries;
using EY.Infrastructure.DataAccess;

namespace EY.Tests.Infrastructure.DataAccess;

[TestFixture]
public class UnitOfWorkTests
{
    [SetUp]
    public void SetUp()
    {
        _dbContextMock = Substitute.For<AppDbContext>();
        _serviceProviderMock = Substitute.For<IServiceProvider>();
        _unitOfWork = new UnitOfWork(_dbContextMock, _serviceProviderMock);
    }

    [TearDown]
    public void TearDown()
    {
        _unitOfWork.Dispose();
        _dbContextMock.Dispose();
    }

    private AppDbContext _dbContextMock;
    private IServiceProvider _serviceProviderMock;
    private UnitOfWork _unitOfWork;

    [Test]
    public void Repository_ShouldReturnCorrectRepository()
    {
        // Arrange
        var repositoryMock = Substitute.For<IRepository<Country>>();
        _serviceProviderMock.GetService(typeof(IRepository<Country>))
            .Returns(repositoryMock);

        // Act
        var repository = _unitOfWork.Repository<Country>();

        // Assert
        Assert.That(repository, Is.EqualTo(repositoryMock));
    }

    [Test]
    public void Commit_ShouldCallDbContextSaveChanges()
    {
        // Act
        _unitOfWork.Commit();

        // Assert
        _dbContextMock.Received(1).SaveChanges();
    }

    [Test]
    public void Dispose_ShouldCallDbContextDispose()
    {
        // Act
        _unitOfWork.Dispose();

        // Assert
        _dbContextMock.Received(1).Dispose();
    }
}