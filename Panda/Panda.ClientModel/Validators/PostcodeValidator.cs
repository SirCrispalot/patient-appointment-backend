using System.Text.RegularExpressions;

namespace Panda.ClientModel.Validators
{
    public class PostcodeValidator
    {
        public bool TryValidateAndFormatPostcode(string postcode, out string formattedPostcode)
        {
            // Remove all whitespace
            postcode = postcode.Replace(" ", "").ToUpper();

            // Validate format using regex
            Regex postcodeRegex = new Regex(@"^[A-Z]{1,2}[0-9][0-9A-Z]?[0-9][A-Z]{2}$");
            if (!postcodeRegex.IsMatch(postcode))
            {
                formattedPostcode = "";
                return false;
            }

            // Insert space before the last three characters
            formattedPostcode = postcode.Insert(postcode.Length - 3, " ");
            
            return true;
        }
    }
}
