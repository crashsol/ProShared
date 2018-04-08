using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.ContactApi.Models.Dtos;

namespace ProShare.ContactApi.Controllers
{
    public class BaseController : Controller
    {

        public readonly ILogger<BaseController> _logger;

        public BaseController(ILogger<BaseController> logger)
        {
            _logger = logger;
        }

        protected UserIdentity UserIdentity => new UserIdentity { UserId=1,Name="crashsol" };

    }
}