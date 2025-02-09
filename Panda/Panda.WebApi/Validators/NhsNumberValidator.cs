namespace Panda.WebApi.Validators
{
    public class NhsNumberValidator
    {
        public bool IsValid(string nhsNumber)
        {
            // Remove any whitespace
            nhsNumber = nhsNumber.Replace(" ", "");

            // Check length and ensure all characters are digits
            if (nhsNumber.Length != 10 || !nhsNumber.All(char.IsDigit))
            {
                return false;
            }

            // Extract digits and calculate checksum
            int[] weights = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] digits = nhsNumber.Take(9).Select(c => c - '0').ToArray();
            int checksumDigit = nhsNumber[9] - '0';

            int weightedSum = digits.Zip(weights, (digit, weight) => digit * weight).Sum();
            int remainder = weightedSum % 11;
            int expectedChecksum = (11 - remainder) % 11;

            // Validate checksum
            return expectedChecksum == checksumDigit;
        }
    }
}
