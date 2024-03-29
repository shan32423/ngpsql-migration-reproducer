﻿using Migration.Reproducer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Npgsql;
using System;

namespace Migration.Reproducer.Tests
{
    public class MyEntityRepositoryTests: IDisposable
    {
          

        public MyEntityRepositoryTests()
        {
           
            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.Migrate();
                if (dbContext.Database.GetDbConnection() is NpgsqlConnection npgsqlConnection)
                {
                     npgsqlConnection.Open();
                    npgsqlConnection.ReloadTypes();
                    npgsqlConnection.Close();
                }
            }

        }
        public void Dispose()
        {
            // Dispose of resources, such as the database context
            using (var dbContext = new AppDbContext())
            {
                dbContext.Database.EnsureDeleted();
            }
        }
        [Fact]
        public void CrudOperation_Success()
        {
            // Arrange
      

            using (var dbContext = new AppDbContext())
            {
                var repository = new MyEntityRepository(dbContext);

                // Act
                repository.Add(new ActiveEntity { Id = 1, Name = "TestEntity 1" });
                repository.Add(new InactiveEntity { Id = 2, Name = "TestEntity 2", status = StatusType.Inactive });
                repository.Add(new ActiveEntity { Id = 3, Name = "TestEntity 3", status = StatusType.Active });

                // Assert
                var entity = repository.GetById(1);
                
                Assert.NotNull(repository.GetAllActive());
                Assert.Equal("TestEntity 1", entity.Name);
                Assert.Equal(3, repository.GetAll().Count());
                Assert.Equal(2, repository.GetAllActive().Count());
                Assert.Equal(1, repository.GetAllInactive().Count());

            }
        }
    }
}
