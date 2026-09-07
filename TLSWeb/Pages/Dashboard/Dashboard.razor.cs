using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using SharedLib.DTOs;
using System.Security.Claims;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Pages.Dashboard;

public partial class Dashboard : ComponentBase
{
    protected CenterDashboardDto? Stats { get; private set; }
    protected bool IsLoading { get; private set; } = true;

    protected override async Task OnInitializedAsync()
    {
        var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
        var userId = authState.User.FindFirstValue(ClaimTypes.NameIdentifier);
        CenterDashboardDto? localStats;
        try
        {
        if (!string.IsNullOrEmpty(userId))
           Stats = await DashboardApi.Get();
        }
        catch (Exception ex)
        {
            //throw;
        }
        
        IsLoading = false;
    }
}