namespace swishesUnitTests
{
    using System;
    using System.Data.Entity;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using NUnit.Framework;
    using swishes.Infrastructure.Repositories;
    using swishes.Infrastructure.Logging;
    using Moq;
    using swishes.Infrastructure.DataAccess;
    using FakeDbSet;
    using System.Data.Entity.Infrastructure;
    using System.Data;

    [TestFixture]
    public class RepositoryUnitTests
    {
        private Mock<IDataContext> ctxMock;
        private IRepository<object> testRepo;
        private InMemoryDbSet<object> fakeDbSet;
        private Mock<ILogger> _logger;

        [SetUp]
        public void PrepareData()
        {
            _logger = new Mock<ILogger>();
            ctxMock = new Mock<IDataContext>();
            fakeDbSet = new InMemoryDbSet<object>();
            for (int i = 0; i < 10; i++)
            {
                fakeDbSet.Add(new object());
            }
            ctxMock.Setup(ctx => ctx.Set<object>()).Returns(fakeDbSet);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
        }

        #region GetAll()
        
        [Test]
        public void GetAllResultIsNotNullTestMethod()
        {
            Assert.IsNotNull(testRepo.GetAll());
        }

        [Test]
        public void GetAllReturnsAll10Items()
        {
            Assert.AreEqual(10, testRepo.GetAll().Count());
        }
        #endregion

        #region Add()
        
        [Test]
        public void AddIncreasesItemCountByOne()
        {
            int countBefore = fakeDbSet.Count();
            testRepo.Add(new object());
            Assert.AreEqual(countBefore + 1, testRepo.GetAll().Count());
        }

        [Test]
        public void AddOperationResultSeccessForNewItem()
        {
            var operationResult = testRepo.Add(new object());
            Assert.AreEqual(RepositoryOperationResult.Success, operationResult);
        }

        [Test]
        public void AddOperationResultErrorForExistingItem()
        {
            var item = new object();
            fakeDbSet.Add(item);
            var operationResult = testRepo.Add(item);
            Assert.AreEqual(RepositoryOperationResult.EntityAlreadyExists, operationResult);
        }

        [Test]
        public void AddOperationResultUnknownErrorIfException()
        {
            var exceptionalDbSet = new Mock<IDbSet<object>>();
            exceptionalDbSet.Setup(set => set.Add(new object())).Throws(new Exception("Some strange exception for testing"));
            ctxMock.Setup(ctx => ctx.Set<object>()).Returns(exceptionalDbSet.Object);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
            var operationResult = testRepo.Add(new object());
            Assert.AreEqual(RepositoryOperationResult.UnknownError, operationResult);
        }

        [Test]
        public void AddOperationResultNullEntityForNullArgumetnt()
        {
            var operationResult = testRepo.Add(null);
            Assert.AreEqual(RepositoryOperationResult.NullEntity, operationResult);
        }

        [Test]
        public void AddOperationDoesNotIncreaseIfOperationNotSuccess()
        {
            int countBefore = fakeDbSet.Count();
            var existingItem = fakeDbSet.FirstOrDefault();
            testRepo.Add(existingItem);
            testRepo.Add(null);
            Assert.AreEqual(fakeDbSet.Count(), countBefore);
        }

        #endregion

        #region Delete()

        [Test]
        public void DeleteDecreasesItemCountByOne()
        {
            int countBefore = fakeDbSet.Count();
            var itemToDelete = fakeDbSet.FirstOrDefault();
            ctxMock.Setup(c => c.SetEntryState<object>(itemToDelete, EntityState.Deleted)).Returns(false);
            testRepo.Delete(itemToDelete);
            Assert.AreEqual(countBefore - 1, fakeDbSet.Count());
        }

        [Test]
        public void DeleteOperationRresultSuccessForExistingItem()
        {
            var itemToDelete = fakeDbSet.FirstOrDefault();
            ctxMock.Setup(c => c.SetEntryState<object>(itemToDelete, EntityState.Deleted)).Returns(true);
            var operationResult = testRepo.Delete(itemToDelete);
            Assert.AreEqual(RepositoryOperationResult.Success, operationResult);
        }

        [Test]
        public void DeleteOperationResultErrorForAbsentItem()
        {
            var itemToDelete = new object();
            ctxMock.Setup(c => c.SetEntryState<object>(itemToDelete, EntityState.Deleted)).Returns(false);
            var operationResult = testRepo.Delete(itemToDelete);
            Assert.AreEqual(RepositoryOperationResult.EntityNotFound, operationResult);
        }

        [Test]
        public void DeleteOperationNullEntityForNullArgument()
        {
            var operationResult = testRepo.Delete(null);
            Assert.AreEqual(RepositoryOperationResult.NullEntity, operationResult);
        }

        [Test]
        public void DeleteUnknownErrorWhenException()
        {
            var exceptionalDbSet = new Mock<IDbSet<object>>();
            exceptionalDbSet.Setup(set => set.Remove(new object())).Throws(new Exception("Some strange exception for testing"));
            ctxMock.Setup(ctx => ctx.Set<object>()).Returns(exceptionalDbSet.Object);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
            var itemToDelete = new object();
            exceptionalDbSet.Object.Add(itemToDelete);
            var operationResult = testRepo.Delete(itemToDelete);
            Assert.AreEqual(RepositoryOperationResult.UnknownError, operationResult);
        }

        [Test]
        public void DeleteDoNotDecreasesItemCountIfResultIsNotSuccess()
        {
            int countBefore = fakeDbSet.Count();
            testRepo.Delete(new object());
            testRepo.Delete(null);
            Assert.AreEqual(countBefore, fakeDbSet.Count());
        }

        #endregion

        #region DeleteAll()

        [Test]
        public void DeleteAllDecreasesItemsCountBy5()
        {
            var itemsToDelete = fakeDbSet.Take(5);
            ctxMock.Setup(c => c.SetEntryState<object>(It.IsAny<object>(), EntityState.Deleted)).Returns(true);
            int countBefore = fakeDbSet.Count();
            testRepo.DeleteAll(itemsToDelete);
            Assert.AreEqual(5, countBefore - fakeDbSet.Count());
        }

        [Test]
        public void DeleteAllDecreasesItemCountBy4WithOneOf5ErrorItem()
        {
            var itemsToDelete = fakeDbSet.Take(4).ToList();
            int countBefore = fakeDbSet.Count();
            ctxMock.Setup(c => c.SetEntryState<object>(It.IsAny<object>(), EntityState.Deleted)).Returns(true);
            itemsToDelete.Add(new object());
            testRepo.DeleteAll(itemsToDelete);
            Assert.AreEqual(4, countBefore - fakeDbSet.Count());
        }

        [Test]
        public void DeleteAllNullEntityResultForNullArgument()
        {
            var operationResult = testRepo.DeleteAll(null);
            Assert.AreEqual(RepositoryOperationResult.NullEntity, operationResult);
        }

        [Test]
        public void DeleteAllClearsTheCollection()
        {
            var allItemsInCollection = fakeDbSet.AsEnumerable();
            ctxMock.Setup(c => c.SetEntryState<object>(It.IsAny<object>(), EntityState.Deleted)).Returns(true);
            var operationResult = testRepo.DeleteAll(allItemsInCollection);
            Assert.AreEqual(0, fakeDbSet.Count());
            Assert.AreEqual(RepositoryOperationResult.Success, operationResult);
        }

        #endregion

        #region Update

        [Test]
        public void UpdateEntityNullResultForNullArgument()
        {
            var operationResult = testRepo.Update(null);
            Assert.AreEqual(RepositoryOperationResult.NullEntity, operationResult);
        }

        [Test]
        public void UpdateChangesEntityAndSavesIt()
        {
            var itemToUpdate = fakeDbSet.FirstOrDefault();
            ctxMock.Setup(c => c.SetEntryState<object>(itemToUpdate, EntityState.Modified)).Returns(true);
            var operationResult = testRepo.Update(itemToUpdate);
            Assert.AreEqual(RepositoryOperationResult.Success, operationResult);
        }

        [Test]
        public void UpdateEntityNotFoundResultForUnknownObject()
        {
            var itemToUpdate = new object();
            ctxMock.Setup(c => c.SetEntryState<object>(itemToUpdate, EntityState.Deleted)).Returns(false);
            var opertaionResult = testRepo.Update(itemToUpdate);
            Assert.AreEqual(RepositoryOperationResult.EntityNotFound, opertaionResult);
        }

        [Test]
        public void UpdateUnknownResultForException()
        {
            var itemToUpdate = fakeDbSet.Take(1).Single();
            var exceptionalDbSet = new Mock<IDbSet<object>>();
            exceptionalDbSet.Setup(set => set.Attach(itemToUpdate)).Throws(new Exception("Some strange exception for testing"));
            ctxMock.Setup(ctx => ctx.Set<object>()).Returns(exceptionalDbSet.Object);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
            var operationResult = testRepo.Update(itemToUpdate);
            Assert.AreEqual(RepositoryOperationResult.UnknownError, operationResult);
        }

        #endregion

        #region Any

        [Test]
        public void AnyReturnsTrueForFilledCollection()
        {
            var operationsResult = testRepo.Any();
            Assert.IsTrue(operationsResult);
        }

        [Test]
        public void AnyReturnsFalseForEmptyCollection()
        {
            fakeDbSet.Clear();
            var operationResult = testRepo.Any();
            Assert.IsFalse(operationResult);
        }

        [Test]
        public void AnyReturnsFalseForNullCollection()
        {
            IDbSet<object> nullDbSet = null;
            ctxMock.Setup(ctx=>ctx.Set<object>()).Returns(nullDbSet);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
            var operationResult = testRepo.Any();
            Assert.IsFalse(operationResult);
        }

        #endregion

        [TearDown]
        public void DestroyData()
        {
            fakeDbSet.Clear();
        }
    }
}
