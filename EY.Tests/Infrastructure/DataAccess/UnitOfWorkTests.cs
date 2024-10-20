using EY.Domain.Contracts;
using EY.Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Infrastructure.DataAccess
{
    public class UnitOfWorkTests
    {
        private AppDbContext _dbContext;
        private IServiceProvider _serviceProvider;
        private UnitOfWork _unitOfWork;

        [SetUp]
        public void Setup()
        {
            // Create a mock for the AppDbContext
            _dbContext = Substitute.For<AppDbContext>();

            // Create a mock for the IServiceProvider
            _serviceProvider = Substitute.For<IServiceProvider>();
        }

        [Test]
        public void Repository_ShouldReturnRepositoryInstance_WhenCalled()
        {
            // Arrange
            var repositoryMock = Substitute.For<IRepository<MyEntity>>();
            _serviceProvider.GetRequiredService<IRepository<MyEntity>>().Returns(repositoryMock);

            _unitOfWork = new UnitOfWork(_dbContext, _serviceProvider);

            // Act
            var repository = _unitOfWork.Repository<MyEntity>();

            // Assert
            repository.Should().NotBeNull();
            repository.Should().Be(repositoryMock);
        }

        [Test]
        public void Commit_ShouldCallSaveChangesOnDbContext_WhenCalled()
        {
            // Arrange
            _unitOfWork = new UnitOfWork(_dbContext, _serviceProvider);

            // Act
            _unitOfWork.Commit();

            // Assert
            _dbContext.Received(1).SaveChanges();
        }

        [Test]
        public void Dispose_ShouldDisposeDbContext_WhenCalled()
        {
            // Arrange
            _unitOfWork = new UnitOfWork(_dbContext, _serviceProvider);

            // Act
            _unitOfWork.Dispose();

            // Assert
            _dbContext.Received(1).Dispose();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
            _unitOfWork?.Dispose();
        }
    }

    // Example entity for the repository mock
    public class MyEntity
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
