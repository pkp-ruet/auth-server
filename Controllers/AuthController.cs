using AuthServer.Models;
using AuthServer.Repositories;
using AuthServer.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IHashService _hashService;
        private readonly AccessTokenGenerator _tokenGenerator;

        public AuthController(IUserRepository userRepository, IHashService hashService, AccessTokenGenerator tokenGenerator)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]

        public IActionResult Register([FromBody] RegisterRequest registerRequest)
        {
            if (!ModelState.IsValid)
            {
                return ReturnBadRequestResponse();
            }
            if (registerRequest.Password != registerRequest.ConfirmPassword)
            {
                return BadRequest(new ErrorResponse("Password not matched!"));
            }

            if ((_userRepository.GetUserByEmail(registerRequest.Email)) != null)
            {
                return Conflict(new ErrorResponse("Email already exists!"));
            }
            if ((_userRepository.GetUserByUsername(registerRequest.UserName)) != null)
            {
                return Conflict(new ErrorResponse("Username already exists!"));
            }

            var passwordHash = _hashService.GetPasswordHash(registerRequest.Password);
            var user = new User
            {
                Username = registerRequest.UserName,
                Email = registerRequest.Email,
                PasswordHash = passwordHash,
                Id = Guid.NewGuid().ToString()
            };
            _userRepository.CreateUser(user);

            return Ok(user);
        }

        

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest loginRequest)
        {
            if (!ModelState.IsValid)
            {
                ReturnBadRequestResponse();
            }

            var user = _userRepository.GetUserByUsername(loginRequest.Username);
            if (user == null)
            {
                return Unauthorized();
            }

            if (!_hashService.VerifyPasswordHash(loginRequest.Password, user.PasswordHash))
            {
                return Unauthorized();
            }

            string accessToken = _tokenGenerator.GenerateToken(user);
            return Ok(new AuthenticatedUserResponse() {AccessToken = accessToken});
        }

        private IActionResult ReturnBadRequestResponse()
        {
            IEnumerable<string> errorMessages =
                ModelState.Values.SelectMany(o => o.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errorMessages));
        }
    }
}
