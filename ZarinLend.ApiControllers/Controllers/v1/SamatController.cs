using Asp.Versioning;
using Microsoft.Extensions.Logging;
using Services;
using WebFramework.Api;

namespace Web.ApiControllers.Controllers.v1
{
    [ApiVersion("1")]
    public class SamatController : BaseApiController
    {
        private readonly ILogger<SamatController> logger;
        private readonly ISamatService samatService;

        public SamatController(ILogger<SamatController> logger, ISamatService samatService)
        {
            this.logger = logger;
            this.samatService = samatService;
        }

    }
}
