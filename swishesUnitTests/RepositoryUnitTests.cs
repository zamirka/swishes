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

        [Test]
        public void AddIncreasesItemCountByOne()
        {
            testRepo.Add(new object());
            Assert.AreEqual(11, testRepo.GetAll().Count());
        }

        [Test]
        public void AddOperationResultSeccessForNewItem()
        {
            var operationResult = testRepo.Add(new object());
            Assert.AreEqual(RepositoryOperationStatus.Success, operationResult);
        }

        [Test]
        public void AddOperationResultErrorForExistingItem()
        {
            var item = new object();
            fakeDbSet.Add(item);
            var operationResult = testRepo.Add(item);
            Assert.AreEqual(RepositoryOperationStatus.EntityAlreadyExists, operationResult);
        }

        [Test]
        public void AddOperationResultUnknownErrorIfException()
        {
            var exceptionalDbSet = new Mock<IDbSet<object>>();
            exceptionalDbSet.Setup(set => set.Add(new object())).Throws(new Exception("Some strange exception for testing"));
            ctxMock.Setup(ctx => ctx.Set<object>()).Returns(exceptionalDbSet.Object);
            testRepo = new Repository<object>(ctxMock.Object, _logger.Object);
            var operationResult = testRepo.Add(new object());
            Assert.AreEqual(RepositoryOperationStatus.UnknownError, operationResult);
        }

        [TearDown]
        public void DestroyData()
        {
            fakeDbSet.Clear();
        }
    }
}
