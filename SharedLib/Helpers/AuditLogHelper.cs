using System;
using System.Reflection;
using System.Text;

public static class AuditLogHelper
{
    //public static string CompareObjects<T>(T oldObject, T newObject) where T : class
    //{
    //    // التحقق من أن الكائنات ليست فارغة
    //    if (oldObject == null || newObject == null)
    //        return "أحد الكائنات أو كلاهما فارغ، لا يمكن إجراء المقارنة.";

    //    var logBuilder = new StringBuilder();
    //    PropertyInfo? nameProperty = typeof(T).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);

    //    if (nameProperty != null)
    //    {
    //        object? nameValue = nameProperty.GetValue(newObject);
    //        logBuilder.AppendLine($"=تقرير التعديلات : {nameValue ?? "بدون اسم"}=");
    //    }

    //    // الحصول على جميع الخصائص العامة (Public Properties) للكائن
    //    PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

    //    foreach (PropertyInfo property in properties)
    //    {
    //        // جلب قيمة الخاصية من الكائن القديم والجديد
    //        object? oldValue = property.GetValue(oldObject);
    //        object? newValue = property.GetValue(newObject);

    //        // --- التعديل الجديد: استخراج اسم المركز إذا كانت الخاصية تمثل مركزاً ---
    //        object? displayOldValue = GetCenterNameIfApplicable(oldValue);
    //        object? displayNewValue = GetCenterNameIfApplicable(newValue);

    //        // مقارنة القيم بناءً على قيم العرض المستخرجة (مع مراعاة القيم الفارغة Null)
    //        if (!Equals(displayOldValue, displayNewValue))
    //        {
    //            // إذا اختلفت القيم، نقوم بإضافتها لنص التقرير
    //            logBuilder.AppendLine($"[تعديل في {property.Name}]: ");
    //            logBuilder.AppendLine($"   - القيمة القديمة: '{displayOldValue ?? "فارغ"}'");
    //            logBuilder.AppendLine($"   - القيمة الجديدة: '{displayNewValue ?? "فارغ"}'");
    //            logBuilder.AppendLine(new string('-', 30));
    //        }
    //    }

    //    // إذا لم يتم العثور على أي اختلاف
    //    if (logBuilder.Length == 0)
    //        return "لم يتم تعديل أي خصائص.";

    //    return logBuilder.ToString();
    //}

    //// دالة مساعدة ذكية لاستخراج Name من السلسلة (Object -> Center -> Name)
    //private static object? GetCenterNameIfApplicable(object? obj)
    //{
    //    if (obj == null) return null;

    //    var objType = obj.GetType();

    //    // 1. التحقق مما إذا كان الكائن عبارة عن قائمة (List / Collection) وليس نصاً عاديًا
    //    if (obj is System.Collections.IEnumerable enumerable && objType != typeof(string))
    //    {
    //        var names = new List<string>();

    //        foreach (var item in enumerable)
    //        {
    //            if (item == null) continue;

    //            // نبحث عن خاصية Center داخل العنصر الحالي
    //            PropertyInfo? centerProp = item.GetType().GetProperty("Center", BindingFlags.Public | BindingFlags.Instance);
    //            if (centerProp != null)
    //            {
    //                object? centerValue = centerProp.GetValue(item);
    //                if (centerValue != null)
    //                {
    //                    PropertyInfo? nameProp = centerValue.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
    //                    if (nameProp != null)
    //                    {
    //                        var nameVal = nameProp.GetValue(centerValue)?.ToString();
    //                        if (!string.IsNullOrEmpty(nameVal))
    //                        {
    //                            names.Add(nameVal);
    //                        }
    //                    }
    //                }
    //            }
    //        }

    //        // إذا عثرنا على أسماء داخل القائمة، ندمجها ونفصل بينها بفاصلة
    //        if (names.Any())
    //        {
    //            return $"[{string.Join(", ", names)}]";
    //        }
    //    }

    //    // 2. إذا كان كائناً فردياً (وليس قائمة) نقوم بنفس العمل السابق
    //    PropertyInfo? singleCenterProp = objType.GetProperty("Center", BindingFlags.Public | BindingFlags.Instance);
    //    if (singleCenterProp != null)
    //    {
    //        object? centerValue = singleCenterProp.GetValue(obj);
    //        if (centerValue != null)
    //        {
    //            PropertyInfo? nameProp = centerValue.GetType().GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);
    //            if (nameProp != null)
    //            {
    //                return nameProp.GetValue(centerValue);
    //            }
    //        }
    //    }

    //    // إذا لم يكن قائمة ولا يحتوي على الخصائص المطلوبة، نرجعه كما هو (ليقوم بالتحويل النصي الافتراضي)
    //    return obj;
    //}


    public static string CompareObjects<T>(T oldObject, T newObject) where T : class
    {
        // التحقق من أن الكائنات ليست فارغة
        if (oldObject == null || newObject == null)
            return "أحد الكائنات أو كلاهما فارغ، لا يمكن إجراء المقارنة.";

        var logBuilder = new StringBuilder();
        PropertyInfo? nameProperty = typeof(T).GetProperty("Name", BindingFlags.Public | BindingFlags.Instance);

        if (nameProperty != null)
        {
            // جلب القيمة (مثلاً من الكائن الجديد لتبين الاسم الحالي)
            object? nameValue = nameProperty.GetValue(newObject);

            // إضافتها في بداية الـ logBuilder
            logBuilder.AppendLine($"=تقرير التعديلات : {nameValue ?? "بدون اسم"}=");
            //logBuilder.AppendLine(new string('=', 5));
        }
        // الحصول على جميع الخصائص العامة (Public Properties) للكائن
        PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (PropertyInfo property in properties)
        {
            // جلب قيمة الخاصية من الكائن القديم والجديد
            object? oldValue = property.GetValue(oldObject);
            object? newValue = property.GetValue(newObject);

            // مقارنة القيم (مع مراعاة القيم الفارغة Null)
            if (!Equals(oldValue, newValue))
            {
                // إذا اختلفت القيم، نقوم بإضافتها لنص التقرير
                logBuilder.AppendLine($"[تعديل في {property.Name}]: ");
                logBuilder.AppendLine($"   - القيمة القديمة: '{oldValue ?? "فارغ"}'");
                logBuilder.AppendLine($"   - القيمة الجديدة: '{newValue ?? "فارغ"}'");
                logBuilder.AppendLine(new string('-', 30));
            }
        }

        // إذا لم يتم العثور على أي اختلاف
        if (logBuilder.Length == 0)
            return "لم يتم تعديل أي خصائص.";

        return logBuilder.ToString();
    }
}