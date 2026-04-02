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

    // للتبديل بين عرض الملخص الكلي وتفاصيل مركز معين
    protected long? SelectedCenterId { get; private set; } = null;
    protected CenterSummaryDto? SelectedCenter
        => Stats?.Centers.FirstOrDefault(c => c.CenterId == SelectedCenterId);

    protected override async Task OnInitializedAsync()
    {
        try
        {
            Stats = await AdminDashboardApi.Get();
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
}
