// ╔══════════════════════════════════════════════════════════════╗
// ║  TLSWeb/Services/ExcelDownloadService.cs                     ║
// ║  يتلقى bytes من الـ API ويطلق التنزيل في المتصفح            ║
// ╚══════════════════════════════════════════════════════════════╝

using Microsoft.JSInterop;

namespace TLSWeb.Services;

public class ExcelDownloadService
{
    private readonly IJSRuntime _js;

    public ExcelDownloadService(IJSRuntime js) => _js = js;

    /// <summary>
    /// يستقبل HttpResponseMessage من Refit ويُنزّل الملف
    /// </summary>
    public async Task DownloadFromResponse(HttpResponseMessage response, string fallbackFileName)
    {
        // استخراج اسم الملف من Content-Disposition header
        var fileName = fallbackFileName;
        if (response.Content.Headers.ContentDisposition?.FileNameStar is { } fn)
            fileName = Uri.UnescapeDataString(fn);
        else if (response.Content.Headers.ContentDisposition?.FileName is { } fn2)
            fileName = fn2.Trim('"');

        var bytes = await response.Content.ReadAsByteArrayAsync();
        await DownloadBytes(bytes, fileName);
    }

    /// <summary>
    /// يُنزّل byte[] مباشرةً كملف Excel
    /// </summary>
    public async Task DownloadBytes(byte[] bytes, string fileName)
    {
        // استدعاء دالة JavaScript لإنشاء رابط تنزيل
        await _js.InvokeVoidAsync("BlazorDownloadFile", fileName, 
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            bytes);
    }
}
