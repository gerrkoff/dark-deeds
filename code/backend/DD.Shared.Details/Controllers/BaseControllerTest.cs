using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.Shared.Details.Controllers;

[AllowAnonymous]
[Route("api/test")]
[ServiceFilter(typeof(TestAttribute))]
public abstract class BaseControllerTest : ControllerBase;
