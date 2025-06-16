using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.Users;
using DDDSample1.Domain.Shared;
using DDDSample1.Application.Shared;
using System;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/user
        [HttpGet]
        public async Task<ActionResult> GetAllUsers([FromQuery] UserSearchParamsDto searchParams)
        {
            try
            {
               
                var userList = await _userService.GetAllWithFiltersAsync(searchParams);
                return ApiResponse.For(userList).WithMessage("Users retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<IEnumerable<UserApiDto>>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // POST api/user
        [HttpPost]
        public async Task<ActionResult> CreateUser([FromBody] CreatingUserDto dto)
        {
            try
            {
                var result = await _userService.CreateAsync(dto);
                return ApiResponse.For(result).WithMessage("User created successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<UserApiDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // GET api/user/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetUser(string id)
        {
            try
            {
                var user = await _userService.GetByIdAsync(new UserId(id));
                if (user == null)
                {

                    return ApiResponse.For<UserApiDto>().AsError().WithMessage($"User with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(user).WithMessage("User retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<UserApiDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // GET api/user/email/5
        [HttpGet("email/{email}")]
        public async Task<ActionResult> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userService.GetByEmailAsync(email);
                if (user == null)
                {
                    return ApiResponse.For<UserApiDto>().AsError().WithMessage($"User with email {email} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(user).WithMessage("User retrieved successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<UserApiDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }



        // PUT api/user/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateUser(string id, [FromBody] UpdatingUserDto dto)
        {
            try
            {
                var updatedUser = await _userService.UpdateAsync(id, dto);

                if (updatedUser == null)
                {
                    return ApiResponse.For<UserApiDto>().AsError().WithMessage($"User with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For(updatedUser).WithMessage("User updated successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<UserApiDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // DELETE api/user/hard/5
        [HttpDelete("hard/{id}")]
        public async Task<ActionResult> DeleteUser(string id)
        {
            try
            {
                var user = await _userService.DeleteAsync(new UserId(id));
                if (!user)
                {
                    return ApiResponse.For(user).AsError().WithMessage($"User with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }
                return ApiResponse.For(user).WithMessage("User deleted successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<UserApiDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // PATCH api/user/activate/5
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult> ActivateUser(string id)
        {
            try
            {
                var user = await _userService.ActivateUserAsync(id);

                if (!user)
                {
                    return ApiResponse.For<bool>().AsError().WithMessage($"User with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }
                return ApiResponse.For(user).WithMessage("User activated successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        //DELETE api/user/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<ActionResult> DeactivateUser(string id)
        {
            try
            {
                var user = await _userService.DeactivateUserAsync(id);

                if (!user)
                {

                    return ApiResponse.For<bool>().AsError().WithMessage($"User with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }
                return ApiResponse.For(user).WithMessage("User deactivated successfully.").Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

    }
}
