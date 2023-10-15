using ESMS_Data.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ESMS_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserRepository userRepository;
        public UserController() {
            userRepository = new UserRepository();
        }
        
        [HttpGet]
        [Route("getAll")]
        public async Task<IActionResult> GetListStudent()
        {
            var studentList =  userRepository.GetAll().ToList();
            return Ok(studentList);
        }
    }
}
