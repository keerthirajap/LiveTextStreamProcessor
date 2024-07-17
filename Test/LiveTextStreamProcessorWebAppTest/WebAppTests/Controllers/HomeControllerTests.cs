namespace LiveTextStreamProcessorTest.WebApp.Controllers
{
    using LiveTextStreamProcessorWebApp.Controllers;
    using LiveTextStreamProcessorWebApp.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Moq;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    [TestClass]
    [ExcludeFromCodeCoverage]
    public class HomeControllerTests
    {
        private Mock<ILogger<HomeController>> _loggerMock;
        private Mock<IHttpContextAccessor> _httpContextAccessorMock;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<HomeController>>();
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        }

        [TestMethod]
        public void HomeController_Index_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_loggerMock.Object, null); // Pass null for IHttpContextAccessor, not used in Index

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void HomeController_Privacy_ReturnsViewResult()
        {
            // Arrange
            var controller = new HomeController(_loggerMock.Object, null); // Pass null for IHttpContextAccessor, not used in Privacy

            // Act
            var result = controller.Privacy() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void HomeController_Error_ReturnsViewResultWithModel()
        {
            // Arrange
            var controller = new HomeController(_loggerMock.Object, _httpContextAccessorMock.Object);

            // Mock HttpContextAccessor
            var httpContext = new DefaultHttpContext();
            httpContext.TraceIdentifier = "mock-trace-id"; // Set a mock trace identifier
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

            // Act
            var result = controller.Error() as ViewResult;
            var model = result?.Model as ErrorViewModel;

            // Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
            Assert.IsNotNull(model);
            Assert.AreEqual("mock-trace-id", model.RequestId); // Verify the requestId matches the mocked value
        }
    }
}