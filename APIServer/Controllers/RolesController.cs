using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

[Route("api/[controller]")]
[ApiController]
//[Authorize(Roles = "Admin")] // حماية الكنترولر للأدمن فقط
public class RolesController : ControllerBase
{
    private readonly RoleManager<IdentityRole> _roleManager;

    public RolesController(RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
    }

    // 1. جلب كل الأدوار
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var roles = await _roleManager.Roles.Select(_=>_.Name).ToListAsync()??new List<string?>();
        return Ok(roles);
    }

    // 2. إنشاء دور جديد
    [HttpPost]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return BadRequest("اسم الدور لا يمكن أن يكون فارغاً");

        var roleExist = await _roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
            if (result.Succeeded)
            {
                return Ok(new { Message = $"تم إنشاء الدور {roleName} بنجاح" });
            }
            return BadRequest(result.Errors);
        }
        return BadRequest("الدور موجود مسبقاً");
    }

    // 3. حذف دور
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRole(string id)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null)
            return NotFound("الدور غير موجود");

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
        {
            return Ok(new { Message = "تم حذف الدور بنجاح" });
        }
        return BadRequest(result.Errors);
    }

    // 4. تعديل اسم الدور
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRole(string id, [FromBody] string newRoleName)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role == null) return NotFound();

        role.Name = newRoleName;
        var result = await _roleManager.UpdateAsync(role);

        if (result.Succeeded) return Ok(role);
        return BadRequest(result.Errors);
    }
}
















//using APIServerLib.Repositories.Interfaces;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using SharedLib.Responses;

//namespace APIServer.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    // [Authorize(Roles = "Admin")] // يفضل تفعيل الحماية هنا
//    public class RolesController : ControllerBase
//    {
//        private readonly IRolesRepository _rolesRepository;

//        public RolesController(IRolesRepository rolesRepository)
//        {
//            _rolesRepository = rolesRepository;
//        }

//        [HttpGet]
//        public async Task<ActionResult<List<IdentityRole>>> GetAll()
//        {
//            var roles = await _rolesRepository.GetAll();
//            return Ok(roles);
//        }

//        [HttpGet("{id}")]
//        public async Task<ActionResult<IdentityRole>> GetById(long id)
//        {
//            var role = await _rolesRepository.GetById(id);
//            if (role == null)
//                return NotFound(new GeneralResponse(false, "الصلاحية غير موجودة."));

//            return Ok(role);
//        }

//        [HttpPost]
//        public async Task<ActionResult<GeneralResponse>> Insert([FromBody] IdentityRole role)
//        {
//            if (role == null || string.IsNullOrEmpty(role.Name))
//                return BadRequest(new GeneralResponse(false, "بيانات الصلاحية غير مكتملة."));

//            // تأكد من عمل NormalizedName لضمان عمل Identity بشكل صحيح
//            role.NormalizedName = role.Name.ToUpper();

//            var response = await _rolesRepository.Insert(role);
//            return Ok(response);
//        }

//        [HttpPut]
//        public async Task<ActionResult<GeneralResponse>> Update([FromBody] IdentityRole role)
//        {
//            if (role == null)
//                return BadRequest(new GeneralResponse(false, "بيانات غير صالحة."));

//            role.NormalizedName = role.Name.ToUpper();
//            var response = await _rolesRepository.Update(role);
//            return Ok(response);
//        }

//        [HttpDelete("{id}")]
//        public async Task<ActionResult<GeneralResponse>> Delete(long id)
//        {
//            var response = await _rolesRepository.DeleteById(id);
//            if (!response.Success)
//                return NotFound(response);

//            return Ok(response);
//        }
//    }
//}