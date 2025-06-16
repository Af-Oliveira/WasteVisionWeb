using Microsoft.AspNetCore.Mvc;
using DDDSample1.Domain.Shared;
using DDDSample1.Domain.Roles;
using System.Threading.Tasks;
using DDDSample1.Application.Shared;
using System.Collections.Generic;

namespace DDDSample1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET: api/role
        [HttpGet]
        public async Task<ActionResult> GetAllRoles([FromQuery] RoleSearchParamsDto searchParams)
        {
            try
            {
                var roles = await _roleService.GetAllWithFiltersAsync(searchParams);
                return ApiResponse.For(roles).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<List<RoleDto>>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // GET api/role/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult> GetRole(string id)
        {
            try
            {
                var role = await _roleService.GetByIdAsync(new RoleId(id));
                if (role == null)
                {
                    return ApiResponse.For<RoleDto>().AsError().WithMessage($"Role with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }
                return ApiResponse.For(role).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoleDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // POST api/role
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreatingRoleDto dto)
        {
            try
            {
                var result = await _roleService.AddAsync(dto);
                return ApiResponse.For(result).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoleDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // PUT api/role/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateRole(string id, [FromBody] RoleDto dto)
        {
            try
            {
                var result = await _roleService.UpdateAsync(id, dto);
                if (result == null)
                {
                    return ApiResponse.For<RoleDto>().AsError().WithMessage($"Role with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }
                return ApiResponse.For(result).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<RoleDto>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // DELETE api/role/soft/5
        [HttpDelete("soft/{id}")]
        public async Task<ActionResult> DeactivateRole(string id)
        {
            try
            {
                var result = await _roleService.DeactivateAsync(new RoleId(id));

                if (!result)
                {
                    return ApiResponse.For<bool>().AsError().WithMessage($"Role with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For<bool>().WithData(true).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }


        // PATCH api/role/activate/5
        [HttpPatch("activate/{id}")]
        public async Task<ActionResult> ActivateRole(string id)
        {
            try
            {
                var result = await _roleService.ActivateAsync(new RoleId(id));

                if (!result)
                {
                    return ApiResponse.For<bool>().AsError().WithMessage($"Role with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For<bool>().WithData(true).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }

        // DELETE api/role/hard/5
        [HttpDelete("hard/{id}")]

        public async Task<ActionResult> DeleteRole(string id)
        {
            try
            {
                var result = await _roleService.DeleteAsync(new RoleId(id));
                if (!result)
                {
                    return ApiResponse.For<bool>().AsError().WithMessage($"Role with ID {id} not found.").Build(StatusCodeEnum.NotFoundError);
                }

                return ApiResponse.For<bool>().WithData(true).Build(StatusCodeEnum.Success);
            }
            catch (BusinessRuleValidationException ex)
            {
                return ApiResponse.For<bool>().AsError().WithMessage(ex.Message).Build(StatusCodeEnum.BadRequestError);
            }
        }
    }
}
