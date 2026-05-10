using HtmlAgilityPack;
using Microsoft.AspNetCore.Mvc;
using SharedLib.DTOs;
using System.Net;
using System.Xml;

[ApiController]
[Route("api/[controller]")]
public class EmisController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public EmisController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("fetch-Std-Info")]
    public async Task<ActionResult<StudentEmisDto>> GetResult(StudentEmisRequest request)
    {
        var client = _httpClientFactory.CreateClient();

        // تجهيز البيانات المطلوبة من قبل سيرفر الأونروا
        var formData = new Dictionary<string, string>
        {
            { "IdentityNo", request.IdentityNo },
            { "birthYear", request.BirthYear }
        };

        var content = new FormUrlEncodedContent(formData);

        // إرسال الطلب للموقع الرسمي
        var response = await client.PostAsync("https://emis.unrwa.org/Result/StudentsResult", content);

        if (!response.IsSuccessStatusCode)
            return BadRequest("تعذر الاتصال بسيرفر النتائج.");

        var html = await response.Content.ReadAsStringAsync();
        var doc = new HtmlDocument();
        doc.LoadHtml(html);

        try
        {
            // استخراج البيانات باستخدام XPath بناءً على هيكلية الـ HTML التي أرفقتها
            var result = new StudentEmisDto
            {
                IdentityNo = doc.DocumentNode.SelectSingleNode("//input[@id='identity-no']")?.GetAttributeValue("value", ""),
                StudentId = doc.DocumentNode.SelectSingleNode("//input[@id='StudentId']")?.GetAttributeValue("value", ""),

                // استخراج النصوص من داخل وسوم <p1>
                ResultStatus = ExtractTextByLabel(doc, "النتيجه"),
                FullName = WebUtility.HtmlDecode(ExtractTextByLabel(doc, "اسم الطالب كاملا")),
                SchoolName = WebUtility.HtmlDecode( ExtractTextByLabel(doc, "المدرسة")),
                Grade = ExtractTextByLabel(doc, "الصف"),
                Section = ExtractTextByLabel(doc, "الشعبة"),
                Mobile = ExtractTextByLabel(doc, "موبايل التواصل"),
                WhatsAppGroup = doc.DocumentNode.SelectSingleNode("//a[contains(@href, 'chat.whatsapp.com')]")?.GetAttributeValue("href", "")
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            return NotFound( new StudentEmisDto());
        }
    }

    // وظيفة مساعدة للبحث عن النص بناءً على العنوان داخل الـ Label
    private string ExtractTextByLabel(HtmlDocument doc, string labelText)
    {
        var node = doc.DocumentNode.SelectSingleNode($"//label[contains(text(), '{labelText}')]/p1");
        return node?.InnerText.Trim() ?? "غير متوفر";
    }
}