using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProShare.UserApi.Models.Dtos;

namespace ProShare.UserApi.Controllers
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