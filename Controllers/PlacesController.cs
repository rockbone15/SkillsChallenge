using InterviewTest.Cache;
using InterviewTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InterviewTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlacesController : ControllerBase, IDisposable
    {
       private readonly PlaceContext _context;

        private readonly ILogger<PlacesController> _logger;
        private readonly ICacheService _cacheService;

        public PlacesController(ILogger<PlacesController> logger, PlaceContext context, ICacheService cacheService)
        {
            _logger = logger;
            _context = context;
            _cacheService = cacheService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Place>>> Get(string? filter)
        {
            string cacheKey = $"Place_{filter}";
            var cacheData = _cacheService.GetData<IEnumerable<Place>>(cacheKey);
            if (cacheData != null)
            {
                return cacheData.ToList();
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(2.0);

            if (!string.IsNullOrEmpty(filter))
            {
                var _places= await _context.Places.Where(p => p.Name.ToLower().Contains(filter.ToLower()) || p.City.ToLower().Contains(filter.ToLower()) || p.State.ToLower().Contains(filter.ToLower())).ToListAsync();
                _cacheService.SetData<IEnumerable<Place>>(cacheKey, _places, expirationTime);
                return _places;
            }

            var places= await _context.Places.ToListAsync();
            _cacheService.SetData<IEnumerable<Place>>(cacheKey, places, expirationTime);
            return places;
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
