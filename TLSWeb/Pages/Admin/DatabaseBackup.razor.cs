using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using Refit;
using SharedLib.DTOs;
using SharedLib.Entities;
using System.Reflection;
using TLSClientSharedLib.Services.Apis;

namespace TLSWeb.Pages.Admin;   

public partial class DatabaseBackup
{
    [Inject] private IBackupApi BackupApi { get; set; } = default!;
    [Inject] private IJSRuntime JS { get; set; } = default!;

    private BackupFormModel model = new();
    private List<BackupRecord> backupHistory = [];
    private List<DatabaseInfo> availableDatabases = [];
    private List<TableInfo> availableTables = [];
    private BackupRecord? lastBackup;

    private bool isRunning;
    private bool isLoadingHistory;
    private int dbSizeMB;
    private double totalBackupSizeGB;
    private IJSObjectReference? jsModule;
    protected override async Task OnInitializedAsync()
    {
        InitDefaults();
        await LoadDataAsync();
    }

    private void InitDefaults()
    {
        model = new BackupFormModel
        {
            BackupName = $"backup_{DateTime.Now:yyyy_MM_dd_HHmm}",
            SavePath = @"D:\Backups\TempEdu\",
            BackupType = "Full",
            Format = "bak",
            Compress = true,
            VerifyAfter = true,
            NotifyOnComplete = true,
        };

        availableTables =
        [
            new("Students",    "الطلاب",      true),
            new("Employees",   "الموظفون",    true),
            new("Centers",     "المراكز",     true),
            new("Classes",     "الصفوف",      true),
            new("Attendance",  "الحضور",      true),
            new("AspNetUsers", "المستخدمون",  true),
        ];
    }

    private async Task LoadDataAsync()
    {
        isLoadingHistory = true;
        StateHasChanged();
        try
        {
            availableDatabases = await BackupApi.GetDatabasesAsync();
            model.DatabaseName = availableDatabases.FirstOrDefault()?.Name ?? string.Empty;

            backupHistory = await BackupApi.GetHistoryAsync();
            lastBackup = backupHistory.FirstOrDefault(b => b.CreatedAt.Date == DateTime.Today);

            var sizeResult = await BackupApi.GetDbSizeAsync(model.DatabaseName);
            dbSizeMB = sizeResult.SizeMB;

            totalBackupSizeGB = Math.Round(backupHistory.Sum(b => b.SizeMB) / 1024.0, 2);
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"خطأ في تحميل البيانات: {ex.ReasonPhrase}", Severity.Error);
        }
        finally
        {
            isLoadingHistory = false;
            StateHasChanged();
        }
    }


    private async Task StartBackupAsync()
    {
        if (isRunning) return;
        isRunning = true;
        StateHasChanged();

        try
        {
            var request = model.ToRequest(
                availableTables.Where(t => t.IsSelected).Select(t => t.Name).ToList()
            );

            var result = await BackupApi.CreateBackupAsync(request);

            backupHistory.Insert(0, result);
            totalBackupSizeGB = Math.Round(backupHistory.Sum(b => b.SizeMB) / 1024.0, 2);
            lastBackup = result;

            MudSnackbar.Add("تمت النسخة الاحتياطية بنجاح!", Severity.Success);
            ResetForm();
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.InternalServerError)
        {
            MudSnackbar.Add("فشلت النسخة الاحتياطية على السيرفر", Severity.Error);
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"خطأ: {ex.ReasonPhrase}", Severity.Error);
        }
        finally
        {
            isRunning = false;
            StateHasChanged();
        }
    }


    private async Task DownloadBackupAsync(BackupRecord backup)
    {
        try
        {
            var content = await BackupApi.DownloadBackupAsync(backup.Id);
            var bytes = await content.ReadAsByteArrayAsync();

            await JS.InvokeVoidAsync(
                "downloadFileFromBytess",
                backup.FileName,
                Convert.ToBase64String(bytes),
                "application/octet-stream");

            //await jsModule!.InvokeVoidAsync(
            //   "downloadFileFromBytes",
            //   backup.FileName,
            //   Convert.ToBase64String(bytes),
            //   "application/octet-stream");
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"تعذّر التحميل: {ex.ReasonPhrase}", Severity.Error);
        }
    }

    // ── Delete ────────────────────────────────────────────
    private async Task DeleteBackupAsync(BackupRecord backup)
    {
        bool confirmed = await JS.InvokeAsync<bool>(
            "confirm", $"هل أنت متأكد من حذف النسخة: {backup.FileName}؟");
        if (!confirmed) return;

        try
        {
            await BackupApi.DeleteBackupAsync(backup.Id);
            backupHistory.Remove(backup);
            MudSnackbar.Add("تم حذف النسخة بنجاح", Severity.Success);
        }
        catch (ApiException ex)
        {
            MudSnackbar.Add($"فشل الحذف: {ex.ReasonPhrase}", Severity.Error);
        }
    }

    private void ResetForm() => model.Reset();
    public async ValueTask DisposeAsync()
    {
        if (jsModule is not null)
            await jsModule.DisposeAsync();
    }
}