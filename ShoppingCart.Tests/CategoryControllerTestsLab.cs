using Moq;
using ShoppingCart.DataAccess.Repositories;
using ShoppingCart.DataAccess.ViewModels;
using ShoppingCart.Models;
using ShoppingCart.Tests.Datasets;
using ShoppingCart.Web.Areas.Admin.Controllers;
using Xunit;


namespace ShoppingCart.Tests
{
    public class CategoryControllerTestsLab
    {
        [Fact]
        public void GetCategories_All_ReturnAllCategories()
        {
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();

            repositoryMock.Setup(r => r.GetAll(It.IsAny<string>()))
                .Returns(() => CategoryDataset.Categories);
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            var result = controller.Get();

            Assert.Equal(CategoryDataset.Categories, result.Categories);
        }

        [Fact]
        public void CreateUpdate_WithValidModel_CreatesOrUpdateCategory()
        {
            var category = new Category { Id = 0, Name = "Category 1" };
            var viewModel = new CategoryVM { Category = category };
            var mockRepository = new Mock<ICategoryRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(mockRepository.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            controller.CreateUpdate(viewModel);

            mockRepository.Verify(repo => repo.Add(category), Times.Once);
            mockRepository.Verify(repo => repo.Update(category), Times.Never);
            mockUnitOfWork.Verify(uow => uow.Save(), Times.Once);
        }

        [Fact]
        public void CreateUpdate_WithInvalidModel_ThrowsException()
        {
            var viewModel = new CategoryVM { Category = null };
            var mockRepository = new Mock<ICategoryRepository>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(mockRepository.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            var exception = Assert.Throws<NullReferenceException>(() => controller.CreateUpdate(viewModel));
        }

        [Fact]
        public void DeleteCategory_NonexistentId_ThrowsException()
        {
            int nonExistentId = 12;
            Mock<ICategoryRepository> repositoryMock = new Mock<ICategoryRepository>();
            repositoryMock.Setup(r => r.Delete(null))
                .Throws<Exception>();
            var mockUnitOfWork = new Mock<IUnitOfWork>();
            mockUnitOfWork.Setup(uow => uow.Category).Returns(repositoryMock.Object);
            var controller = new CategoryController(mockUnitOfWork.Object);

            Assert.Throws<Exception>(() => controller.DeleteData(nonExistentId));
        }
    }
}