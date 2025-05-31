using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudyBuddyAPI.DTOs;
using StudyBuddyAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace StudyBuddyAPI.Controllers
{
    /// <summary>
    /// API Controller for managing users
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Authenticates a user and returns a JWT token
        /// </summary>
        /// <param name="dto">The login credentials</param>
        /// <returns>JWT token for authenticated user</returns>
        /// <response code="200">Returns the JWT token</response>
        /// <response code="401">If the credentials are invalid</response>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody][Required] LoginDto dto)
        {
            var (isSuccess, tokenOrError, userId) = await _userService.LoginAsync(dto);
            if (!isSuccess) return Unauthorized(tokenOrError);
            return Ok(new { Token = tokenOrError, UserId = userId });
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="dto">The user registration data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the user was registered successfully</response>
        /// <response code="400">If the registration data is invalid</response>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateUser([FromBody][Required] RegisterUserDto dto)
        {
            var (isSuccess, error) = await _userService.CreateUserAsync(dto);
            if (!isSuccess)
                return BadRequest(error);
            return Ok("User created successfully.");
        }

        /// <summary>
        /// Gets all users
        /// </summary>
        /// <returns>A list of all users</returns>
        /// <response code="200">Returns the list of users</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users.Select(u => new {
                u.Id,
                u.UserName,
                u.FullName,
                u.Email,
                u.Role
            }));
        }

        /// <summary>
        /// Gets a specific user by id
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>The requested user</returns>
        /// <response code="200">Returns the user</response>
        /// <response code="404">If the user is not found</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpGet("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUser([Required] string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound("User not found.");
            return Ok(user);
        }

        /// <summary>
        /// Updates an existing user
        /// </summary>
        /// <param name="id">The user id</param>
        /// <param name="dto">The updated user data</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the user was updated successfully</response>
        /// <response code="400">If the request data is invalid or user doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([Required] string id, [FromBody][Required] RegisterUserDto dto)
        {
            var (isSuccess, error) = await _userService.UpdateUserAsync(id, dto);
            if (!isSuccess) return BadRequest(error);
            return Ok("User updated successfully.");
        }

        /// <summary>
        /// Deletes a user
        /// </summary>
        /// <param name="id">The user id</param>
        /// <returns>A success message</returns>
        /// <response code="200">If the user was deleted successfully</response>
        /// <response code="400">If the user doesn't exist</response>
        /// <response code="401">If the user is not authorized</response>
        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteUser([Required] string id)
        {
            var (isSuccess, error) = await _userService.DeleteUserAsync(id);
            if (!isSuccess) return BadRequest(error);
            return Ok("User deleted successfully.");
        }
    }
}