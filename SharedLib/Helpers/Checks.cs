using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Helpers
{
    public static class Checks
    {
        public static bool CheckLuhnE9(long number)
        {
            // تحويل الرقم إلى نص مكون من 9 خانات مع إضافة أصفار على اليسار إذا كان أقل من ذلك
            string text = number.ToString("D9");

            // المصفوفة السحرية التي ضربت بها في إكسل {1,2,1,2,1,2,1,2,1}
            int[] multipliers = { 1, 2, 1, 2, 1, 2, 1, 2, 1 };
            int sum = 0;

            for (int i = 0; i < 9; i++)
            {
                // تحويل الحرف (الخانة) إلى رقم
                int digit = text[i] - '0';

                // ضرب الخانة في المعامل المقابل لها
                int product = digit * multipliers[i];

                // في إكسل قمت بقسمة الناتج على 10 وأخذ الجزء الصحيح + باقي القسمة على 10
                // في البرمجة، هذا يعادل جمع خانات الرقم الناتج (مثلاً لو الناتج 14، يصبح 1 + 4 = 5)
                sum += (product / 10) + (product % 10);
            }

            // التحقق مما إذا كان المجموع يقبل القسمة على 10 بدون باقٍ
            return sum % 10 == 0;
        }

        public static bool CheckLuhnE9(string number)
        {
            long IdNo = 0;
            long.TryParse(number, out IdNo);

            if(IdNo == 0)
                return false;
            else
            {
                return CheckLuhnE9(IdNo);
            }
        }
     }
}
