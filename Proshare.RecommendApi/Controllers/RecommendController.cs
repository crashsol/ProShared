using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Proshare.RecommendApi.Data;
using Microsoft.EntityFrameworkCore;

namespace ProShare.RecommendApi.Controllers
{
    [Route("api/Recommends")]
    public class RecommendController : BaseController
    {
        private readonly RecommendDbContext _recommendDbContext;
    
        public RecommendController(RecommendDbContext recommendDbContext ,ILogger<RecommendController> logger):base(logger)
        {
            _recommendDbContext = recommendDbContext;
        }

        [HttpGet]
        [Route("")]
        public async Task<IActionResult> GetAsync()
        {
           return Ok(await _recommendDbContext.ProjectRecommends.AsNoTracking()
                    .Where(b => b.UserId == UserIdentity.UserId).ToListAsync());
        }

    }
}
