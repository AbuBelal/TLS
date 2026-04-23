// ╔══════════════════════════════════════════════════════════════╗
// ║  TLSWeb/Pages/Centers/CenterEdit.razor.cs                    ║
// ╚══════════════════════════════════════════════════════════════╝

using Microsoft.AspNetCore.Components;
using MudBlazor;
using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using SharedLib.Responses;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Pages.Centers;

public partial class CenterEdit : ComponentBase
{
    [Parameter] public Center? center { get; set; }
    [Parameter] public string BackgroundColor { get; set; } = "#fff";
    List<LookupValue> WHoures=new List<LookupValue>();
    bool IsAdmin=false;
    // ── State ──────────────────────────────────────────────────
    protected CenterUpsertDto Model    { get; set; } = new();
    protected bool IsLoading           { get; set; } = true;
    protected bool IsSaving            { get; set; } = false;
    protected string? ErrorMsg         { get; set; }

    // أيام الدوام — Checkboxes
    protected readonly string[] AllDays = { "السبت", "الأحد", "الاثنين", "الثلاثاء", "الأربعاء", "الخميس", "الجمعة" };
    protected HashSet<string> SelectedDays { get; set; } = new();

    // ── تحميل البيانات ─────────────────────────────────────────
    protected override async Task OnInitializedAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity!.IsAuthenticated)
        {
            var roles = user.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value!;
            IsAdmin = roles == SharedLib.Fixed.Roles.Admin;
        }

        try
        {
            WHoures =await LookupValueApi.GetByValueType(SharedLib.Fixed.LookupTypes.WHoures);
            if (center == null) 
             center = await CenterApi.GetMyCenter();
            MapToModel(center);
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            ErrorMsg = "لا يوجد مركز مرتبط بحسابك. تواصل مع المدير.";
        }
        catch (Exception ex)
        {
            ErrorMsg = $"حدث خطأ أثناء تحميل البيانات: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    // ── تحويل Center → Model ───────────────────────────────────
    private void MapToModel(Center c)
    {
        Model.Id          = c.Id;
        Model.Name        = c.Name;
        Model.CenterCode  = c.CenterCode;
        Model.Address     = c.Address;
        Model.DaysOfWeek  = c.DaysOfWeek;
        Model.Rooms       = c.Rooms       ?? 0;
        Model.Tarpaulins  = c.Tarpaulins  ?? 0;
        Model.OtherSpaces = c.OtherSpaces ?? 0;
        Model.Comments    = c.Comments;
        Model.WHours    = c.WhoursId;
        Model.EnName    = c.EnName;
        Model.SortOrder    = c.SortOrder;
        Model.BuildingCode    = c.BuildingCode;

        // تحليل DaysOfWeek إلى Checkboxes
        SelectedDays.Clear();
        if (!string.IsNullOrWhiteSpace(c.DaysOfWeek))
        {
            foreach (var day in c.DaysOfWeek.Split('-', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                if (AllDays.Contains(day))
                    SelectedDays.Add(day);
        }
    }
    private Center MapToCenter(CenterUpsertDto Model)
    {
        Center c = new();
        c.Id = Model.Id;
        c.Name = Model.Name;      
        c.CenterCode = Model.CenterCode;
        c.Address = Model.Address;
        c.DaysOfWeek = Model.DaysOfWeek;
        c.Rooms = Model.Rooms;
        c.Tarpaulins = Model.Tarpaulins;
        c.OtherSpaces = Model.OtherSpaces;
        c.Comments = Model.Comments;
        c.DaysOfWeek = Model.DaysOfWeek;
        c.EnName = Model.EnName;
        c.SortOrder = Model.SortOrder;
        c.BuildingCode = Model.BuildingCode;
        return c;
    }

    // ── تبديل يوم ─────────────────────────────────────────────
    protected void ToggleDay(string day)
    {
        if (!SelectedDays.Remove(day))
            SelectedDays.Add(day);

        // ترتيب الأيام حسب الترتيب الطبيعي
        Model.DaysOfWeek = string.Join("-",
            AllDays.Where(d => SelectedDays.Contains(d)));
    }

    protected bool IsDaySelected(string day) => SelectedDays.Contains(day);

    // ── حفظ ───────────────────────────────────────────────────
    protected async Task HandleSubmit()
    {
        if (IsSaving) return;
        IsSaving = true;

        try
        {
            GeneralResponse result;
            if (IsAdmin)
            {
                if (Model.Id == 0)
                    result = await CenterApi.Insert(MapToCenter(Model));
                else
                    result = await CenterApi.Update(Model);
            }
            else
                result = await CenterApi.UpdateMyCenter(Model);
 
            if (result.Success)
            {
                MudSnackbar.Add(result.Message, Severity.Success);
            }
            else
            {
                MudSnackbar.Add(result.Message, Severity.Error);
            }
        }
        catch (Exception ex)
        {
            MudSnackbar.Add($"فشل الحفظ: {ex.Message}", Severity.Error);
        }
        finally
        {
            IsSaving = false;
        }
    }
}
