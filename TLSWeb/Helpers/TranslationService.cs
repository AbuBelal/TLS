using System.Net.Http.Json;

public class TranslationService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "https://api.mymemory.translated.net/get";

    public TranslationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string> TranslateNameAsync(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName)) return "";

        // MyMemory API يطلب النص، وزوج اللغات (من العربية إلى الإنجليزية)
        var url = $"{BaseUrl}?q={Uri.EscapeDataString(fullName)}&langpair=ar|en";

        var response = await _httpClient.GetFromJsonAsync<MyMemoryResponse>(url);

        // المخرج يكون غالباً في 'translatedText'
        return response?.ResponseData?.TranslatedText ?? "Error in translation";
    }
}

// الكلاسات الخاصة باستقبال بيانات JSON
public class MyMemoryResponse
{
    public ResponseData ResponseData { get; set; }
}

public class ResponseData
{
    public string TranslatedText { get; set; }
}