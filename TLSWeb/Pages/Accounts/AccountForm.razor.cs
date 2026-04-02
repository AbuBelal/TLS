using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib.DTOs;
using SharedLib.Entities;
using TLSWeb.Helpers;

namespace TLSWeb.Pages.Accounts;

public partial class AccountForm : ComponentBase
{
    // ────────────────────────────────────────────────
    //  Parameters
    // ────────────────────────────────────────────────
    [Parameter] public string? Id { get; set; }

    // ────────────────────────────────────────────────
    //  State
    // ────────────────────────────────────────────────
    protected UserUpsertDto Model { get; set; } = new();
    protected List<string> AvailableRoles = new();
    protected List<Employee> AllEmployees = new();
    protected Employee? SelectedEmployee;
    protected string employeeSearchText = "";
    protected bool IsSearchingEmployees = false;
    protected bool IsSaving = false;
    protected string? ErrorMessage;

    // ────────────────────────────────────────────────
    //  Computed
    // ────────────────────────────────────────────────
    protected bool IsEditMode => !string.IsNullOrWhiteSpace(Id);
    protected string PageTitle => IsEditMode ? "تعديل بيانات المستخدم" : "إضافة مستخدم جديد";
    protected string SaveButtonText => IsEditMode ? "حفظ التعديلات" : "إضافة المستخدم";

    protected List<Employee> FilteredEmployees =>
        string.IsNullOrWhiteSpace(employeeSearchText)
            ? new List<Employee>()
            : AllEmployees
                .Where(e =>
                    (e.Name?.Contains(employeeSearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.EmpId?.Contains(employeeSearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.CivilId?.Contains(employeeSearchText, StringComparison.OrdinalIgnoreCase) ?? false))
                .Take(20)
                .ToList();

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────
    protected override async Task OnInitializedAsync()
    {
        // تحميل الصلاحيات والموظفين
        AvailableRoles = await RolesApi.GetAll() ?? new List<string>();
        AllEmployees = await EmployeeApi.GetAll() ?? new List<Employee>();

        if (IsEditMode)
        {
            await LoadUserForEdit();
        }
    }

    private async Task LoadUserForEdit()
    {
        try
        {
            var user = await UserApi.GetById(Id!);
            if (user == null)
            {
                NavManager.NavigateTo(PagesUris.UsersPages.Manage);
                return;
            }

            Model.Id = user.Id;
            Model.Email = user.Email ?? "";
            Model.UserName = user.UserName ?? "";
            Model.PhoneNumber = user.PhoneNumber;
            Model.EmployeeId = user.EmployeeId;

            // جلب الصلاحية الحالية
            var userRoles = await UserApi.GetUserRole(user.Id);
            Model.Role = userRoles?.FirstOrDefault() ?? "";

            // تحميل الموظف المرتبط
            if (user.EmployeeId.HasValue)
            {
                SelectedEmployee = AllEmployees.FirstOrDefault(e => e.Id == user.EmployeeId.Value)
                    ?? await EmployeeApi.GetById(user.EmployeeId.Value);
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"حدث خطأ أثناء تحميل بيانات المستخدم: {ex.Message}";
        }
    }

    // ────────────────────────────────────────────────
    //  Employee Selection
    // ────────────────────────────────────────────────
    private Task OnEmployeeSearchChanged()
    {
        return Task.CompletedTask;
    }

    protected void SelectEmployee(Employee employee)
    {
        Model.EmployeeId = employee.Id;
        SelectedEmployee = employee;
        employeeSearchText = "";
        StateHasChanged();
    }

    protected void ClearEmployee()
    {
        Model.EmployeeId = null;
        SelectedEmployee = null;
        StateHasChanged();
    }

    // ────────────────────────────────────────────────
    //  Submit
    // ────────────────────────────────────────────────
    protected async Task HandleSubmit()
    {
        ErrorMessage = null;
        IsSaving = true;

        try
        {
            if (IsEditMode)
            {
                await UpdateUser();
            }
            else
            {
                await CreateUser();
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"حدث خطأ غير متوقع: {ex.Message}";
            MudSnackbar.Add(ErrorMessage, Severity.Error);
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task CreateUser()
    {
        // التحقق من كلمة المرور في حالة الإضافة
        if (string.IsNullOrWhiteSpace(Model.Password))
        {
            ErrorMessage = "كلمة المرور مطلوبة عند إضافة مستخدم جديد";
            return;
        }

        var input = new AdminUserInputModel
        {
            Email = Model.Email,
            UserName = Model.UserName,
            Password = Model.Password,
            Role = Model.Role
        };

        var result = await UserApi.Insert(input);

        if (result.Success)
        {
            // ربط الموظف إذا تم اختياره
            if (Model.EmployeeId.HasValue)
            {
                await LinkEmployeeToUser(result.ReturnId.ToString(), Model.EmployeeId.Value);
            }

            MudSnackbar.Add("تم إضافة المستخدم بنجاح", Severity.Success);
            NavManager.NavigateTo(PagesUris.UsersPages.Manage);
        }
        else
        {
            ErrorMessage = string.Join(" | ", result.Message);
            MudSnackbar.Add("فشل إضافة المستخدم", Severity.Error);
        }
    }

    private async Task UpdateUser()
    {
        var profile = new ProfileInputModel
        {
            Id = Model.Id!,
            Email = Model.Email,
            UserName = Model.UserName,
            PhoneNumber = Model.PhoneNumber,
            Role = Model.Role,
            EmployeeId = Model.EmployeeId
        };

        var result = await UserApi.Update(profile);

        if (result.Success)
        {
            MudSnackbar.Add("تم تحديث بيانات المستخدم بنجاح", Severity.Success);
            NavManager.NavigateTo(PagesUris.UsersPages.Manage);
        }
        else
        {
            ErrorMessage = string.Join(" | ", result.Message);
            MudSnackbar.Add("فشل تحديث بيانات المستخدم", Severity.Error);
        }
    }

    private async Task LinkEmployeeToUser(string userId, long employeeId)
    {
        try
        {
            var profile = new ProfileInputModel
            {
                Id = userId,
                Email = Model.Email,
                UserName = Model.UserName,
                PhoneNumber = Model.PhoneNumber,
                Role = Model.Role,
                EmployeeId = employeeId
            };
            await UserApi.Update(profile);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error linking employee: {ex.Message}");
        }
    }

    // ────────────────────────────────────────────────
    //  Cancel
    // ────────────────────────────────────────────────
    protected void Cancel() => NavManager.NavigateTo(PagesUris.UsersPages.Manage);
}