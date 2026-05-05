using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace APIServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentProxyController : ControllerBase
    {
        private readonly IHttpClientFactory _clientFactory;

        public StudentProxyController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        [HttpGet("fetch-students")]
        public async Task<IActionResult> GetStudents()
        {
            // 1. إعداد الـ Handler لمنع التعامل التلقائي مع الكوكيز والسماح لنا بإرسالها يدوياً
            using var handler = new HttpClientHandler
            {
                UseCookies = false, // مهم جداً لكي لا يتجاهل الـ Header الذي سنضيفه
                AllowAutoRedirect = true
            };

            using var client = new HttpClient(handler);
            // ضع النص الذي نسخته من المتصفح هنا بالكامل
            string AspNetCookies = "rDpJ7su5Ckf_9o6D8dFYtK-xMDlvHqDO4Z0pT9TWj5Dm3eZeqvanN0ro4wKWtBq3rdpNpg6AGWpiO3xWix9DRDOp1f9wk1Y01XK0GzeAKyrLBihdbi2LWhyy8t3s86U2DeXYoxLdh6Lgd7DLDFIyVpmk0doL9a-LxBceU7GWnRbGYtvJM5UwFhH434-5BRK5DCvBY7B8bH2AhFQEB8IhdBaPfU8Nkh3XyZoBxqNshXR37jL9q4WpxN_73rb5sH7mnMwtvj7s1ykF7dvucB1dCTFHT9ZtKNCW7wVilFaFK4SX8vSvvBgIS9BnpKZv9yibKgNJkSxh7b0B9FeyTpPxIar7TBuwsidWOONuONJlpHIojj0GbcDLOXh1NkAX5FqnKvPuYh7oC4D6OW8dTFnTFv_cFigFrsiCyKlhe8Bc7EsKhU_3U6A_pqz6YQNlbR7uT1c4DHBCV0ejhoMfkntu1_oDiahjRxXsqE_CAmpFWzvyYe4BTXowPj7j7S2QME_1fbjQjj8B01T1BCCIR1XPTEGAjjTCG28wrpuK6SRW0T78aPrP-fh2YljEmVeRQ6YiIDbMZoaY6pcjxrdvUsYT5Xn7tRsi22eX4i2Kk0iuKC9zLRp7sY2SGi6nlLOfEfw8dSLlunsXN_8eDl9brb6dR7Oaw-NYuCfJyuS8KsUXO5e9Xob4WRkf0-1JSL23FsF1u3hGLrnsL7KG8hL-kF6x_ldhYzhZ7fXXXnd3LDqp6II4_lAXMmSOm5KQG4VyC3rSChzHU84dNxwqNUXgSs7Yk2FnoTlG5Ovc5tL2mKvq4INY4_GC5YUqG_vGBNntj-pyjkwuRicG8dRv9lFQYtIgKenDgMRCdiTOY3TLsi-jEyqGyD4DqEkTnoFfdRizHNy4zwhVnImFrfsLTb-Bl6huvxrqDfnhTyus2JbV-k57-OqKgf8GhImWEFW0bAj6NifyZXgnm4DKATaiOI7JjCOPNyrpKwWHRLbdVUhTRF8vLh9FJLebh9ULbatVBtutFGdNz3UNJNt2Ie8-eFLNZK8GxrUUANl1o_lyMtnva13d0v0WwyRwEgAEhc8iTK0CjuLVh5-toFBnu_E-aWtAaiuZboSlx2bcRI_leU6rSR_fr-WXa6dfA_N7RTOqacfoePzfDpyReu9qA1Utt8m8ugMtjdwh7f9BmdxsZ1IpcSezeeY_SiRsGJCMEUYpDCdIL3rubqMMdbaEjvgQtUsH5RtCOmXMGj0wctup0vxmXaMZUtmyuEmg_xX_W-uSiPfCdFvQCLL3plHdc6wOYxJfKAounPTHQPGXDVLBPlMWFSWjp_diKIg6b6vofHlQa_O-plTJEmJjuR6ddo7fySkuhrCk0sOHNIxP2ga7XfeyYubMA3gNh4VrbDSwy5yRApmVpArA9E3AAv6LhDclyceWpbbckvXjyHw6XrHlHepwbnzOZdLCvDdypEuGTb4RxZuncQEP3DTmUTMe5a0AGzhgR_BXn_nV1-RnSWQTFuexJVPStWU9YKPsG5lay2h_L6lMwazmsqkzVVxgWf974mCs6CGzoPzAccvEAshK1DhJisYWNAIJpS41902LOKEf4Z4GjjYbFMQw8ukpyXDcK4mQIkJE2RHQ8nr2G1vYueUcXhc3Ne5hNVii0-GjtXxq_32DJtoitn_lVzWtWlrvqjJhCHhFGYI6XhhOR-opVF0iU1KdHTzIJDUHJw0YOQ8LlRcbYyuykNz1SBXTvB99ill3ZM63KyYBBAB9hOcxjBgJd1WBaDVLm5jFWdToNMNN8n0umW1qlro4n5d0HXVCTGyc8uZySPR_LXczHrSpKP8erJlN9KbsuCjdPv8iUIYbyTsRT67YaJxdzmbEe_Gdl-REqHBCW4V6f9qbj8i4Es9aSt_ZnNdJab9bF45N9xOJmO41rxxIUYNJTcarK0DVkKUf61VMCSD4Wv7Npz9VYeVPJD8cn9DwQhLsEE3vsqv05ZM3_i-MF2RJ4_bYNUeFvbiwtfr6_9VszKJaeAIuK-8iY9NqDwPik3ApHkHGNyoy5Neg0_72sLvHiGQAeJb9wdl0pE0EoFZRZxF3oLecwkd8zIB28WJAjX92wgvoiYfSgKJoAyoinh0E7jTmBPI0iTo7kWyGf_g3_G2KWlZH-nEPZFzxDgWdZTqCQP47ViiWNEUUmgDHrXUBwzXoajRxx7QbQPnupzWy8rl5asDtkAbEwjbf8sWMlaCpM7CfvFF4igQLMbbyEyKNjN6C_xvaSzt4vM2C4XQjo070mcRw3DA80kex8vrYSTfdxR9u0xrmpazTzDzFPHBptvh_Ya7_ZvoG6sAU8OFJ3skuk1oaouOjFVBQr6ibj3rVyfGsQRn5beEW2z3AQbEnrHCv-TcSx7ALeyoy8y9IYfVoGHDuPSU757fIYY8ZTNV0HGMkzmh-aSkXbtkz3jZNUhlIOpP-9RYYskfRksFHQf6HEFwAWc5GGezq-a-Xh15ZMOZY2tqiyrvYzxf44k7p0ENMC3wd4dj5fhjRd6H0FZ3EPAd8OpoGeX-l1gheUL38UmulssEih7vV8hg0he92gjBt_1iSfoS1ysc8x6aTGr6ppvmv8ofDnvW9UXHWhUF0HkqMXPQPW3OwuvHin7hSpAUO9CU_HZt8TbHfCyfrzOh3RG_ZbyCFsHABYA5Bzse9qx3p4Ects0rOExiMbknD6UNdNMGoNz89Ale5TIWMene_vvmSCzDMoD9k8X4tnTuOLUVKP3NN7EwKCZ-hfj01CGEA9DphwkhokTGj_GLzJCFE6ZeoWYvp3oNVpEWaOr01ZXw7gYu9TnjAiQjXKRLAWB3Un353f18PqTshQC2nQ0OPz7Zr3Vom4d1f6U_OrY_sJT26VmPwktaeXG5tbhapQWRUrDTz1mZ8WGWcCToVoWSa3378r0g4XC--8DjB_wglH4g5SYxt0wuDyhbOM9uQguZ90mmVW25wtmh94haWn_Jje-ysgeckKu5XhUw";
            string ASPNET_SessionId = "hjfvn2lhzzv33m1q5cbda3zf";
            string cf_clearance = "ba9ntEVTQ3IX7M8nGIdnF0UrH9EabBiupCRl2Rh7Byg-1763189729-1.2.1.1-mD3NgOH7dpCf4gaaRfjuth9PfdWXe0znSIe6No3tWUNO4i9Qz.CBwljwnceC_ADnX27Dd.lcjtsh0kpXOUnpFLF8y4vjwcIo2gKySqfthwiHJLdWMgaTf.ZfxbWHJlG62f4UaA1YIvdWaCv_iT5ulEUkxpogymHDFukrcPEYRWrh5I9LGJ.AMcfD3Kpy4xsqQzU1d0MHmn5sHM4yXn1rSfDAdEzZiOijS5DC9d9kF7k6rJ0MKDDTJZLx5nQcNWOH";
            string EMIS_Cookie = "fvvSs_l7b-Qcx1OhcCW9WozqnQKKsNk2UoeuSegHaslviyGQvwd66hxsHcNB0uda4b2tigoLG2n9O_qxUYoaZJ78RJwtnYw5B4qRTJXzXrTrzxu0mvgSd1xWqhnxKdk6g5ugGku_Ai33pkmIMGwIzcDDJl-_jpKFH98lEbSQImogwge_sOugq6DdxxpZ1MnagvUeW_hU_jxCNWaYEMnVS0T0g_VGvW6wV7v37cG24lOoRd0tzOpQYuCKzBVv8TmHwks5HtFbtDnAykEwQrjn-JWht9qiXgkugREGoM4LHRSJd9flS11ViyEev4onOyn0aC-tuOM-NknqfbrbXDnbt58LgI_yicEgKqNB0t7ZVOwmJ4bS90q2vKMcLJ3muCusSV3uBbrDZkK_lpKxeC_7CbvUqGmHQsdQ4LxnpAUPNsNVP8mA7Qo_JDQlmCGQtRMlv0Hk6ZY12f88SHNMyjmyCKpnkFNDQe3RgCkQk21cwh7BEazUcyVbxdErnOzKGFrfVQQ-YFKRPJYGTbMa4l6E4c_i9eB74A6T-dF7QP8u32NR5oe-c4pIeTdWnYVBSN0qvr-DAU4bv_po5OjJfiCA6zXPw34OjWZFRXYzERXmg_NRG-oQZgZNcM_ERKyfXHMaAkumHLI30DCZNvyG33S6qDvQxWyK5nrYdrW6iF8VkzsTMGUUZN7nSKoJ1oPwDkBJVdrkvUduQ7ie_sZtdNVAYd4KIulf8DI40HkL2ed9VSMuB8Db37i8RBhNNppDSZtQojZOJvGhcO0WlXGRHB8apODYUvBeNdSfgrppTyiKcVdIeJCvPNhcBXTLquILNZ1ewDZTSSVCqDr6XZ-CmXvRMYM7n5Oc2p1TfHpwK4tzMtxTKORlJQmriTIz8oiABmQPqH8oZlYASmEMDRAhLpr_Z_y2GqJqSeVb_7KnnBenhuy2UqfGKn1wVicyzUb6IGl2gm6kKhmz_yuApLNFiFwRMut2lk2nudA1jhvCQe4zEXduokgEpqAe4HFqlFq_j--EYkdS2KkizsXw2InWwfgbkjI3CTmr5-oFJVmlWeLDldlLcsETkWMyBfmzhTdNxmTPjoNLmiNvJJ9BaYrKN8HyfUqEH1m0tXlx3BgFDrhXTWYAHYHqeRHiOxDwTmUbsNOi3vOsnklJbYK-QhaTRnp-JW0myhBSjZ5TxlY1MfRECejzAg_aA9v5G3okjP7bIP5owyt9VZhcYqIbeyuuIvWV_DwoFns8NTpo6F6g6IZeQYMe4qmkjuITWhhUXBvMELhkuRQEQv5kPDgktyT1u0Qq4MON1r4-qauG9Rn9Z8B_ArE5c9QfoM4px_vGfLierRG2fGHjbdZkhpPosbS2hEbKDmFE34WVqDLynTjwo-sL-4OfkZTGIG2S7JNBLykvVQ8C";

            string myFullCookies = $".AspNet.Cookies={AspNetCookies}; ASP.NET_SessionId={ASPNET_SessionId}; cf_clearance={cf_clearance}; EMIS_Cookie={EMIS_Cookie};";
            string WebApiUlr = @"https://emis.unrwa.org/api/Student/SchoolStudentsAjax?callback=jQuery21105757827720421363_1777871542383&sEcho=5&iColumns=11&sColumns=%2C%2C%2C%2C%2C%2C%2C%2C%2C%2C&iDisplayStart=0&iDisplayLength=10&mDataProp_0=StudentId&bSortable_0=true&mDataProp_1=RefugeeStatusType&bSortable_1=true&mDataProp_2=IndividualRegistrationId&bSortable_2=true&mDataProp_3=ArabicFullNameWithoutQuoted&bSortable_3=true&mDataProp_4=FamilyRegistrationId&bSortable_4=true&mDataProp_5=Gender&bSortable_5=true&mDataProp_6=School&bSortable_6=true&mDataProp_7=Grade&bSortable_7=true&mDataProp_8=Section&bSortable_8=true&mDataProp_9=ClassLeader&bSortable_9=true&mDataProp_10=10&bSortable_10=true&iSortCol_0=3&sSortDir_0=asc&iSortingCols=1&SemesterId=2&StudentName=432338119&ScholasticYearId=20252&FieldId=2&AreaId=&SchoolId=&GradeId=&SectionId=&GenderId=&StudentRegistrationStatusId=2&SNeeds=0&IsSSN=&OrginalFeildId=&RefugeeStatusTypeId=&SFileStatus=&BFileStatus=&_=1777871542389";
            var request = new HttpRequestMessage(HttpMethod.Get, WebApiUlr);

            // 2. يجب مطابقة الـ User-Agent تماماً مع المتصفح الذي نسخت منه الكوكيز
            request.Headers.TryAddWithoutValidation("Cookie", myFullCookies);
            request.Headers.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/147.0.0.0 Safari/537.36");
            request.Headers.TryAddWithoutValidation("Accept", "*/*");
            request.Headers.TryAddWithoutValidation("Referer", "https://emis.unrwa.org/");

            try
            {
                var response = await client.SendAsync(request);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, "فشل الاتصال بموقع الأونروا، قد تكون الكوكيز انتهت.");
                }

                var content = await response.Content.ReadAsStringAsync();

                // 3. التحقق من أن المحتوى هو فعلاً JSONP وليس صفحة خطأ HTML
                if (content.Contains("jQuery") && content.Contains("("))
                {
                    int start = content.IndexOf("(") + 1;
                    int end = content.LastIndexOf(")");
                    string cleanJson = content.Substring(start, end - start);
                    return Content(cleanJson, "application/json");
                }

                return BadRequest("الاستجابة المستلمة ليست بتنسيق البيانات المتوقع.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"حدث خطأ داخلي: {ex.Message}");
            }




            ////////
            //var client = _clientFactory.CreateClient();

            //// ضع النص الذي نسخته من المتصفح هنا بالكامل
            //string AspNetCookies = "rDpJ7su5Ckf_9o6D8dFYtK-xMDlvHqDO4Z0pT9TWj5Dm3eZeqvanN0ro4wKWtBq3rdpNpg6AGWpiO3xWix9DRDOp1f9wk1Y01XK0GzeAKyrLBihdbi2LWhyy8t3s86U2DeXYoxLdh6Lgd7DLDFIyVpmk0doL9a-LxBceU7GWnRbGYtvJM5UwFhH434-5BRK5DCvBY7B8bH2AhFQEB8IhdBaPfU8Nkh3XyZoBxqNshXR37jL9q4WpxN_73rb5sH7mnMwtvj7s1ykF7dvucB1dCTFHT9ZtKNCW7wVilFaFK4SX8vSvvBgIS9BnpKZv9yibKgNJkSxh7b0B9FeyTpPxIar7TBuwsidWOONuONJlpHIojj0GbcDLOXh1NkAX5FqnKvPuYh7oC4D6OW8dTFnTFv_cFigFrsiCyKlhe8Bc7EsKhU_3U6A_pqz6YQNlbR7uT1c4DHBCV0ejhoMfkntu1_oDiahjRxXsqE_CAmpFWzvyYe4BTXowPj7j7S2QME_1fbjQjj8B01T1BCCIR1XPTEGAjjTCG28wrpuK6SRW0T78aPrP-fh2YljEmVeRQ6YiIDbMZoaY6pcjxrdvUsYT5Xn7tRsi22eX4i2Kk0iuKC9zLRp7sY2SGi6nlLOfEfw8dSLlunsXN_8eDl9brb6dR7Oaw-NYuCfJyuS8KsUXO5e9Xob4WRkf0-1JSL23FsF1u3hGLrnsL7KG8hL-kF6x_ldhYzhZ7fXXXnd3LDqp6II4_lAXMmSOm5KQG4VyC3rSChzHU84dNxwqNUXgSs7Yk2FnoTlG5Ovc5tL2mKvq4INY4_GC5YUqG_vGBNntj-pyjkwuRicG8dRv9lFQYtIgKenDgMRCdiTOY3TLsi-jEyqGyD4DqEkTnoFfdRizHNy4zwhVnImFrfsLTb-Bl6huvxrqDfnhTyus2JbV-k57-OqKgf8GhImWEFW0bAj6NifyZXgnm4DKATaiOI7JjCOPNyrpKwWHRLbdVUhTRF8vLh9FJLebh9ULbatVBtutFGdNz3UNJNt2Ie8-eFLNZK8GxrUUANl1o_lyMtnva13d0v0WwyRwEgAEhc8iTK0CjuLVh5-toFBnu_E-aWtAaiuZboSlx2bcRI_leU6rSR_fr-WXa6dfA_N7RTOqacfoePzfDpyReu9qA1Utt8m8ugMtjdwh7f9BmdxsZ1IpcSezeeY_SiRsGJCMEUYpDCdIL3rubqMMdbaEjvgQtUsH5RtCOmXMGj0wctup0vxmXaMZUtmyuEmg_xX_W-uSiPfCdFvQCLL3plHdc6wOYxJfKAounPTHQPGXDVLBPlMWFSWjp_diKIg6b6vofHlQa_O-plTJEmJjuR6ddo7fySkuhrCk0sOHNIxP2ga7XfeyYubMA3gNh4VrbDSwy5yRApmVpArA9E3AAv6LhDclyceWpbbckvXjyHw6XrHlHepwbnzOZdLCvDdypEuGTb4RxZuncQEP3DTmUTMe5a0AGzhgR_BXn_nV1-RnSWQTFuexJVPStWU9YKPsG5lay2h_L6lMwazmsqkzVVxgWf974mCs6CGzoPzAccvEAshK1DhJisYWNAIJpS41902LOKEf4Z4GjjYbFMQw8ukpyXDcK4mQIkJE2RHQ8nr2G1vYueUcXhc3Ne5hNVii0-GjtXxq_32DJtoitn_lVzWtWlrvqjJhCHhFGYI6XhhOR-opVF0iU1KdHTzIJDUHJw0YOQ8LlRcbYyuykNz1SBXTvB99ill3ZM63KyYBBAB9hOcxjBgJd1WBaDVLm5jFWdToNMNN8n0umW1qlro4n5d0HXVCTGyc8uZySPR_LXczHrSpKP8erJlN9KbsuCjdPv8iUIYbyTsRT67YaJxdzmbEe_Gdl-REqHBCW4V6f9qbj8i4Es9aSt_ZnNdJab9bF45N9xOJmO41rxxIUYNJTcarK0DVkKUf61VMCSD4Wv7Npz9VYeVPJD8cn9DwQhLsEE3vsqv05ZM3_i-MF2RJ4_bYNUeFvbiwtfr6_9VszKJaeAIuK-8iY9NqDwPik3ApHkHGNyoy5Neg0_72sLvHiGQAeJb9wdl0pE0EoFZRZxF3oLecwkd8zIB28WJAjX92wgvoiYfSgKJoAyoinh0E7jTmBPI0iTo7kWyGf_g3_G2KWlZH-nEPZFzxDgWdZTqCQP47ViiWNEUUmgDHrXUBwzXoajRxx7QbQPnupzWy8rl5asDtkAbEwjbf8sWMlaCpM7CfvFF4igQLMbbyEyKNjN6C_xvaSzt4vM2C4XQjo070mcRw3DA80kex8vrYSTfdxR9u0xrmpazTzDzFPHBptvh_Ya7_ZvoG6sAU8OFJ3skuk1oaouOjFVBQr6ibj3rVyfGsQRn5beEW2z3AQbEnrHCv-TcSx7ALeyoy8y9IYfVoGHDuPSU757fIYY8ZTNV0HGMkzmh-aSkXbtkz3jZNUhlIOpP-9RYYskfRksFHQf6HEFwAWc5GGezq-a-Xh15ZMOZY2tqiyrvYzxf44k7p0ENMC3wd4dj5fhjRd6H0FZ3EPAd8OpoGeX-l1gheUL38UmulssEih7vV8hg0he92gjBt_1iSfoS1ysc8x6aTGr6ppvmv8ofDnvW9UXHWhUF0HkqMXPQPW3OwuvHin7hSpAUO9CU_HZt8TbHfCyfrzOh3RG_ZbyCFsHABYA5Bzse9qx3p4Ects0rOExiMbknD6UNdNMGoNz89Ale5TIWMene_vvmSCzDMoD9k8X4tnTuOLUVKP3NN7EwKCZ-hfj01CGEA9DphwkhokTGj_GLzJCFE6ZeoWYvp3oNVpEWaOr01ZXw7gYu9TnjAiQjXKRLAWB3Un353f18PqTshQC2nQ0OPz7Zr3Vom4d1f6U_OrY_sJT26VmPwktaeXG5tbhapQWRUrDTz1mZ8WGWcCToVoWSa3378r0g4XC--8DjB_wglH4g5SYxt0wuDyhbOM9uQguZ90mmVW25wtmh94haWn_Jje-ysgeckKu5XhUw";
            //string ASPNET_SessionId = "hjfvn2lhzzv33m1q5cbda3zf";
            //string EMIS_Cookie = "fvvSs_l7b-Qcx1OhcCW9WozqnQKKsNk2UoeuSegHaslviyGQvwd66hxsHcNB0uda4b2tigoLG2n9O_qxUYoaZJ78RJwtnYw5B4qRTJXzXrTrzxu0mvgSd1xWqhnxKdk6g5ugGku_Ai33pkmIMGwIzcDDJl-_jpKFH98lEbSQImogwge_sOugq6DdxxpZ1MnagvUeW_hU_jxCNWaYEMnVS0T0g_VGvW6wV7v37cG24lOoRd0tzOpQYuCKzBVv8TmHwks5HtFbtDnAykEwQrjn-JWht9qiXgkugREGoM4LHRSJd9flS11ViyEev4onOyn0aC-tuOM-NknqfbrbXDnbt58LgI_yicEgKqNB0t7ZVOwmJ4bS90q2vKMcLJ3muCusSV3uBbrDZkK_lpKxeC_7CbvUqGmHQsdQ4LxnpAUPNsNVP8mA7Qo_JDQlmCGQtRMlv0Hk6ZY12f88SHNMyjmyCKpnkFNDQe3RgCkQk21cwh7BEazUcyVbxdErnOzKGFrfVQQ-YFKRPJYGTbMa4l6E4c_i9eB74A6T-dF7QP8u32NR5oe-c4pIeTdWnYVBSN0qvr-DAU4bv_po5OjJfiCA6zXPw34OjWZFRXYzERXmg_NRG-oQZgZNcM_ERKyfXHMaAkumHLI30DCZNvyG33S6qDvQxWyK5nrYdrW6iF8VkzsTMGUUZN7nSKoJ1oPwDkBJVdrkvUduQ7ie_sZtdNVAYd4KIulf8DI40HkL2ed9VSMuB8Db37i8RBhNNppDSZtQojZOJvGhcO0WlXGRHB8apODYUvBeNdSfgrppTyiKcVdIeJCvPNhcBXTLquILNZ1ewDZTSSVCqDr6XZ-CmXvRMYM7n5Oc2p1TfHpwK4tzMtxTKORlJQmriTIz8oiABmQPqH8oZlYASmEMDRAhLpr_Z_y2GqJqSeVb_7KnnBenhuy2UqfGKn1wVicyzUb6IGl2gm6kKhmz_yuApLNFiFwRMut2lk2nudA1jhvCQe4zEXduokgEpqAe4HFqlFq_j--EYkdS2KkizsXw2InWwfgbkjI3CTmr5-oFJVmlWeLDldlLcsETkWMyBfmzhTdNxmTPjoNLmiNvJJ9BaYrKN8HyfUqEH1m0tXlx3BgFDrhXTWYAHYHqeRHiOxDwTmUbsNOi3vOsnklJbYK-QhaTRnp-JW0myhBSjZ5TxlY1MfRECejzAg_aA9v5G3okjP7bIP5owyt9VZhcYqIbeyuuIvWV_DwoFns8NTpo6F6g6IZeQYMe4qmkjuITWhhUXBvMELhkuRQEQv5kPDgktyT1u0Qq4MON1r4-qauG9Rn9Z8B_ArE5c9QfoM4px_vGfLierRG2fGHjbdZkhpPosbS2hEbKDmFE34WVqDLynTjwo-sL-4OfkZTGIG2S7JNBLykvVQ8C";
            //string cf_clearance = "ba9ntEVTQ3IX7M8nGIdnF0UrH9EabBiupCRl2Rh7Byg-1763189729-1.2.1.1-mD3NgOH7dpCf4gaaRfjuth9PfdWXe0znSIe6No3tWUNO4i9Qz.CBwljwnceC_ADnX27Dd.lcjtsh0kpXOUnpFLF8y4vjwcIo2gKySqfthwiHJLdWMgaTf.ZfxbWHJlG62f4UaA1YIvdWaCv_iT5ulEUkxpogymHDFukrcPEYRWrh5I9LGJ.AMcfD3Kpy4xsqQzU1d0MHmn5sHM4yXn1rSfDAdEzZiOijS5DC9d9kF7k6rJ0MKDDTJZLx5nQcNWOH";

            //string myFullCookies = $".AspNet.Cookies={AspNetCookies}; ASP.NET_SessionId={ASPNET_SessionId}; EMIS_Cookie={EMIS_Cookie}; cf_clearance={cf_clearance};";
            //string WebApiUlr = @"https://emis.unrwa.org/api/Student/SchoolStudentsAjax?callback=jQuery21105757827720421363_1777871542383&sEcho=5&iColumns=11&sColumns=%2C%2C%2C%2C%2C%2C%2C%2C%2C%2C&iDisplayStart=0&iDisplayLength=10&mDataProp_0=StudentId&bSortable_0=true&mDataProp_1=RefugeeStatusType&bSortable_1=true&mDataProp_2=IndividualRegistrationId&bSortable_2=true&mDataProp_3=ArabicFullNameWithoutQuoted&bSortable_3=true&mDataProp_4=FamilyRegistrationId&bSortable_4=true&mDataProp_5=Gender&bSortable_5=true&mDataProp_6=School&bSortable_6=true&mDataProp_7=Grade&bSortable_7=true&mDataProp_8=Section&bSortable_8=true&mDataProp_9=ClassLeader&bSortable_9=true&mDataProp_10=10&bSortable_10=true&iSortCol_0=3&sSortDir_0=asc&iSortingCols=1&SemesterId=2&StudentName=432338119&ScholasticYearId=20252&FieldId=2&AreaId=&SchoolId=&GradeId=&SectionId=&GenderId=&StudentRegistrationStatusId=2&SNeeds=0&IsSSN=&OrginalFeildId=&RefugeeStatusTypeId=&SFileStatus=&BFileStatus=&_=1777871542389";
            //var request = new HttpRequestMessage(HttpMethod.Get, WebApiUlr);

            //// إضافة الكوكيز للطلب
            //request.Headers.Add("Cookie", myFullCookies);

            //// إضافة الـ User-Agent (اختياري لكنه مفيد ليبدو الطلب كأنه من متصفح حقيقي)
            //request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");

            //var response = await client.SendAsync(request);
            //var content = await response.Content.ReadAsStringAsync();

            //// ثم تنظيف الـ JSONP كما فعلنا سابقاً...
            //int start = content.IndexOf("(") + 1;
            //int end = content.LastIndexOf(")");
            //string cleanJson = content.Substring(start, end - start);

            //return Content(cleanJson, "application/json");
        }
    }
}
