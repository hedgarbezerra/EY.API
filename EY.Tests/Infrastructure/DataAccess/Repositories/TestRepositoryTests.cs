﻿using EY.Infrastructure.DataAccess.Repositories;
using EY.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EY.Domain.Entities;

namespace EY.Tests.Infrastructure.DataAccess.Repositories
{
    [TestFixture]
    public class BaseRepositoryTests
    {
        private BaseRepository<Country> _repository;
        private AppDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new AppDbContext(options);
            _repository = new BaseRepository<Country>(_dbContext);
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }

        [Test]
        public void Add_ShouldAddEntityToDbSet()
        {
            // Arrange
            var entity = CreateCountry();

            // Act
            _repository.Add(entity);
            _dbContext.SaveChanges();

            // Assert
            _dbContext.Set<Country>().Should().ContainSingle(e => e.Name == "Brazil");
        }

        [Test]
        public void Delete_ShouldRemoveEntityFromDbSet()
        {
            // Arrange
            var entity = CreateCountry();
            _dbContext.Set<Country>().Add(entity);
            _dbContext.SaveChanges();

            // Act
            _repository.Delete(entity.Id.GetHashCode());
            _dbContext.SaveChanges();

            // Assert
            _dbContext.Set<Country>().Should().BeEmpty();
        }

        [Test]
        public void Get_ShouldReturnAllEntities()
        {
            // Arrange
            var entities = new[]
            {
            CreateCountry(),
            CreateCountry()
        };
            _dbContext.Set<Country>().AddRange(entities);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get();

            // Assert
            result.Should().HaveCount(2);
        }

        [Test]
        public void Get_WithFilter_ShouldReturnFilteredEntities()
        {
            // Arrange
            var entities = new[]
            {
                CreateCountry(),
                CreateCountry()
            };
            _dbContext.Set<Country>().AddRange(entities);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Get(e => e.Name == "Brazil");

            // Assert
            result.Should().ContainSingle(e => e.Name == "Brazil");
        }

        [Test]
        public void Update_ShouldModifyExistingEntity()
        {
            // Arrange
            var entity = CreateCountry();
            _dbContext.Set<Country>().Add(entity);
            _dbContext.SaveChanges();

            // Act
            entity.Name = "Updated Name";
            _repository.Update(entity);
            _dbContext.SaveChanges();

            // Assert
            _dbContext.Set<Country>().Single().Name.Should().Be("Updated Name");
        }

        [Test]
        public void Query_ShouldExecuteRawSqlQuery()
        {
            // Arrange
            var entity = CreateCountry();
            _dbContext.Set<Country>().Add(entity);
            _dbContext.SaveChanges();

            // Act
            var result = _repository.Query($"SELECT * FROM Countries WHERE Name = 'Brazil'");

            // Assert
            result.Should().ContainSingle(e => e.Name == "Brazil");
        }

        [Test]
        public void Execute_ShouldRunSqlNonQueryCommand()
        {
            // Arrange
            var entity = CreateCountry();
            _dbContext.Set<Country>().Add(entity);
            _dbContext.SaveChanges();

            // Act
            var rowsAffected = _repository.Execute($"DELETE FROM Countries WHERE Name = 'Brazil'");

            // Assert
            rowsAffected.Should().Be(1);
            _dbContext.Set<Country>().Should().BeEmpty();
        }

        private Country CreateCountry() => new Country
        {
            CreatedAt = DateTime.Now,
            Id = 1,
            Name = "Brazil",
            TwoLetterCode = "2",
            ThreeLetterCode = "3"

        };
    }
}
