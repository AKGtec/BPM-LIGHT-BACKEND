using Microsoft.AspNetCore.Mvc;
using ProjectTemplate.Service.Contracts;

namespace ProjectTemplate.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    protected readonly IServiceManager _serviceManager;

    public BaseApiController(IServiceManager serviceManager)
        => _serviceManager = serviceManager;
}
