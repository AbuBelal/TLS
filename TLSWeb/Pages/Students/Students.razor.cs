using Microsoft.AspNetCore.Components;
using MudBlazor;
using SharedLib.DTOs;
using SharedLib.Entities;
using TLSWeb.Helpers;

namespace TLSWeb.Pages.Students;

public partial class Students : ComponentBase
{
    // ====== البيانات ======
    private List<StudentDto> students = [];
    private bool isLoading = true;

    // ====== الفلاتر ======
    private string searchText = string.Empty;
    private string selectedGender = string.Empty;
    private string selectedLevel = string.Empty;
    private string selectedCenter = string.Empty;
    private short SelectedSection = 0;

    // خيارات الفلاتر (من الـ LookupValues)
    private List<string> genderOptions = [];
    private List<string> levelOptions = [];
    private List<string> CenterOptions = [];

    // ====== Server-Side Pagination ======
    private int currentPage = 1;
    private int pageSize = 10;
    private int totalCount;
    private int totalPages;

    // ====== الحذف ======
    private bool showDeleteModal;
    private bool isDeleting;
    private StudentDto? studentToDelete;
    // ====== حالة التصدير ======
    private bool isExportingFiltered = false;
    private bool isExportingAll = false;
    private bool showExportMenu = false;

    // ====== دورة الحياة ======
    protected override async Task OnInitializedAsync()
    {
        await LoadFilterOptions();
        await LoadData();
    }

    /// <summary>
    /// تحميل خيارات الفلاتر من LookupValues
    /// </summary>
    private async Task LoadFilterOptions()
    {
        try
        {
            var lookups = await LookupValueApi.GetAll();
            genderOptions = lookups
                .Where(l => l.ValueType == "Gender" && l.IsActive)
                .OrderBy(l => l.SortOrder)
                .Select(l => l.Name!)
                .ToList();

            levelOptions = lookups
                .Where(l => l.ValueType == "Level" && l.IsActive)
                .OrderBy(l => l.SortOrder)
                .Select(l => l.Name!)
                .ToList();

            CenterOptions = (await CenterApi.GetAll())
                .Select(c => c.Name!)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطأ في تحميل الفلاتر: {ex.Message}");
        }
    }

    /// <summary>
    /// تحميل البيانات من الـ Backend مع الفلترة والتقسيم
    /// </summary>
    private async Task LoadData()
    {
        isLoading = true;
        StateHasChanged();

        try
        {
            var request = new StudentFilterRequest
            {
                Page = currentPage,
                PageSize = pageSize,
                SearchText = searchText,
                Gender = selectedGender,
                Level = selectedLevel,
                Center = selectedCenter,
                Section = SelectedSection
            };

            var response = await StudentApi.GetPaginated(request);

            students = response.Items;
            totalCount = response.TotalCount;
            totalPages = response.TotalPages;
            currentPage = response.CurrentPage;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطأ في تحميل البيانات: {ex.Message}");
            students = [];
            totalCount = 0;
            totalPages = 1;
        }
        finally
        {
            isLoading = false;
            StateHasChanged();
        }
    }

    // ====== أحداث الفلترة ======
    private async Task OnFilterChanged()
    {
        currentPage = 1;
        await LoadData();
    }

    private async Task ResetFilters()
    {
        searchText = string.Empty;
        selectedGender = string.Empty;
        selectedLevel = string.Empty;
        SelectedSection = 0;
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

    private async Task OnPageSizeChanged()
    {
        currentPage = 1;
        await LoadData();
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

    // ====== التنقل ======
    private void OpenAddModal() =>
        NavManager.NavigateTo(PagesUris.StudentsPages.Add);

    private void OpenEditModal(long id) =>
        NavManager.NavigateTo($"{PagesUris.StudentsPages.Edit}/{id}");

    // ====== الحذف ======
    private void ConfirmDelete(StudentDto student)
    {
        studentToDelete = student;
        showDeleteModal = true;
    }

    private void CancelDelete()
    {
        studentToDelete = null;
        showDeleteModal = false;
    }

    private async Task DeleteConfirmed()
    {
        if (studentToDelete is null) return;
        isDeleting = true;

        try
        {
            await StudentApi.DeleteById(studentToDelete.Id);
            // إعادة تحميل البيانات من السيرفر
            await LoadData();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"خطأ في الحذف: {ex.Message}");
        }
        finally
        {
            isDeleting = false;
            showDeleteModal = false;
            studentToDelete = null;
        }
    }

    // ====== التصدير ======

    /// <summary>تصدير الطلاب المعروضين حسب الفلاتر الحالية</summary>
    /// 
     // ── بناء طلب الفلترة ────────────────────────────────────────
    private StudentFilterRequest BuildRequest() => new()
    {
        Page = currentPage,
        PageSize = pageSize,
        SearchText = searchText,
        Gender = selectedGender,
        Level = selectedLevel,
    };
    private async Task ExportFiltered()
    {
        if (isExportingFiltered) return;
        isExportingFiltered = true;
        showExportMenu = false;

        try
        {
            var request = BuildRequest();
            var response = await StudentApi.ExportFiltered(request);

            if (response.IsSuccessStatusCode)
                await ExcelDownloader.DownloadFromResponse(response, "طلاب_مفلتر.xlsx");
            else
                MudSnackbar.Add("فشل تصدير الملف", Severity.Error);
        }
        catch (Exception ex)
        {
            MudSnackbar.Add($"خطأ: {ex.Message}", Severity.Error);
        }
        finally
        {
            isExportingFiltered = false;
            StateHasChanged();
        }
    }

    /// <summary>تصدير جميع طلاب المركز بدون فلاتر</summary>
    private async Task ExportAll()
    {
        if (isExportingAll) return;
        isExportingAll = true;
        showExportMenu = false;

        try
        {
            var response = await StudentApi.ExportAll();

            if (response.IsSuccessStatusCode)
                await ExcelDownloader.DownloadFromResponse(response, "جميع_الطلاب.xlsx");
            else
                MudSnackbar.Add("فشل تصدير الملف", Severity.Error);
        }
        catch (Exception ex)
        {
            MudSnackbar.Add($"خطأ: {ex.Message}", Severity.Error);
        }
        finally
        {
            isExportingAll = false;
            StateHasChanged();
        }
    }

    private void ToggleExportMenu() => showExportMenu = !showExportMenu;
    private void CloseExportMenu() => showExportMenu = false;

    // نص وصف التصفية الحالية — يظهر في زر التصدير
    private string FilterDescription
    {
        get
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(searchText)) parts.Add($"بحث: {searchText}");
            if (!string.IsNullOrWhiteSpace(selectedGender)) parts.Add(selectedGender);
            if (!string.IsNullOrWhiteSpace(selectedLevel)) parts.Add(selectedLevel);
            return parts.Count > 0 ? string.Join(" | ", parts) : "بدون فلاتر";
        }
    }
}