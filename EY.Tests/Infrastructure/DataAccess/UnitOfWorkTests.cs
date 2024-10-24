using EY.Domain.Contracts;
using EY.Domain.Entities;
using EY.Infrastructure.DataAccess;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EY.Tests.Infrastructure.DataAccess
{
    [TestFixture]
    public class UnitOfWorkTests
    {
        private AppDbContext _dbContextMock;
        private IServiceProvider _serviceProviderMock;
        private UnitOfWork _unitOfWork;

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
    }
