using APIServerLib.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLib.Responses;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserRolesController : ControllerBase
    {
        private readonly IUserRolesRepository _userRolesRepository;

        public UserRolesController(IUserRolesRepository userRolesRepository)
        {
            _userRolesRepository = userRolesRepository;
        }

        // جلب كافة الروابط بين المستخدمين والصلاحيات
        [HttpGet]
        public async Task<ActionResult<List<IdentityUserRole<string>>>> GetAll()
        {
            return Ok(await _userRolesRepository.GetAll());
        }

        // إضافة صلاحية لمستخدم
        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> Insert([FromBody] IdentityUserRole<string> userRole)
        {
            if (userRole == null || string.IsNullOrEmpty(userRole.UserId) || string.IsNullOrEmpty(userRole.RoleId))
                return BadRequest(new GeneralResponse(false, "بيانات المستخدم أو الصلاحية ناقصة."));

            var response = await _userRolesRepository.Insert(userRole);
            return Ok(response);
        }

        // حذف صلاحية من مستخدم
        // ملاحظة: بما أن المفتاح مركب، يفضل الحذف تمرير الـ UserId والـ RoleId
        [HttpDelete("{userId}/{roleId}")]
        public async Task<ActionResult<GeneralResponse>> Delete(string userId, string roleId)
        {
            // هنا يفضل تعديل الـ Repository ليدعم الحذف بواسطة المعرفين معاً
            // ولكن بناءً على الكود الحالي الخاص بك:
            var response = await _userRolesRepository.DeleteById(userId);
            return Ok(response);
        }

        [HttpGet("byuser/{userId}")]
        public async Task<ActionResult<List<IdentityRole>>> GetRolesForUser(long userId)
        {
            var roles = await _userRolesRepository.GetRolesForUser(userId);
            return Ok(roles);
        }
    }
}