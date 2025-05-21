using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using TodoWeb.Application.Dtos.UserModel;
using TodoWeb.Application.Services.Users;

namespace TodoWeb.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("Create")]
        public IActionResult Register(UserCreateViewModel user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(_userService.Post(user));

        }

        [HttpPost("Login")]
        public IActionResult Login(UserLoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //var isSuccess = _userService.UserLogin(user);
            //if (!isSuccess)
            //{
            //    return NotFound("Not Found user");
            //}
            var user = _userService.UserLogin(loginViewModel);
            if (user == null)
            {
                return NotFound("Username or password is wrong");
            }
            HttpContext.Session.SetInt32("UserId", user.Id);
            HttpContext.Session.SetString("Role", user.Role.ToString());
            //{key: session, value: {UserId, Role}}
            return Ok("Login success");

        }

        [HttpPost("login-cookies")]
        public async Task<IActionResult> LoginCookies(UserLoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userService.UserLogin(loginViewModel);
            if (user == null)
            {
                return NotFound("Username or password is wrong");
            }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.EmailAddress),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return Ok("Login successfully!");
        }

        [HttpPost("login-jwt")]
        public async Task<IActionResult> LoginJwt(UserLoginViewModel loginViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = _userService.UserLogin(loginViewModel);
            if (user == null)
            {
                return NotFound("Username or password is wrong");
            }
            var token = _userService.GenerateJwt(user);
            return Ok(token);
        }

        [HttpPost("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return Ok();
        }
    }
}
