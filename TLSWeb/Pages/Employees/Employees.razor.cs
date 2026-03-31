using Microsoft.AspNetCore.Components;
using SharedLib.DTOs;
using TLSWeb.Helpers;

namespace TLSWeb.Pages.Employees;

public partial class Employees : ComponentBase
{

    private List<EmployeeListItemDto> employees = [];
    private bool isLoading = true;

    private string searchText = string.Empty;
    private string selectedGender = string.Empty;
    private string selectedJob = string.Empty;

    private List<string> genderOptions = [];
    private List<string> jobOptions = [];

    private int currentPage = 1;
    private int pageSize = 10;
    private int totalCount;
    private int totalPages = 1;

    private bool showDeleteModal;
    private bool isDeleting;
    private EmployeeListItemDto? employeeToDelete;

    protected override async Task OnInitializedAsync()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        isLoading = true;
        try
        {
            var request = new EmployeeFilterRequest
            {
                SearchText = searchText,
                Gender = selectedGender,
                Job = selectedJob,
                PageNumber = currentPage,
                PageSize = pageSize
            };

            var response = await EmployeeApi.GetPaginated(request);

            employees = response.Items;
            totalCount = response.TotalCount;
            totalPages = Math.Max(1, response.TotalPages);
            currentPage = response.CurrentPage;

            genderOptions = response.GenderOptions;
            jobOptions = response.JobOptions;
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task OnSearchChanged()
    {
        currentPage = 1;
        await LoadDataAsync();
    }

    private async Task OnFilterChangedAsync()
    {
        currentPage = 1;
        await LoadDataAsync();
    }

    private async Task ResetFiltersAsync()
    {
        searchText = string.Empty;
        selectedGender = string.Empty;
        selectedJob = string.Empty;
        currentPage = 1;
        await LoadDataAsync();
    }

    private async Task OnPageSizeChangedAsync()
    {
        currentPage = 1;
        await LoadDataAsync();
    }

    private async Task GoToPageAsync(int page)
    {
        if (page < 1 || page > totalPages)
        {
            return;
        }

        currentPage = page;
        await LoadDataAsync();
    }

    private IEnumerable<int> GetPageNumbers()
    {
        const int maxVisible = 5;

        if (totalPages <= maxVisible)
        {
            return Enumerable.Range(1, totalPages);
        }

        var half = maxVisible / 2;
        var startPage = Math.Max(1, currentPage - half);
        var endPage = Math.Min(totalPages, startPage + maxVisible - 1);

        if (endPage - startPage < maxVisible - 1)
        {
            startPage = Math.Max(1, endPage - maxVisible + 1);
        }

        return Enumerable.Range(startPage, endPage - startPage + 1);
    }

    private void OpenAddPage()
    {
        NavManager.NavigateTo(PagesUris.EmployeesPages.Add);
    }

    private void OpenEditPage(long id)
    {
        NavManager.NavigateTo($"{PagesUris.EmployeesPages.Edit}/{id}");
    }

    private void ConfirmDelete(EmployeeListItemDto employee)
    {
        employeeToDelete = employee;
        showDeleteModal = true;
    }

    private void CancelDelete()
    {
        employeeToDelete = null;
        showDeleteModal = false;
    }

    private async Task DeleteConfirmedAsync()
    {
        if (employeeToDelete is null)
        {
            return;
        }

        isDeleting = true;
        try
        {
            await EmployeeApi.DeleteById(employeeToDelete.Id);
            showDeleteModal = false;

            if (employees.Count == 1 && currentPage > 1)
            {
                currentPage--;
            }

            await LoadDataAsync();
        }
        finally
        {
            isDeleting = false;
            employeeToDelete = null;
        }
    }
}