using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ProShare.RecommendApi.Controllers
{
    [Route("api/[controller]")]
    public class RecommendsController : BaseController
    {
    
        public RecommendsController(ILogger<RecommendsController> logger):base(logger)
        {
          
        }

    }
}
