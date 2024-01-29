using Microsoft.AspNetCore.Mvc;

namespace StrawberryHub.Controllers;

[Route("api/Data")]
[ApiController]
public class DataController : ControllerBase
{
    private readonly IDbService _dbSvc;
    private readonly IAuthService _authSvc;

    public DataController(IAuthService authSvc,
                               IDbService dbSvc)
    {
        _dbSvc = dbSvc;
        _authSvc = authSvc;
    }

    // GET api/<ValuesController>/5
    [HttpGet("login/{username}/{password}")]
    public IActionResult Login(string username, string password)
    {
        const string sqlLogin =
           @"SELECT Username, FirstName FROM User 
               WHERE Username = '{0}' 
                 AND Password1 = HASHBYTES('SHA1', '{1}')";

        if (!_authSvc.Authenticate(sqlLogin, username, password,
                                    out ClaimsPrincipal? principal))
        {
            return BadRequest("Invalid ID or Password");
        }
        else
        {

            StrawberryUser? user = _dbSvc
                .GetList<StrawberryUser>(
                    @"SELECT UserId, FirstName, LastName, Email, 
                         FROM User
                        WHERE Username='{0}'", username)
                .FirstOrDefault();

            return Ok(user);
        }
    }

    // GET: api/<ValuesController>
    [HttpGet("users")]
    public IEnumerable<string> Get()
    {
        return new string[] { "daniel", "chuen", "aqidah", "guanyi" };
    }


}
