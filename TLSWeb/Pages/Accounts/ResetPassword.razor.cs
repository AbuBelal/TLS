// ╔══════════════════════════════════════════════════════════════╗
// ║  TLSWeb/Pages/Accounts/ResetPassword.razor.cs                ║
// ╚══════════════════════════════════════════════════════════════╝

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using Refit;
using SharedLib.DTOs;
using System.Security.Claims;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Pages.Accounts;

public partial class ResetPassword : ComponentBase
{

    // ── State عام ─────────────────────────────────────────────
    protected bool   IsAdmin    { get; private set; } = false;
    protected bool   IsLoading  { get; private set; } = true;
    protected string CurrentUserId { get; private set; } = string.Empty;

    // ── قائمة المستخدمين (للمدير فقط) ────────────────────────
    protected List<UserWithRoles?>  Users          { get; private set; } = new();
    protected string                SearchText     { get; set; } = string.Empty;
    protected UserWithRoles?        SelectedUser   { get; private set; }

    // ── نماذج كلمة المرور ─────────────────────────────────────
    // للمدير: إعادة ضبط بدون كلمة المرور القديمة
    protected AdminResetModel AdminReset { get; set; } = new();

    // للمستخدم العادي: تغيير بكلمة المرور القديمة
    protected SelfChangeModel SelfChange { get; set; } = new();

    // ── حالة الحفظ ────────────────────────────────────────────
    protected bool IsSavingAdmin { get; private set; } = false;
    protected bool IsSavingSelf  { get; private set; } = false;

    // ── تهيئة ─────────────────────────────────────────────────
    protected override async Task OnInitializedAsync()
    {
        var auth  = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user  = auth.User;

        CurrentUserId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
        IsAdmin       = user.IsInRole(SharedLib.Fixed.Roles.Admin);

        if (IsAdmin)
        {
            try { Users = await UserApi.GetAllWithRoles(); }
            catch { Users = new(); }
        }

        SelfChange.UserId = CurrentUserId;
        IsLoading = false;
    }

    // ── بحث ──────────────────────────────────────────────────
    protected IEnumerable<UserWithRoles?> FilteredUsers =>
        string.IsNullOrWhiteSpace(SearchText)
            ? Users
            : Users.Where(u =>
                (u?.UserName?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                (u?.Email?.Contains(SearchText, StringComparison.OrdinalIgnoreCase)    ?? false) ||
                (u?.Employee?.Name?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));

    // ── اختيار مستخدم ─────────────────────────────────────────
    protected void SelectUser(UserWithRoles? user)
    {
        SelectedUser  = user;
        AdminReset    = new AdminResetModel { UserId = user?.Id ?? string.Empty };
    }

    protected void ClearSelection()
    {
        SelectedUser = null;
        AdminReset   = new();
    }

    // ── إعادة ضبط (مدير) ─────────────────────────────────────
    protected async Task HandleAdminReset()
    {
        if (IsSavingAdmin || SelectedUser is null) return;
        IsSavingAdmin = true;

        try
        {
            var model = new PasswordInputModel
            {
                UserId      = SelectedUser.Id,
                OldPassword = "ADMIN_RESET",   // مطلوب في الـ DTO لكن لا يُستخدم في ResetPassword
                NewPassword = AdminReset.NewPassword,
                ConfirmPassword = AdminReset.ConfirmPassword
            };

            var result = await UserApi.ResetPassword(model);

            if (result.Success)
            {
                MudSnackbar.Add($"✅ تم إعادة ضبط كلمة مرور {SelectedUser.UserName} بنجاح", Severity.Success);
                ClearSelection();
            }
            else
            {
                MudSnackbar.Add($"❌ {result.Message}", Severity.Error);
            }
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"خطأ في الاتصال: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsSavingAdmin = false;
        }
    }

    // ── تغيير كلمة المرور (المستخدم نفسه) ────────────────────
    protected async Task HandleSelfChange()
    {
        if (IsSavingSelf) return;
        IsSavingSelf = true;

        try
        {
            var model = new PasswordInputModel
            {
                UserId          = CurrentUserId,
                OldPassword     = SelfChange.OldPassword,
                NewPassword     = SelfChange.NewPassword,
                ConfirmPassword = SelfChange.ConfirmPassword
            };

            var result = await UserApi.ChangePassword(model);

            if (result.Success)
            {
                MudSnackbar.Add("✅ تم تغيير كلمة المرور بنجاح", Severity.Success);
                SelfChange = new SelfChangeModel { UserId = CurrentUserId };
            }
            else
            {
                MudSnackbar.Add($"❌ {result.Message}", Severity.Error);
            }
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"خطأ في الاتصال: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsSavingSelf = false;
        }
    }

    // ── إخفاء / إظهار كلمة المرور ─────────────────────────────
    protected Dictionary<string, bool> ShowPassword { get; } = new()
    {
        ["old"]     = false,
        ["new"]     = false,
        ["confirm"] = false,
        ["anew"]    = false,
        ["aconfirm"]= false,
    };

    protected void ToggleShow(string key)
        => ShowPassword[key] = !ShowPassword[key];

    protected string InputType(string key)
        => ShowPassword[key] ? "text" : "password";

    protected string EyeIcon(string key)
        => ShowPassword[key] ? "bi-eye-slash" : "bi-eye";

    // ── قوة كلمة المرور ───────────────────────────────────────
    protected (int Score, string Label, string Color) PasswordStrength(string? pwd)
    {
        if (string.IsNullOrEmpty(pwd)) return (0, "", "");
        int score = 0;
        if (pwd.Length >= 8)  score++;
        if (pwd.Any(char.IsUpper)) score++;
        if (pwd.Any(char.IsDigit)) score++;
        if (pwd.Any(c => !char.IsLetterOrDigit(c))) score++;

        return score switch
        {
            1 => (25,  "ضعيفة",    "bg-danger"),
            2 => (50,  "متوسطة",   "bg-warning"),
            3 => (75,  "جيدة",     "bg-info"),
            4 => (100, "قوية جداً","bg-success"),
            _ => (0,   "",         "")
        };
    }
}

// ── نماذج الصفحة ──────────────────────────────────────────────
public class AdminResetModel
{
    public string UserId { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "يجب أن تكون 6 أحرف على الأقل")]
    public string NewPassword { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [System.ComponentModel.DataAnnotations.Compare(nameof(NewPassword), ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public string ConfirmPassword { get; set; } = string.Empty;
}

public class SelfChangeModel
{
    public string UserId { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "كلمة المرور الحالية مطلوبة")]
    public string OldPassword { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "كلمة المرور الجديدة مطلوبة")]
    [System.ComponentModel.DataAnnotations.MinLength(6, ErrorMessage = "يجب أن تكون 6 أحرف على الأقل")]
    public string NewPassword { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "تأكيد كلمة المرور مطلوب")]
    [System.ComponentModel.DataAnnotations.Compare(nameof(NewPassword), ErrorMessage = "كلمتا المرور غير متطابقتين")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
