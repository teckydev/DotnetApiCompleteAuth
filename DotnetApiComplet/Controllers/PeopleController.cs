using Microsoft.AspNetCore.Mvc;

namespace DotnetApiComplet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PeopleController : ControllerBase
    {   [HttpGet]
        public IActionResult GetPeople()
        {
           return Ok("people data") ;
        }
         [HttpPost]
        public IActionResult CreatePerson()
        {
           return Ok("people data") ;
        }
    }
}
