using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TShop.Helpers
{
    public class Utilities
    {
        public static int PAGE_SIZE { get; set; } = 8;

        public static async Task<string> SaveImageAsync(IFormFile imageFile, string directory)
        {
            if (imageFile == null || imageFile.Length == 0)
            {
                return null;
            }

            string[] ValidImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
            if (!ValidImageExtensions.Contains(fileExtension))
            {
                return null;
            }

            var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/TShop/img", directory);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var fileName = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + Path.GetExtension(imageFile.FileName);
            var filePath = Path.Combine(directoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/TShop/img/{directory}/{fileName}";
        }

        public static string GenerateProductCode()
        {
            var random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, 10)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        public static string RemoveAccents(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return text;

            text = text.Normalize(NormalizationForm.FormD);
            char[] chars = text
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c)
                != UnicodeCategory.NonSpacingMark).ToArray();

            return new string(chars).Normalize(NormalizationForm.FormC);
        }
        public static string Slugify( string phrase)
        {
            string output = RemoveAccents(phrase).ToLower();

            output = Regex.Replace(output, @"[^A-Za-z0-9\s-]", "");

            output = Regex.Replace(output, @"\s+", " ").Trim();

            output = Regex.Replace(output, @"\s", "-");

            return output;
        }
    }
    
}
