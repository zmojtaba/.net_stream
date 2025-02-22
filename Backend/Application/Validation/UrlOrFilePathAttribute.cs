using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Backend.Application.Validation
{
    public class UrlOrFilePathAttribute : ValidationAttribute
    {
        private static readonly string UrlPattern = @"^(rtsp?):\/\/[^\s/$.?#].[^\s]*$";
        private static readonly string WindowsPathPattern = @"^[a-zA-Z]:\\(?:[^\\/:*?""<>|]+\\)*[^\\/:*?""<>|]*$";
        private static readonly string LinuxPathPattern = @"^(/([a-zA-Z0-9._\s-]+))+(\/([a-zA-Z0-9._\s-]+))?$|^~\/([a-zA-Z0-9._\s-]+(?:\/[a-zA-Z0-9._\s-]+)*)?$";



        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            string input = value.ToString().Trim();

            // Check if input matches URL format
            if (Regex.IsMatch(input, UrlPattern, RegexOptions.IgnoreCase))
            {
                return ValidationResult.Success;
            }

            // Check if input matches a valid Windows or Linux file path
            if (Regex.IsMatch(input, WindowsPathPattern) || Regex.IsMatch(input, LinuxPathPattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The input must be a valid Rtsp URL or a valid file path (Windows/Linux).");
        }
    }
}