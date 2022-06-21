using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using HotelListing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using AutoMapper;
using HotelListingAPI.HotelListing.Models;
using Microsoft.AspNetCore.Http;
using HotelListingAPI.HotelListing.Services;

namespace HotelListing.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApiUser> _userManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IMapper _mapper;
        private readonly IAuthManager _authManager;

        public AccountController(
            UserManager<ApiUser> userManager,
            ILogger<AccountController> logger,
            IMapper mapper,
            IAuthManager authManager
        )
        {
            _userManager = userManager;
            _logger = logger;
            _mapper = mapper;
            _authManager = authManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
        {
            _logger.LogInformation($"Registering attemp for {userDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var user = _mapper.Map<ApiUser>(userDTO);
                user.UserName = userDTO.Email;
                var result = await _userManager.CreateAsync(user, userDTO.Password);
                if (!result.Succeeded)
                {
                    result.Errors.ToList().ForEach(e => ModelState.AddModelError(e.Code, e.Description));
                    return BadRequest(ModelState);
                }
                _userManager.AddToRolesAsync(user, userDTO.Roles).Wait();
                return Accepted();
            }
            catch (Exception ex)
            {
                var errorMessage = $"Something went wrong in the {nameof(Register)}";
                _logger.LogError(ex, errorMessage);
                return Problem( errorMessage, statusCode: 500);
            }

        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            _logger.LogInformation($"Login attemp for {loginUserDTO.Email}");
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var result = await _authManager.ValidateUser(loginUserDTO);
                if (!result)
                {
                    return Unauthorized();
                }
                var token = await _authManager.CreateToken();

                return Accepted(new { token });
            }
            catch (Exception ex)
            {
                var errorMessage = $"Something went wrong in the {nameof(Login)}";
                _logger.LogError(ex, errorMessage);
                return Problem( errorMessage, statusCode: 500);
            }
        }
    }
}