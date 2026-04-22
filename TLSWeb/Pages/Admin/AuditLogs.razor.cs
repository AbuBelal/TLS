using Microsoft.AspNetCore.Components;
using SharedLib.DTOs;
using TLSWeb.Helpers;

namespace TLSWeb.Pages.Admin;

public partial class AuditLogs : ComponentBase
{
    private List<AuditLogDto> auditLogs = [];
    private bool isLoading = true;

    private string searchText = string.Empty;
    private string selectedUserId = string.Empty;
    private string ActionType = string.Empty;
    private string Details = string.Empty;
    private DateTime? fromDate;
    private DateTime? toDate;
    private bool sortDescending = true;

    private int currentPage = 1;
    private int pageSize = 20;
    private int totalCount;
    private int totalPages = 1;

    protected override async Task OnInitializedAsync()
        => await LoadDataAsync();

    private async Task LoadDataAsync()
    {
        isLoading = true;
        try
        {
            var request = new AuditLogFilterRequest
            {
                SearchText = string.IsNullOrWhiteSpace(searchText) ? null : searchText,
                UserId = string.IsNullOrWhiteSpace(selectedUserId) ? null : selectedUserId,
                Action = string.IsNullOrWhiteSpace(ActionType) ? null : ActionType,
                Details = string.IsNullOrWhiteSpace(Details) ? null : Details,
                FromDate = fromDate,
                ToDate = toDate,
                SortDescending = sortDescending,
                PageNumber = currentPage,
                PageSize = pageSize
            };

            var response = await AuditLogApi.GetPaginated(request);
            auditLogs = response.Items;
            totalCount = response.TotalCount;
            totalPages = Math.Max(1, response.TotalPages);
            currentPage = response.CurrentPage;
        }
        finally { isLoading = false; }
    }

    private async Task OnSearchChanged() { currentPage = 1; await LoadDataAsync(); }
    private async Task OnFilterChangedAsync() { currentPage = 1; await LoadDataAsync(); }

    private async Task ResetFiltersAsync()
    {
        searchText = string.Empty;
        selectedUserId = string.Empty;
        ActionType = string.Empty;
        Details = string.Empty;
        fromDate = null;
        toDate = null;
        sortDescending = true;
        currentPage = 1;
        await LoadDataAsync();
    }

    private async Task OnPageSizeChangedAsync() { currentPage = 1; await LoadDataAsync(); }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > totalPages) return;
        currentPage = page;
        await LoadDataAsync();
    }

    private IEnumerable<int> GetPageNumbers()
    {
        const int maxVisible = 5;
        if (totalPages <= maxVisible) return Enumerable.Range(1, totalPages);
        var half = maxVisible / 2;
        var startPage = Math.Max(1, currentPage - half);
        var endPage = Math.Min(totalPages, startPage + maxVisible - 1);
        if (endPage - startPage < maxVisible - 1)
            startPage = Math.Max(1, endPage - maxVisible + 1);
        return Enumerable.Range(startPage, endPage - startPage + 1);
    }
}
