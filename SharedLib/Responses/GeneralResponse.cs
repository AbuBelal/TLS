using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Responses
{
    // يستخدم لإرجاع استجابة بسيطة (نجاح/فشل مع رسالة)
    // الـ Flag: القيمة true تعني نجاح العملية، و false تعني فشلها
    public record GeneralResponse(bool Success, string Message , long ReturnId=0);
}
