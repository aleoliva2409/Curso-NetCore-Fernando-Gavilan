using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using WebApiAuthors.Controllers.V1;
using WebApiAuthors.Tests.Mocks;

namespace WebApiAuthors.Tests.UnitTests
{
    [TestClass]
    public class RootControllerTests
    {
        [TestMethod]
        public async Task IfUserIsAdmin_Get4Links()
        {
            // Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Success();
            var rootController = new RootController(authorizationService);
            // reemplazamos el Url de rootController
            rootController.Url = new URLHelperMock();

            // Ejecution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(result.Value.Count(), 4);
        }

        [TestMethod]
        public async Task UserIsNotAdmin_Get2Links()
        {
            // Preparation
            var authorizationService = new AuthorizationServiceMock();
            authorizationService.Result = AuthorizationResult.Failed();
            var rootController = new RootController(authorizationService);
            // reemplazamos el Url de rootController
            rootController.Url = new URLHelperMock();

            // Ejecution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(result.Value.Count(), 2);
        }

        [TestMethod]
        public async Task UserIsNotAdmin_Get2Links_UsingMoqLibrary()
        {
            // Preparation
            var mockAuthorizationService = new Mock<IAuthorizationService>();
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<IEnumerable<IAuthorizationRequirement>>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            // seteamos el metodo y sus args que vamos a usar en el test
            mockAuthorizationService.Setup(x => x.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<object>(),
                It.IsAny<string>()
                )).Returns(Task.FromResult(AuthorizationResult.Failed()));

            var mockURLHelper = new Mock<IUrlHelper>();
            mockURLHelper.Setup(x => x.Link(
                It.IsAny<string>(),
                It.IsAny<object>()
                )).Returns("MOCK");

            var rootController = new RootController(mockAuthorizationService.Object);
            // reemplazamos el Url de rootController
            rootController.Url = mockURLHelper.Object;

            // Ejecution
            var result = await rootController.Get();

            // Verification
            Assert.AreEqual(result.Value.Count(), 2);
        }
    }
}
