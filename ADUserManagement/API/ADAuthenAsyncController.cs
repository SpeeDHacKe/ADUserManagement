using ADUserManagement.Models;
using ADUserManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADUserManagement.API
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ADAuthenAsyncController : ControllerBase
    {
        private readonly UserManagerService _userManagerService;
        public ADAuthenAsyncController(UserManagerService userManagerService) 
        {
            _userManagerService = userManagerService;
        }

        [HttpGet("LockUserAsync", Name = "LockUserAsync")]
        public async Task<IActionResult> LockUserAsync(string username, string domainName = "dev.com")
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                returnResponseModel = await _userManagerService.LockUserAsync(username, domainName);
                return Ok(returnResponseModel);
            }
            catch (Exception ex)
            {
                returnResponseModel.error = ex;
                returnResponseModel.message = $"{ex.Message} - {ex.InnerException}";
                return BadRequest(returnResponseModel);
            }
        }

        [HttpGet("UnlockUserAsync", Name = "UnlockUserAsync")]
        public async Task<IActionResult> UnlockUser(string username, string domainName = "dev.com")
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                returnResponseModel = await _userManagerService.UnlockUserAsync(username, domainName);
                return Ok(returnResponseModel);
            }
            catch (Exception ex)
            {
                returnResponseModel.error = ex;
                returnResponseModel.message = $"{ex.Message} - {ex.InnerException}";
                return BadRequest(returnResponseModel);
            }
        }

        [HttpGet("ChangePasswordAsync", Name = "ChangePasswordAsync")]
        public async Task<IActionResult> ChangePasswordAsync(string username, string newPassword, string domainName = "dev.com")
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                returnResponseModel = await _userManagerService.ChangePasswordAsync(username, domainName, newPassword);
                return Ok(returnResponseModel);
            }
            catch (Exception ex)
            {
                returnResponseModel.error = ex;
                returnResponseModel.message = $"{ex.Message} - {ex.InnerException}";
                return BadRequest(returnResponseModel);
            }
        }
    }
}
