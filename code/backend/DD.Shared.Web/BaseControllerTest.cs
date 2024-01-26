using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DD.Shared.Web;

[AllowAnonymous]
[Route("api/test")]
[ServiceFilter(typeof(TestAttribute))]
public abstract class BaseControllerTest : ControllerBase;
