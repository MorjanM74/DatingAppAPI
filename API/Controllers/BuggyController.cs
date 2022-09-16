using API.Data;
using API.Entites;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly ApplicationDbContext _dbContext;

    // GET
    public BuggyController(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
    {
        return "secret text";
    }
    
    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = _dbContext.Users.Find(-1);

        if (thing == null)
        {
            return NotFound();
        }

        return Ok("thing");
    }
    
    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _dbContext.Users.Find(-1);

        var thingToReturn = thing.ToString();
        return thingToReturn;
    }
    
    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("this was not a good request");
    }
}