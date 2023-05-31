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
        private readonly TokenGenerator _tokenGenerator;
        private readonly RefreshTokenValidator _refreshTokenValidator;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public AuthController(IUserRepository userRepository, IHashService hashService, TokenGenerator tokenGenerator, RefreshTokenValidator refreshTokenValidator, IRefreshTokenRepository refreshTokenRepository)
        {
            _userRepository = userRepository;
            _hashService = hashService;
            _tokenGenerator = tokenGenerator;
            _refreshTokenValidator = refreshTokenValidator;
            _refreshTokenRepository = refreshTokenRepository;
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

            var response = GetAuthenticatedUserResponse(user);
            return Ok(response);
        }

        private AuthenticatedUserResponse GetAuthenticatedUserResponse(User user)
        {
            string accessToken = _tokenGenerator.GenerateAccessToken(user);
            string refreshToken = _tokenGenerator.GenerateRefreshToken();
            RefreshToken refreshTokenDTO = new RefreshToken()
            {
                Token = refreshToken,
                UserId = user.Id
            };
            _refreshTokenRepository.Create(refreshTokenDTO);
            return new AuthenticatedUserResponse()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        [HttpPost("refresh")]

        public IActionResult Refresh([FromBody] RefreshRequest refreshRequest)
        {
            if (!ModelState.IsValid)
            {
                ReturnBadRequestResponse();
            }

            if (!_refreshTokenValidator.Validate(refreshRequest.RefreshToken))
            {
                return BadRequest(new ErrorResponse("Invalid refresh token"));
            }

            RefreshToken refreshTokenDTO = _refreshTokenRepository.GetByToken(refreshRequest.RefreshToken);
            if (refreshTokenDTO == null)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token"));
            }

            var user = _userRepository.GetUserById(refreshTokenDTO.UserId);
            if (user == null)
            {
                return BadRequest(new ErrorResponse("User not found"));
            }
            _refreshTokenRepository.Delete(refreshTokenDTO.Id);
            var response = GetAuthenticatedUserResponse(user);
            return Ok(response);

        }

        private IActionResult ReturnBadRequestResponse()
        {
            IEnumerable<string> errorMessages =
                ModelState.Values.SelectMany(o => o.Errors.Select(e => e.ErrorMessage));
            return BadRequest(new ErrorResponse(errorMessages));
        }
    }
}
