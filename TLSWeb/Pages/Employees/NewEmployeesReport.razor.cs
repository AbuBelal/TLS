using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib.DTOs;
using TLSWeb.Helpers;
using TLSWeb.Services;

namespace TLSWeb.Pages.Employees;

public partial class NewEmployeesReport : ComponentBase
{
    // ====== البيانات ======
    private List<EmployeeListItemDto> employees = [];
    private bool isLoading;

    // ====== فلتر التاريخ ======
    private DateOnly? fromDate;
    private DateOnly? toDate;

    // ====== Pagination ======
    private int currentPage = 1;
    private int pageSize = 25;
    private int totalCount;
    private int totalPages;

    // ====== التصدير ======
    private bool isExporting;

    // ====== دورة الحياة ======
    protected override async Task OnInitializedAsync()
    {
        //fromDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
        fromDate = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            var request = new EmployeeFilterRequest
            {
                PageNumber = currentPage,
                PageSize = pageSize,
                FromDate = fromDate
            };

            var response = await EmployeeApi.GetPaginated(request);

            employees = response.Items.OrderByDescending(e => e.AddedDate).ToList();
            totalCount = response.TotalCount;
            totalPages = response.TotalPages;
            currentPage = response.CurrentPage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطأ في تحميل البيانات: {ex.Message}");
            employees = [];
            totalCount = 0;
            totalPages = 1;
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    // ====== أحداث التاريخ ======
    private async Task OnDateChanged()
    {
        currentPage = 1;
        await LoadData();
    }

    private async Task ResetDate()
    {
        fromDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, 1);
        toDate = null;
        currentPage = 1;
        await LoadData();
    }

    // ====== Pagination ======
    private async Task GoToPage(int page)
    {
        if (page >= 1 && page <= totalPages && page != currentPage)
        {
            currentPage = page;
            await LoadData();
        }
    }

    private IEnumerable<int> GetPageNumbers()
    {
        const int max = 5;
        if (totalPages <= max)
            return Enumerable.Range(1, totalPages);

        int start = Math.Max(1, currentPage - max / 2);
        int end = Math.Min(totalPages, start + max - 1);
        if (end - start < max - 1)
            start = Math.Max(1, end - max + 1);

        return Enumerable.Range(start, end - start + 1);
    }

    // ====== التصدير ======
    private async Task ExportReport()
    {
        if (isExporting) return;
        isExporting = true;

        try
        {
            var request = new EmployeeFilterRequest
            {
                PageNumber = 1,
                PageSize = 99999,
                FromDate = fromDate
            };

            var response = await EmployeeApi.ExportFiltered(request);

            if (response.IsSuccessStatusCode)
            {
                var datePart = fromDate?.ToString("yyyy-MM-dd") ?? "all";
                await ExcelDownloader.DownloadFromResponse(response, $"موظفون_مضافون_من_{datePart}.xlsx");
            }
            else
            {
                MudSnackbar.Add("فشل تصدير الملف", Severity.Error);
            }
        }
        catch (Exception ex)
        {
            MudSnackbar.Add($"خطأ: {ex.Message}", Severity.Error);
        }
        finally
        {
            isExporting = false;
            StateHasChanged();
        }
    }
}
