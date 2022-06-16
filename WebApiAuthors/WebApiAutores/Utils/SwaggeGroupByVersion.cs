using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace WebApiAuthors.Utils
{
    public class SwaggeGroupByVersion : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var namespaceController = controller.ControllerType.Namespace; // nombre del controlador
            var APIVersion = namespaceController.Split('.').Last().ToLower(); // sustraemos la version y la volvemos en minuscula
            controller.ApiExplorer.GroupName = APIVersion;
        }
    }
}
