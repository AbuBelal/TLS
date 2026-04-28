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
        selectedCenterId=employee.EmpCenters.OrderByDescending(x=>x.FromDate).FirstOrDefault()?.CenterId;
    }

    // ────────────────────────────────────────────────
    //  Duplicate Check
    // ────────────────────────────────────────────────
    protected async Task CheckDuplicateAsync()
    {
        //تجاهل الفحص إذا كان حقل الهوية فارغاً
        if (string.IsNullOrWhiteSpace(employee.CivilId) || string.IsNullOrWhiteSpace(employee.EmpId))
        {
            IsDuplicate = true;
            //ResetDuplicateState();
            DuplicateMessage = "يرجى كتابة رقم الهوية أو رقم الوظيفة   .";
            return;
        }

        if (IsEditMode)
        {
            EmployeeDuplicateCheckRequest request = new EmployeeDuplicateCheckRequest
            {
                EmpId = employee.EmpId,
                CivilId = employee.CivilId,
                ExcludeEmployeeId = Id
                
            };
            var IsEmpIdDublicate =  await EmployeeApi.IsEmpIdDuplicate(request) ;
            var IsCivilIdDublicate = await EmployeeApi.IsCivilIdDuplicate(request) ;
            if (IsEmpIdDublicate.Id>0)
            {
                IsDuplicate = true;
                DuplicateMessage = $"رقم الوظيفة مكرر مع الموظف: {IsEmpIdDublicate.Name}";
                return;
            }
            else
                if (IsCivilIdDublicate.Id>0)
                {
                    IsDuplicate = true;
                    DuplicateMessage = $"رقم الهوية مكرر مع الموظف: {IsCivilIdDublicate.Name}";
                    return;
                }
            ResetDuplicateState() ;
            return;
        }

        IsCheckingDuplicate = true;
        StateHasChanged();

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


            result = await EmployeeApi.GetByCivilId(employee.CivilId);

            if (result is not null)
            {
                IsDuplicate = true;
                DuplicateMessage = $"هذا الموظف مسجل بنفس رقم الهوية مسبقاً باسم: {result.Name}";
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
        var mapper = new EmployeeMapper();
        var employeeToSend = mapper.ToEmployeeUpsertDTO(employee);
        employeeToSend.CenterId = selectedCenterId;
        EmployeeDuplicateCheckRequest request = new EmployeeDuplicateCheckRequest
        {
            EmpId = employee.EmpId,
            CivilId = employee.CivilId,
            ExcludeEmployeeId = Id
        };

        var IsCivilIdDublicate = await EmployeeApi.IsCivilIdDuplicate(request);
        if (IsCivilIdDublicate?.Id > 0)
        {
            IsDuplicate = true;
            DuplicateMessage = $"رقم الهوية مكرر مع الموظف: {IsCivilIdDublicate.Name}";
            IsSaving = false;
            MudSnackbar.Add(DuplicateMessage, Severity.Error);
            return;
        }
        else
        {
            var IsEmpIdDublicate = await EmployeeApi.IsEmpIdDuplicate(request);
            if (IsEmpIdDublicate?.Id > 0)
            {
                IsDuplicate = true;
                DuplicateMessage = $"رقم الوظيفة مكرر مع الموظف: {IsEmpIdDublicate.Name}";
                IsSaving = false;
                MudSnackbar.Add(DuplicateMessage, Severity.Error);
                return;
            }
        }

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





        //IsSaving = true;
        ////حماية مزدوجة: تحقق من التكرار قبل الحفظ
        //await CheckDuplicateAsync();
        //if (IsDuplicate)
        //{ 
        //    MudSnackbar.Add(DuplicateMessage, Severity.Error); 
        //    IsSaving = false;
        //    return; 
        //}

        //IsSaving = true;

        //var mapper = new EmployeeMapper();
        //var employeeToSend = mapper.ToEmployeeUpsertDTO(employee);

        //try
        //{
        //    if (IsEditMode)
        //    {
        //        var response = await EmployeeApi.Update(employeeToSend);

        //        if (response.Success)
        //        {
        //            MudSnackbar.Add(response.Message, Severity.Success);
        //            NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);
        //        }
        //        else
        //        {
        //            MudSnackbar.Add(response.Message, Severity.Error);
        //        }
        //    }
        //    else
        //    {

        //        var employee = mapper.ToEntity(employeeToSend);
        //        var employeeWithCenter = new EmployeeWithCenter
        //        {
        //            Employee = employee,
        //            CenterId = selectedCenterId ?? 0 // تأكد من تعيين مركز افتراضي إذا لم يتم الاختيار
        //        };
        //        var response = await EmployeeApi.AddWithCenter(employeeWithCenter);

        //        if (response.Success)
        //        {
        //            MudSnackbar.Add(response.Message, Severity.Success);
        //            NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);
        //        }
        //        else
        //        {
        //            MudSnackbar.Add(response.Message, Severity.Error);
        //        }
        //    }
        //}
        //catch (Exception ex)
        //{
        //    MudSnackbar.Add($"حدث خطأ غير متوقع: {ex.Message}", Severity.Error);
        //}
        //finally
        //{
        //    IsSaving = false;
        //}
        //IsSaving = false;
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
