using Humanizer;
using Microsoft.AspNetCore.Mvc;
using System.Text.Unicode;
using System.Text;

namespace StrawberryHub.Controllers;

public class AccountController : Controller
{
    private const string REDIRECT_CNTR = "Home";
    private const string REDIRECT_ACTN = "Index";
    private const string LOGIN_VIEW = "Login";

    private readonly AppDbContext _dbCtx;
    private readonly IAuthService _authSvc;

    public AccountController(IAuthService authSvc, AppDbContext dbContext)
    {
        _dbCtx = dbContext;
        _authSvc = authSvc;
    }

    private bool AuthenticateUser(string uid, string pw, out ClaimsPrincipal? principal)
    {
        principal = null;

        var sql = String.Format(
            @"SELECT * FROM StrawberryUser 
               WHERE Username = '{0}' 
                 AND Password = HASHBYTES('SHA1', '{1}')",
            uid.EscQuote(),  // prevent SQL Injection 
            pw.EscQuote());  // prevent SQL Injection
        StrawberryUser? appUser = _dbCtx.StrawberryUser
            .FromSqlRaw(sql)
            .FirstOrDefault();

        if (appUser != null)
        {
            principal =
               new ClaimsPrincipal(
                  new ClaimsIdentity(
                     new Claim[] {
                        new Claim(ClaimTypes.NameIdentifier, appUser.Username),
                        new Claim(ClaimTypes.Name, appUser.FirstName),
                        new Claim(ClaimTypes.Role, appUser.UserRole)
                     },
                     "Basic"
                  )
               );
            return true;
        }
        return false;
    }

    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        TempData["ReturnUrl"] = returnUrl;
        return View(LOGIN_VIEW);
    }

    [AllowAnonymous]
    [HttpPost]
    public IActionResult Login(LoginUser user)
    {
        const string sqlLogin =
                @"SELECT UserId, Username, UserRole FROM StrawberryUser WHERE Username = '{0}' 
                 AND Password = HASHBYTES('SHA1', '{1}')";

        // Convert byte array to string using UTF-8 encoding
        //string thePassword = Encoding.UTF8.GetString(user.Password);
        if (!_authSvc.Authenticate(sqlLogin, user.Username, user.Password,
                out ClaimsPrincipal? principal)) //changes
        {
            ViewData["Message"] = "Incorrect User Id or Password";
            ViewData["MsgType"] = "warning";
            return View(LOGIN_VIEW, user);
        }
        else
        {
            HttpContext.SignInAsync(
               CookieAuthenticationDefaults.AuthenticationScheme,
               principal!,
               new AuthenticationProperties
               {
                   IsPersistent = true
               });


            string? returnUrl = TempData["returnUrl"]?.ToString();
            if (returnUrl != null && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
        }
    }

    [Authorize]
    public IActionResult Logoff(string? returnUrl = null)
    {
        HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        if (Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
        return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
    }

    [AllowAnonymous]
    public IActionResult Forbidden()
    {
        return View();
    }

    [Authorize]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public IActionResult ChangePassword(PasswordDTO pwd)
    {
        var userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        if (_dbCtx.Database.ExecuteSqlInterpolated(
              $@"UPDATE StrawberryUser 
                    SET Password = 
                        HASHBYTES('SHA1', CONVERT(VARCHAR, {pwd.NewPwd})) 
                  WHERE Username = {userid} 
                    AND Password = 
                         HASHBYTES('SHA1', CONVERT(VARCHAR, {pwd.CurrentPwd}))"
                ) == 1)
            ViewData["Msg"] = "Password Updated";
        else
            ViewData["Msg"] = "Failed to Update Password!";
        return View();
    }

    [Authorize]
    public JsonResult VerifyCurrentPassword(string CurrentPwd)
    {
        var userid = 
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        StrawberryUser? user = _dbCtx.StrawberryUser
            .FromSqlInterpolated(
                $@"SELECT * FROM StrawberryUser 
                    WHERE Username = {userid} 
                      AND Password = HASHBYTES('SHA1', 
                          CONVERT(VARCHAR, {CurrentPwd}))")
            .FirstOrDefault();

        if (user != null)          // User's Password Found
            return Json(true);     // Current Password Valid
        else
            return Json(false);    // Current Password Invalid
    }

    [Authorize]
    public JsonResult VerifyNewPassword(string NewPwd)
    {
        var userid = 
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        StrawberryUser? user = _dbCtx.StrawberryUser
            .FromSqlInterpolated(
                $@"SELECT * FROM User 
                    WHERE Username = {userid} 
                      AND Password = HASHBYTES('SHA1', 
                          CONVERT(VARCHAR, {NewPwd}))")
            .FirstOrDefault();

        // New Password cannot be the same as Current Password
        if (user == null)        // User's Password Not Found
            return Json(true);   // New Password Valid
        else
            return Json(false);  // New Password Invalid
    }

    [Authorize]
    public JsonResult VerifyNewUsername(string NewUname)
    {
        DbSet<StrawberryUser> dbs = _dbCtx.StrawberryUser;

        StrawberryUser? user = dbs
            .FromSqlInterpolated(
                $"SELECT * FROM User WHERE Username = {NewUname}")
            .FirstOrDefault();

        if (user == null)
            return Json(true);
        else
            return Json(false);
    }

    public IActionResult ChangeUsername()
    {
        ViewData["userid"] =
            User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        return View();
    }

    [HttpPost]
    public IActionResult ChangeUsername(UsernameDTO uu)
    {
        var userid = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        var theNewUsername = uu.NewUname;

        if (_dbCtx.Database.ExecuteSqlInterpolated(
            @$"UPDATE AppUser 
                  SET Id={theNewUsername} 
                WHERE Id={userid}") == 1)
            ViewData["Msg"] = "Username successfully updated!";
        else
            ViewData["Msg"] = "Failed to update username!";

        HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(REDIRECT_ACTN, REDIRECT_CNTR);
    }


}
