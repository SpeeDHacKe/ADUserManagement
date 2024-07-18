using ADUserManagement.Models;
using ADUserManagement.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ADUserManagement.API
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class ADAuthenController : ControllerBase
    {
        private readonly UserManagerService _userManagerService;
        public ADAuthenController(UserManagerService userManagerService) 
        {
            _userManagerService = userManagerService;
        }

        [HttpGet("LockUser", Name = "LockUser")]
        public IActionResult LockUser(string username)
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                _userManagerService.LockUser(username, "dev.com");
                returnResponseModel.success = true;
                returnResponseModel.status = 200;
                return Ok(returnResponseModel);
            }
            catch (Exception ex)
            {
                returnResponseModel.error = ex;
                returnResponseModel.message = $"{ex.Message} - {ex.InnerException}";
                return BadRequest(returnResponseModel);
            }
        }

        [HttpGet("UnlockUser", Name = "UnlockUser")]
        public IActionResult UnlockUser(string username)
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                _userManagerService.UnlockUser(username, "dev.com");
                returnResponseModel.success = true;
                returnResponseModel.status = 200;
                return Ok(returnResponseModel);
            }
            catch (Exception ex)
            {
                returnResponseModel.error = ex;
                returnResponseModel.message = $"{ex.Message} - {ex.InnerException}";
                return BadRequest(returnResponseModel);
            }
        }

        [HttpGet("ChangePassword", Name = "ChangePassword")]
        public IActionResult ChangePassword(string username, string newPassword)
        {
            ResponseModel returnResponseModel = new ResponseModel();
            try
            {
                _userManagerService.ChangePassword(username, "dev.com", newPassword);
                returnResponseModel.success = true;
                returnResponseModel.status = 200;
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
