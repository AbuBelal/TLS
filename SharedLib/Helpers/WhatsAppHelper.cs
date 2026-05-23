using System;
using System.Collections.Generic;
using System.Text;

namespace SharedLib.Helpers
{
    public static class WhatsAppHelper
    {
        public static string GetWhatsAppLink(string phoneNumber, string? message=null , string countryCode = "970")
        {
            if (string.IsNullOrEmpty(phoneNumber)) return "";
            // Remove any non-digit characters from the phone number
            var cleanedPhoneNumber = new StringBuilder();
            foreach (var ch in phoneNumber)
            {
                if (char.IsDigit(ch))
                {
                    cleanedPhoneNumber.Append(ch);
                }
            }
            if (cleanedPhoneNumber[0] == '0') {
                cleanedPhoneNumber.Remove(0, 1);
            }
            cleanedPhoneNumber.Insert(0, countryCode); // Assuming the country code is +970 for Palestine
            // Encode the message for URL
            var encodedMessage = message is not null ? Uri.EscapeDataString(message) : string.Empty;
            // Construct the WhatsApp link
            return $"https://wa.me/{cleanedPhoneNumber}?text={encodedMessage}";
        }
    }
}
