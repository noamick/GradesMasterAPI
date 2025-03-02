using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected int GetUserId()
        {
            return int.Parse(this.User.Claims.First(i => i.Type == "UserID").Value);
        }
    }
}
