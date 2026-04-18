// ╔══════════════════════════════════════════════════════════════╗
// ║  TLSWeb/Pages/Dashboard/AdminDashboard.razor.cs              ║
// ╚══════════════════════════════════════════════════════════════╝

using Microsoft.AspNetCore.Components;
using SharedLib.DTOs;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Pages.Dashboard;

public partial class AdminDashboard : ComponentBase
{
    [Inject] private IAdminDashboardApi AdminDashboardApi { get; set; } = default!;

    protected AdminDashboardDto?  Stats     { get; private set; }
    protected bool                IsLoading { get; private set; } = true;
    protected string?             ErrorMsg  { get; private set; }
    private string searchText = string.Empty;

    // للتبديل بين عرض الملخص الكلي وتفاصيل مركز معين
    protected long? SelectedCenterId { get; private set; } = null;
    protected List<CenterSummaryDto> AllCenters { get;  set; }
    protected CenterSummaryDto? SelectedCenter
        => Stats?.Centers.FirstOrDefault(c => c.CenterId == SelectedCenterId);

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Stats = await AdminDashboardApi.Get();
            AllCenters=Stats.Centers.ToList();
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

    protected void SelectCenter(long centerId)
        => SelectedCenterId = (SelectedCenterId == centerId) ? null : centerId;

    protected void ClearSelection()
        => SelectedCenterId = null;
    private async Task OnSearchChanged()
    {
        Stats.Centers=AllCenters.Where(x=>x.CenterName.ToLower().Contains(searchText.ToLower()) || 
        x.CenterManager.ToLower().Contains(searchText.ToLower())||
        x.CenterCode.Contains(searchText)).ToList();

        //currentPage = 1;
        //await LoadDataAsync();
    }
}
