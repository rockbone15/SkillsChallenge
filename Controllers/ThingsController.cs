using InterviewTest.Cache;
using InterviewTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThingsController : ControllerBase, IDisposable
    {
      

        private readonly ILogger<ThingsController> _logger;
        private readonly ThingContext _context;
        private readonly ICacheService _cacheService;

        public ThingsController(ILogger<ThingsController> logger, ThingContext context, ICacheService cacheService)
        {
            _logger = logger;
            _context = context;
            _cacheService = cacheService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Thing>>> Get(string? filter)
        {
            string cacheKey = $"Things_{filter}";
            var cacheData = _cacheService.GetData<IEnumerable<Thing>>(cacheKey);
            if (cacheData != null)
            {
                return cacheData.ToList();
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(2.0);
            if (!string.IsNullOrEmpty(filter))
            {
                var _things = await _context.Things.Where(p => p.Name.ToLower().Contains(filter.ToLower()) || p.Description.ToLower().Contains(filter.ToLower())).ToListAsync();
                _cacheService.SetData<IEnumerable<Thing>>(cacheKey, _things, expirationTime);
                return _things;
            }

            var things =await _context.Things.ToListAsync();
            _cacheService.SetData<IEnumerable<Thing>>(cacheKey, things, expirationTime);
            return things;
        }
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
