using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Fixed;
using SharedLib.Mappers;
using TLSWeb.Helpers;

namespace TLSWeb.Pages.Employees;

public partial class EmployeeForm : ComponentBase
{
    
    // ────────────────────────────────────────────────
    //  Parameters
    // ────────────────────────────────────────────────
    [Parameter] public long Id { get; set; }

    // ────────────────────────────────────────────────
    //  State
    // ────────────────────────────────────────────────
    protected Employee employee = new();
    protected List<LookupValue> genders = new();
    protected List<LookupValue> jobs = new();
    protected List<LookupValue> specializations = new();

    protected bool IsSaving = false;
    protected bool IsCheckingDuplicate = false;
    protected bool IsDuplicate = false;
    protected string DuplicateMessage = string.Empty;
    private List<Center> centers = new();          // ← جديد
    private long? selectedCenterId;
    // ────────────────────────────────────────────────
    //  Computed
    // ────────────────────────────────────────────────
    protected bool IsEditMode => Id != 0;
    protected string PageTitle => IsEditMode ? "تعديل بيانات موظف" : "إضافة موظف جديد";
    protected string SaveButtonText => IsEditMode ? "حفظ التعديل" : "حفظ الموظف";

    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────
    protected override async Task OnInitializedAsync()
    {
        // تحميل بيانات الموظف إذا كنا في وضع التعديل
        if (IsEditMode)
        {
            employee = await EmployeeApi.GetById(Id);
        }

        // جلب بيانات المنسدلات
        genders = await LookupValueApi.GetByValueType(LookupTypes.Gender) ?? new();
        jobs = await LookupValueApi.GetByValueType(LookupTypes.Job) ?? new();
        specializations = await LookupValueApi.GetByValueType(LookupTypes.Specialization) ?? new();
        centers = await CenterApi.GetAll() ?? new();
    }

    // ────────────────────────────────────────────────
    //  Duplicate Check
    // ────────────────────────────────────────────────
    protected async Task CheckDuplicateAsync()
    {
        //تجاهل الفحص إذا كان حقل الهوية فارغاً
        if (string.IsNullOrWhiteSpace(employee.CivilId) && string.IsNullOrWhiteSpace(employee.EmpId))
        {
            ResetDuplicateState();
            return;
        }

        // في وضع التعديل: لا نفحص إذا لم يتغير رقم الهوية
        if (IsEditMode)
        {
            var original = await EmployeeApi.GetById(Id);
            if (original?.CivilId == employee.CivilId)
            {
                ResetDuplicateState();
                return;
            }
        }

        IsCheckingDuplicate = true;
        StateHasChanged();

        try
        {
            

            if (string.IsNullOrWhiteSpace(employee.EmpId))
            {
                IsDuplicate = true;
                DuplicateMessage = $"يرجى كتابة رقم الموظف ";
            }
            else
            {

                try
                {
                    var result = await EmployeeApi.GetByEmpId(employee.EmpId);
                    if (result is not null)
                    {
                        IsDuplicate = true;
                        DuplicateMessage = $"هذا الموظف مسجل بنفس رقم الوظيفة مسبقاً باسم: {result.Name}";
                        return;
                    }
                    else
                        ResetDuplicateState();
                }
                catch  { }

                
            }

            if (!string.IsNullOrWhiteSpace(employee.CivilId))
            {
                var result = await EmployeeApi.GetByCivilId(employee.CivilId);

                if (result is not null)
                {
                    IsDuplicate = true;
                    DuplicateMessage = $"هذا الموظف مسجل بنفس رقم الهوية مسبقاً باسم: {result.Name}";
                }
            }
        }
        catch
        {
            ResetDuplicateState();
            //IsCheckingDuplicate = false;
            //DuplicateMessage = string.Empty;// "لا يوجد تكرار ، يمكنك الاستمرار .";
            //StateHasChanged();
        }
        finally
        {
            IsCheckingDuplicate = false;
            StateHasChanged();
        }
    }

    // ────────────────────────────────────────────────
    //  Submit
    // ────────────────────────────────────────────────
    protected async Task HandleSubmit()
    {
        IsSaving = true;
        //حماية مزدوجة: تحقق من التكرار قبل الحفظ
        await CheckDuplicateAsync();
        if (IsDuplicate) return;

        IsSaving = true;

        var mapper = new EmployeeMapper();
        var employeeToSend = mapper.ToEmployeeUpsertDTO(employee);
       
        try
        {
            if (IsEditMode)
            {
                var response = await EmployeeApi.Update(employeeToSend);

                if (response.Success)
                {
                    MudSnackbar.Add(response.Message, Severity.Success);
                    NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);
                }
                else
                {
                    MudSnackbar.Add(response.Message, Severity.Error);
                }
            }
            else
            {
                var employee = mapper.ToEntity(employeeToSend);
                var employeeWithCenter = new EmployeeWithCenter
                {
                    Employee = employee,
                    CenterId = selectedCenterId ?? 0 // تأكد من تعيين مركز افتراضي إذا لم يتم الاختيار
                };
                var response = await EmployeeApi.AddWithCenter(employeeWithCenter);

                if (response.Success)
                {
                    MudSnackbar.Add(response.Message, Severity.Success);
                    NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);
                }
                else
                {
                    MudSnackbar.Add(response.Message, Severity.Error);
                }
            }
        }
        catch (Exception ex)
        {
            MudSnackbar.Add($"حدث خطأ غير متوقع: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsSaving = false;
        }
        IsSaving = false;
    }

    // ────────────────────────────────────────────────
    //  Cancel
    // ────────────────────────────────────────────────
    protected void Cancel() => NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);

    // ────────────────────────────────────────────────
    //  Helpers
    // ────────────────────────────────────────────────
    private void ResetDuplicateState()
    {
        IsDuplicate = false;
        DuplicateMessage = string.Empty;
    }
}
