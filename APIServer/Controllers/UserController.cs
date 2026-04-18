using APIServerLib.Repositories.Implemntations;
using APIServerLib.Repositories.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Fixed;
using SharedLib.Responses;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APIServer.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AuditLogService _auditLogService;

        public UserController(IUserRepository userRepository, 
            UserManager<ApplicationUser> userManager, 
            SignInManager<ApplicationUser> signInManager, 
            AuditLogService auditLogService)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _auditLogService = auditLogService;
        }


        [HttpGet("profile")]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);
            var user1 =await _userRepository.GetUserByEmail(user.Email); //await _userManager.FindByIdAsync(userId);
            if(user == null) return NotFound("User not found");

            UserProfileDto profile = new UserProfileDto();
            profile.Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault();
            profile.Email = user.Email;
            profile.UserName = user.UserName;
            profile.UserId = user.Id;
            profile.EmployeeId = user1.EmployeeId;
            profile.EmployeeName = user1.Employee.Name != null ? user.Employee.Name : null;
            if (profile.EmployeeId is not null)
            {
                var Center =  user1.Employee?.EmpCenters.OrderByDescending(x => x.FromDate).FirstOrDefault();
                profile.CenterId = Center?.CenterId;
                profile.CenterName = Center?.Center.Name;
            }
            await _auditLogService.LogAsync("Read", "User", userId, $"قراءة ملف المستخدم: {user.UserName}");
            return Ok(profile);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            // إنهاء جلسة المستخدم وحذف الكوكيز
            //await HttpContext.SignOutAsync();
            await _signInManager.SignOutAsync();
            // إعادة التوجيه إلى الصفحة الرئيسية أو صفحة تسجيل الدخول
            return Ok();
        }

        [HttpGet]
        public async Task<ActionResult<List<ApplicationUser>>> GetAll()
        {
            var users = await _userRepository.GetAll();
            await _auditLogService.LogAsync("Read", "User", "", $"قراءة جميع المستخدمين");
            return Ok(users);
        }
        [HttpGet("GetAllWithRoles")]
        public async Task<ActionResult<List<UserWithRoles>>> GetAllWithRols()
        {
            var users = await _userRepository.GetAllWithRols();
            await _auditLogService.LogAsync("Read", "User", "", $"قراءة جميع المستخدمين مع الأدوار");
            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetById(string id)
        {
            var result = await _userRepository.GetById(id);
            if (result == null)
                return NotFound();
            await _auditLogService.LogAsync("Read", "User", id, $"قراءة مستخدم: {result.UserName}");
            return Ok(result);
        }

        [HttpGet("GetByEmail/{Email}")]
        public async Task<ActionResult<ApplicationUser>> GetByEmail(string Email)
        {
            var result = await _userRepository.GetUserByEmail(Email);
            if (result == null)
                return NotFound();
            await _auditLogService.LogAsync("Read", "User", "", $"قراءة مستخدم بالبريد الإلكتروني: {result.UserName}");
            return Ok(result);
        }

        //[HttpPost]
        //public async Task<ActionResult<GeneralResponse>> Insert(Student student)
        //{

        //    var response = await _userRepository.Insert(student);
        //    return Ok(response);
        //}

        [HttpPut]
        public async Task<ActionResult<GeneralResponse>> Update(ProfileInputModel Profile)//ProfileInputModel Profile)
        {
            //var updateResult = await _userManager.UpdateAsync(user);
            var user = await _userManager.FindByIdAsync(Profile.Id);
            if (user != null)
            {
                user.PhoneNumber = Profile.PhoneNumber;
                user.UserName = Profile.UserName;
                user.EmployeeId = Profile.EmployeeId;
                // ... other properties ...

                // Update the user data
                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded && !string.IsNullOrEmpty(Profile.Role))
                {
                    // Handle role updates separately
                    var currentRoles = await _userManager.GetRolesAsync(user);
                    var newRoles = Profile.Role; // From your view model

                    // Remove roles not in the new list
                    await _userManager.RemoveFromRolesAsync(user, currentRoles);

                    currentRoles.Clear();
                    currentRoles.Add(Profile.Role);
                    await _userManager.AddToRolesAsync(user, currentRoles);

                    // Optional: Update security stamp
                    await _userManager.UpdateSecurityStampAsync(user);
                    await _auditLogService.LogAsync("Update", "User", "", $"تم تعديل ملف المستخدم: {user.UserName}");
                    return Ok(new GeneralResponse(true, "All User Profile updated successfully"));
                }
                await _auditLogService.LogAsync("Update", "User", "", $"تم تعديل ملف المستخدم: {user.UserName}");
                return Ok(new GeneralResponse(true, "User Data updated successfully"));
                //var response = await _userRepository.Update(user);
                //_userManager.UpdateAsync(user).Wait();
                //var user1 = await _userRepository.GetUserByEmail(user.Email);
                //_userManager.AddToRoleAsync(user1, Roles.Admin).Wait();

                //return Ok(new GeneralResponse(true,""));
                // return Ok(response);
            }
            return BadRequest();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<GeneralResponse>> Delete(string id)
        {
            var response = await _userRepository.DeleteById(id);
            var user = await _userManager.FindByIdAsync(id);
            await _auditLogService.LogAsync("Delete", "User", id, $"تم حذف المستخدم: {user?.UserName}");
            return Ok(response);
        }

        [HttpPost("ChangePassword")]
        public async Task<ActionResult<GeneralResponse>> ChangePassword(PasswordInputModel passwordInputModel)
        {
            var user = await _userManager.FindByIdAsync(passwordInputModel.UserId);
            var userId = passwordInputModel.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            if (user == null) return NotFound(new GeneralResponse(false, $"المستخدم غير موجود", 0));

            var result = await _userManager.ChangePasswordAsync(user, passwordInputModel.OldPassword, passwordInputModel.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                
                return BadRequest(new GeneralResponse(false, $"فشل تغيير كلمة المرور: {errors}", 0));
            }
            await _auditLogService.LogAsync("Update", "User", "", $"تم تعديل ملف المستخدم: {user.UserName}");
            return Ok(new GeneralResponse(true, "تم تغيير كلمة المرور بنجاح", 0));
        }

        [HttpPost("ResetPassword")]
        public async Task<ActionResult<GeneralResponse>> ResetPassword(PasswordInputModel passwordInputModel)
        {

            // 1. العثور على المستخدم عبر الـ ID
            var user = await _userManager.FindByIdAsync(passwordInputModel.UserId);
            if (user == null) return NotFound("المستخدم غير موجود.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, passwordInputModel.NewPassword);
            if(result.Succeeded)
            {
                
                await _userManager.UpdateSecurityStampAsync(user);
                await _auditLogService.LogAsync("Update", "User", "", $"تم تعيين كلمة مرور جديدة للمستخدم: {user.UserName}");   
                return Ok(new GeneralResponse(true, "تم تعيين كلمة مرور جديدة للمستخدم بنجاح.", 0));
            }
            else
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                await _auditLogService.LogAsync("Update", "User", "", $"فشل في تعيين كلمة المرور الجديدة: {errors}");
                return BadRequest(new GeneralResponse(false, $"فشل في تعيين كلمة المرور الجديدة: {errors}", 0));
            }
        }

        [HttpPost("LogIn")]
        public async Task<ActionResult<GeneralResponse>> LogIn(LoginModel loginmodel)
        {
            //var userId = passwordInputModel.UserId;
            //if (string.IsNullOrEmpty(userId)) return Unauthorized();

            var user = await _userRepository.GetUserByEmail(loginmodel.Email);// _userManager.FindByIdAsync(userId);

            var result = await _signInManager.PasswordSignInAsync(user.Email, loginmodel.Password, true, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                return Ok("تم تسجيل الدخول بنجاح");
            }

            if (result.IsLockedOut)
            {
                // الحساب مقفل بسبب محاولات خاطئة متكررة
                return BadRequest("الحساب مقفل حالياً، يرجى المحاولة لاحقاً.");
            }

            if (result.IsNotAllowed)
            {
                // الحساب غير مسموح له بالدخول (مثلاً الإيميل غير مؤكد)
                return BadRequest("الحساب غير مفعل، يرجى تأكيد البريد الإلكتروني.");
            }

            if (result.RequiresTwoFactor)
            {
                // الحساب يتطلب مصادقة ثنائية
                return Ok(new { RequiresTwoFactor = true, Message = "يرجى إدخال رمز التحقق." });
            }
            await _auditLogService.LogAsync("Read", "User", "", $"فشل في تسجيل الدخول: {loginmodel.Email}");
            // إذا لم يكن أي مما سبق، فالمشكلة غالباً هي كلمة مرور خاطئة
            return BadRequest("اسم المستخدم أو كلمة المرور غير صحيحة.");
        }

        [HttpPost("AddUser")]
        public async Task<ActionResult<GeneralResponse>> AddUser(AdminUserInputModel Input)
        {
            var newUser = new ApplicationUser { UserName = Input.Email, Email = Input.Email, EmailConfirmed = true };
            newUser.EmployeeId = Input.EmployeeId==0 ? null : Input.EmployeeId;
            //1.إنشاء المستخدم
            var result = await _userManager.CreateAsync(newUser, Input.Password);

            if (result.Succeeded)
            {
                // 2. تعيين الدور (Role) إذا تم اختياره
                if (!string.IsNullOrEmpty(Input.Role))
                {
                    await _userManager.AddToRoleAsync(newUser, Input.Role);
                }
                await _auditLogService.LogAsync("Create", "User", "", $"تم إضافة مستخدم جديد: {newUser.UserName}");
                return Ok(new GeneralResponse(true, "User added successfully", 0));
                //NavManager.NavigateTo("/admin/users?success=UserAdded");
            }
            else
            {
                //Errors.AddRange(result.Errors.Select(e => e.Description));
                //isProcessing = false;
                await _auditLogService.LogAsync("Create", "User", "", $"فشل في إضافة مستخدم جديد: {newUser.UserName}"); 
                return BadRequest(new GeneralResponse(false, $"User creation failed: {string.Join("; ", result.Errors.Select(e => e.Description))}", 0));
            }
        }

        [HttpGet("GetUserRoles/{id}")]
        public async Task<ActionResult<List<string>?>> GetUserRoles(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound("User not found");
            var roles = await _userManager.GetRolesAsync(user);
            await _auditLogService.LogAsync("Read", "User", id, $"قراءة أدوار المستخدم: {user.UserName}");  
            return Ok(roles);
        }

        [HttpGet("GetCurUserRoles")]
        public async Task<ActionResult<List<string>?>> GetCurUserRoles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound("User not found");
            var roles = await _userManager.GetRolesAsync(user);
            await _auditLogService.LogAsync("Read", "User", userId, $"قراءة أدوار المستخدم: {user.UserName}");
            return Ok(roles);
        }
    }
}
