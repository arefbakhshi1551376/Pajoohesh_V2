using System;

namespace Pajoohesh_V2.Utility
{
    public static class Builder
    {
        public static string BuildNameForImages(int length = 10)
        {
            string finalString = "Image";
            DateTime dateTime = DateTime.UtcNow;
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                var currentCharIndex = random.Next(allChars.Length);
                var currentChar = allChars[currentCharIndex];
                finalString += currentChar;
            }

            var stringDate = dateTime.ToPersianDateForNaming();
            finalString += stringDate;

            return finalString;
        }

        public static string BuildNameForVideos(int length = 5)
        {
            string finalString = "Video";
            DateTime dateTime = DateTime.UtcNow;
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                var currentCharIndex = random.Next(allChars.Length);
                var currentChar = allChars[currentCharIndex];
                finalString += currentChar;
            }

            var stringDate = dateTime.ToPersianDateForNaming();
            finalString += stringDate;

            return finalString;
        }

        public static string BuildKeyForForgetPassword(int length = 30)
        {
            string finalString = "ForgetPassword";
            DateTime dateTime = DateTime.UtcNow;
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                var currentCharIndex = random.Next(allChars.Length);
                var currentChar = allChars[currentCharIndex];
                finalString += currentChar;
            }

            var stringDate = dateTime.ToPersianDateForNaming();
            finalString += stringDate;

            return finalString;
        }

        public static string BuildKeyForNewsLetter(int length = 30)
        {
            string finalString = "NewsLetter";
            DateTime dateTime = DateTime.UtcNow;
            string allChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                var currentCharIndex = random.Next(allChars.Length);
                var currentChar = allChars[currentCharIndex];
                finalString += currentChar;
            }

            var stringDate = dateTime.ToPersianDateForNaming();
            finalString += stringDate;

            return finalString;
        }
    }
}
