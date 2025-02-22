using System;
using System.Collections;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common.Utilities
{
    public static class Assert
    {
        public static void NotNull<T>(T obj, string name, string message = null)
            where T : class
        {
            if (obj is null)
                throw new ArgumentNullException($"{name} : {typeof(T)}", message);
        }

        public static void NotNull<T>(T? obj, string name, string message = null)
            where T : struct
        {
            if (!obj.HasValue)
                throw new ArgumentNullException($"{name} : {typeof(T)}", message);

        }

        public static void NotEmpty<T>(T obj, string name, string message = null, T defaultValue = null)
            where T : class
        {
            if (obj == defaultValue
                || (obj is string str && string.IsNullOrWhiteSpace(str))
                || (obj is IEnumerable list && !list.Cast<object>().Any()))
            {
                throw new ArgumentException("Argument is empty : " + message, $"{name} : {typeof(T)}");
            }
        }
    }

    public static class BankHelper
    {
        public static bool CheckIBAN(string iban)
        {
            if (string.IsNullOrEmpty(iban) || iban.Length < 26 || iban.Length > 26)
                return false;

            int moduloValue = 97;
            string IBAN = (iban.Substring(4, 22) + iban.Substring(0, 4)).Replace("IR", "1827");
            var checker = Convert.ToDecimal(IBAN) % moduloValue;
            if (checker != 1)
            {
                return false;
            }

            return true;
        }
    }
    public static class NationalCodeHelper
    {
        public static bool IsValidIranianNationalCode(string nationalCode)
        {
            if (nationalCode.Length == 10)
            {
                if (nationalCode == "0000000000" ||
                    nationalCode == "1111111111" ||
                    nationalCode == "2222222222" ||
                    nationalCode == "3333333333" ||
                    nationalCode == "4444444444" ||
                    nationalCode == "5555555555" ||
                    nationalCode == "6666666666" ||
                    nationalCode == "7777777777" ||
                    nationalCode == "8888888888" ||
                    nationalCode == "9999999999")
                {
                    return false;
                }
                else
                {
                    var nationalCode9 = Int32.Parse(nationalCode[9].ToString());

                    var nationalCodeCell = (int.Parse(nationalCode[0].ToString()) * 10) +
                                           (int.Parse(nationalCode[1].ToString()) * 9) +
                                           (int.Parse(nationalCode[2].ToString()) * 8) +
                                           (int.Parse(nationalCode[3].ToString()) * 7) +
                                           (int.Parse(nationalCode[4].ToString()) * 6) +
                                           (int.Parse(nationalCode[5].ToString()) * 5) +
                                           (int.Parse(nationalCode[6].ToString()) * 4) +
                                           (int.Parse(nationalCode[7].ToString()) * 3) +
                                           (int.Parse(nationalCode[8].ToString()) * 2);

                    int nationalCodeCheck = nationalCodeCell - Convert.ToInt32(nationalCodeCell / 11) * 11;

                    if ((nationalCodeCheck == 0 && nationalCodeCheck == nationalCode9) ||
                        (nationalCodeCheck == 1 && nationalCode9 == 1) ||
                        (nationalCodeCheck > 1 && nationalCode9 == 11 - nationalCodeCheck))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }
        }

    }
    public class RegularExpression
    {
        //public const string Email = @"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"
        public const string Email = @"[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?";
        public const string Mobile = @"^09([01239])\d{8}$";
        public const string PhoneNumber = @"^0(?!00)\d{2}\d{8}$";
        public const string FacilityNumber = @"^68\d{11}$";
        public const string WebSite = @"^http(s)?://([\w-]+.)+[\w-]+(/[\w- ./?%&=])?$";
        //public const string NationalCode = @"(\S)+";
        public const string NationalCode = @"^[0-9]{10}$";
        public const string PoliceNumber = @"^[1-9]{1}[0-9]{9}$";
        public const string SaftehNumber = @"^[1-9]{1}[0-9]{10}$";
        public const string ChequeNumber = @"^[1-9]{1}[0-9]{15}$";
        public const string AccountNumber = @"^[0-9-.]{10,20}$";
        public const string ClientId = @"^\d{1,64}$";
        public const string CustomerNumber = @"^[0-9-.]{5,15}$";
        public const string AccountNumber_Length13 = @"^[0-9]{13}$";
        public const string Otp5 = @"^[0-9]{5}$";
        public const string IBAN = @"^IR[0-9]{24}$";
        public const string SSID = @"^[0-9]{1,10}$";
        public const string BirthCertificateSerial = @"^[0-9]{6}$";
        public const string PostalCode = @"^[1-9]{1}[0-9]{9}$";
        public const string Money = @"^\$?(\d{1,3},?(\d{3},?)*\d{3}(.\d{0,3})?|\d{1,3}(.\d{2})?)$";
        public const string IntegerNumber = @"(\d)+";
        public const string NumberThousandSeparator = @"^[0-9]{1,3}(,[0-9]{3})*?$";
        public const string PersianCharacter = @"^[\s,\u0600-\u06FF]+$";
        public const string EnglishCharacter = @"^[a-zA-Z]+$";
        public const string EnglishAndNumberCharacter = @"^[a-zA-Z0-9]+$";
        public const string PersianDate = @"^[1-4]\d{3}\/((0[1-6]\/((3[0-1])|([1-2][0-9])|(0[1-9])))|((1[0-2]|(0[7-9]))\/(30|([1-2][0-9])|(0[1-9]))))$";
        /// <summary>
        /// شماره فراگیر برای اتباع خارجی
        /// </summary>
        public const string PrivateNumber = @"^\d{8,12}";

        /// <summary>
        /// CardNumber RegEx,is 16 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string BankCardNumberWithDash = @"^[1-9]{1}[0-9]{3}-[0-9]{4}-[0-9]{4}-[0-9]{4}$";

        /// <summary>
        /// CardNumber RegEx,is 16 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string AyandehBankCardNumberWithDash = @"^6362-14[0-9]{2}-[0-9]{4}-[0-9]{4}$";


        /// <summary>
        /// CardNumber RegEx,is 16 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string AyandehBankCardNumber = @"^636214[0-9]{10}$";

        /// <summary>
        /// CardNumber RegEx,is 16 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string BankCardNumber = @"^[1-9]{1}[0-9]{15}$";

        /// <summary>
        /// CardNumber RegEx(Part1),is 4 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string BankCardNumber_Part1 = @"^[1-9]{1}[0-9]{3}$";

        /// <summary>
        /// CardNumber RegEx(Part1),is 4 Digit,First Number Is Not Zero(0).
        /// </summary>
        public const string BankCardNumber_Part2_To_Part4 = @"^[0-9]{4}$";
        /// <summary>
        /// Expire Year to Card
        /// </summary>
        public const string ExpireYear = @"^[0-9]{2,4}$";
        /// <summary>
        /// Expire Month to Card
        /// </summary>
        public const string ExpireMonth = @"^[0-9]{2}$";
        public const string FloatNumber = @"^\d+.?\d{0,2}$";
        public const string FloatNumber3 = @"^\d+.?\d{0,3}$";
        public const string CompanyWebsitePostFix = "^[a-zA-Z0-9-]+$";
        public const string Password = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[A-Za-z\d$@$!%*#?&]{5,}$";
    }

    public static class CodeGenerator
    {
        private static readonly Random Random = new Random();

        public static string CodeChallenge => GenerateCodeChallenge(CodeVerifier());

        public static string GenerateRandomNumber(int length)
        {
            const string chars = "0123456789";

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Random.Next(s.Length)]).ToArray());
        }

        private static string CodeVerifier()
        {
            const string chars = "abcdef0123456789";
            var nonce = new char[64];
            for (int i = 0; i < nonce.Length; i++)
                nonce[i] = chars[Random.Next(chars.Length)];

            var result = Convert.ToBase64String(Encoding.UTF8.GetBytes(new string(nonce))).Replace("/", "_").Replace("+", "-").Replace("=", "");
            return result;
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(codeVerifier));
            //var stringHash = Encoding.UTF8.GetString(hash);
            var result = Convert.ToBase64String(hash).Replace('+', '-').Replace('/', '_').Replace("=", "");
            return result;
        }

    }
}
