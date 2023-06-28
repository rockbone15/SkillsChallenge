using InterviewTest.Cache;
using InterviewTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace InterviewTest.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PeopleController : ControllerBase, IDisposable
    {
        private readonly PersonContext _context;
        private readonly ICacheService _cacheService;

        private readonly ILogger<PeopleController> _logger;

        public PeopleController(ILogger<PeopleController> logger, PersonContext context,  ICacheService cacheService) { 
            _logger = logger;
            _context = context; 
            _cacheService= cacheService;
        }



        [HttpGet]
        public async Task<ActionResult<IEnumerable<Person>>> Get(string? filter)
        {
            string cacheKey = $"Person_{filter}";
            var cacheData = _cacheService.GetData<IEnumerable<Person>>(cacheKey);
            if (cacheData != null)
            {
                return cacheData.ToList();
            }
            var expirationTime = DateTimeOffset.Now.AddMinutes(2.0);

            if (!string.IsNullOrEmpty(filter))
            {
                var _persons = await _context.People.Where(p => p.LastName.ToLower().Contains(filter.ToLower()) || p.FirstName.ToLower().Contains(filter.ToLower())).ToListAsync();
                _cacheService.SetData<IEnumerable<Person>>(cacheKey, _persons, expirationTime);
                return _persons;
            }
            var persons = await _context.People.ToListAsync();
            _cacheService.SetData<IEnumerable<Person>>(cacheKey, persons, expirationTime);
            return persons;
        }

        public void Dispose()
        {
           if(_context!=null)
            {
                _context.Dispose();
            }
        }
    }
}
