using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Fixed;
using SharedLib.Helpers;
using SharedLib.Mappers;
using SharedLib.Responses;
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
    protected List<LookupValue> AdrGovs = new();
    protected List<LookupValue> AdrAreas = new();

    string WhatsappNo01, WhatsappNo02;

    protected bool IsSaving = false;
    protected bool IsCheckingDuplicate = false;
    protected bool IsDuplicate = false;
    protected string DuplicateMessage = string.Empty;
    private List<Center> centers = new();          // ← جديد
    private long? selectedCenterId=0;
    private long? selectedAdrGovId=0;
    private long? selectedAdrAreaId=0;
    bool isDialogOpen = false;
    string dialogMessage = "هذا الموظف غير مسجل في أي مركز، هل تريد إضافته في مركزكم ؟";
    // ────────────────────────────────────────────────
    //  Computed
    // ────────────────────────────────────────────────
    protected bool IsEditMode=false;
    protected string PageTitle => IsEditMode ? "تعديل بيانات موظف" : "إضافة موظف جديد";
    protected string SaveButtonText => IsEditMode ? "حفظ التعديل" : "حفظ الموظف";
    bool IsCivilIdGood = false;
    // ────────────────────────────────────────────────
    //  Lifecycle
    // ────────────────────────────────────────────────
    protected override async Task OnInitializedAsync()
    {
        IsEditMode = Id != 0;
        // تحميل بيانات الموظف إذا كنا في وضع التعديل
        if (IsEditMode)
        {
            employee = await EmployeeApi.GetById(Id);
            if(employee.CivilId is not null)
             CheckCivilId();
            WhatsappNo01 = WhatsAppHelper.GetWhatsAppLink(employee.Mobile);
            WhatsappNo02 = WhatsAppHelper.GetWhatsAppLink(employee.Mobile, "", "972");
        }

        // جلب بيانات المنسدلات
        genders = await LookupValueApi.GetByValueType(LookupTypes.Gender) ?? new();
        jobs = await LookupValueApi.GetByValueType(LookupTypes.Job) ?? new();
        specializations = await LookupValueApi.GetByValueType(LookupTypes.Specialization) ?? new();
        AdrGovs = await LookupValueApi.GetByValueType(LookupTypes.Governate) ?? new();
        AdrAreas = await LookupValueApi.GetByValueType(LookupTypes.Area) ?? new();
        centers = await CenterApi.GetAll() ?? new();
        selectedCenterId=employee.EmpCenters.FirstOrDefault(c=>c.IsActive)?.CenterId;
        selectedAdrGovId = employee.AdrGovId;
        selectedAdrAreaId = employee.AdrAreaId;

    }

    // ────────────────────────────────────────────────
    //  Submit
    // ────────────────────────────────────────────────
    protected async Task SaveEmployee()
    {
        var mapper = new EmployeeMapper();
        var employeeToSend = mapper.ToEmployeeUpsertDTO(employee);
        employeeToSend.CenterId = selectedCenterId;
        try
        {
            if (IsEditMode)
            {
                long newCenter = employeeToSend.CenterId??0;
                var ee = employee.EmpCenters.FirstOrDefault(c => c.IsActive);
                long oldCenter = ee is null ?0 : ee.CenterId;

                GeneralResponse response;
                if (oldCenter == newCenter)
                    response = await EmployeeApi.Update(employeeToSend);
                else
                    response = await EmployeeApi.UpdateWithCeneter(employeeToSend);

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
    protected async Task HandleSubmit()
    {
        employee.Name = employee.Name.Trim();
        employee.EnName = employee.EnName?.Trim();
        employee.CivilId = employee.CivilId.Trim();
        employee.EmpId = employee.EmpId.Trim();
        IsSaving = true;
        
        EmployeeDuplicateCheckRequest request = new EmployeeDuplicateCheckRequest
        {
            EmpId = employee.EmpId,
            CivilId = employee.CivilId,
            ExcludeEmployeeId = Id
        };

        var IsDublicate = await EmployeeApi.IsEmployeeDuplicate(request);
        if (IsDublicate?.Id > 0)
        {
            employee.Id = IsDublicate.Id;
            IsDuplicate = true;
            if (IsDublicate.EmpId == employee.EmpId || IsDublicate.CivilId == employee.CivilId)
            {
                if (IsDublicate.EmpCenters?.FirstOrDefault(c => c.IsActive) != null)
                {
                    DuplicateMessage = $"رقم الوظيفة أو رقم الهوية أو الإثنين مكرران مع الموظف: {IsDublicate.Name} في مركز {IsDublicate.EmpCenters?.FirstOrDefault(c => c.IsActive)?.Center?.Name}";
                }
                else
                {
                    DuplicateMessage = $"رقم الوظيفة ورقم الهوية مكرران مع الموظف: {IsDublicate.Name} في مركز غير محدد";
                    dialogMessage = $"هذا الموظف / {IsDublicate.Name} موجود مسبقاُ وغير مسجل في أي مركز ، هل ترغب بنقله لمركزكم ؟";
                    isDialogOpen = true;
                    StateHasChanged();
                    return;
                }
            }
           
            IsSaving = false;
            MudSnackbar.Add(DuplicateMessage, Severity.Error);
            return;
        }
        else
        await SaveEmployee();
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
    private void CancelDialog()
    {
        IsSaving = false;
        isDialogOpen = false;
        StateHasChanged();
    }
    private async void ConfirmDialog()
    {
        IsSaving = false;
        //IsEditMode = true;
        var mapper = new EmployeeMapper();
        var employeeToSend = mapper.ToEmployeeUpsertDTO(employee);
        employeeToSend.CenterId = selectedCenterId;
        var response = await EmployeeApi.RegisterEmpInCenter(employeeToSend);

        isDialogOpen = false;
        if (response.Success)
        {
            MudSnackbar.Add("تم تسجيل الموظف في المركز", Severity.Success);
            NavManager.NavigateTo(PagesUris.EmployeesPages.Manage);
        }
        else
            MudSnackbar.Add(response.Message, Severity.Error);
    }

    private async Task TranslateToEng()
    {
        string arabicName = employee.Name;
        string EnglishName = employee.EnName;
        if (!string.IsNullOrWhiteSpace(arabicName) && string.IsNullOrWhiteSpace(EnglishName))
        {
            //var translationService = new TranslationService(new HttpClient());
            // isLoading = true;
            try
            {
                EnglishName = await Translationservice.TranslateNameAsync(arabicName);
                if (!string.IsNullOrEmpty(EnglishName))
                {
                    employee.EnName = EnglishName;
                    StateHasChanged();
                }
            }
            catch (Exception ex)
            {
                EnglishName = "حدث خطأ أثناء الاتصال بالخدمة.";
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // isLoading = false;
            }
        }

    }


    private void CheckCivilId()
    {
        if (!string.IsNullOrEmpty(employee.CivilId) && Checks.CheckLuhnE9(employee.CivilId))
        {
            IsCivilIdGood = true;
        }
        else if (!string.IsNullOrEmpty(employee.CivilId))
        {
            IsCivilIdGood = false;
        }
    }
}
