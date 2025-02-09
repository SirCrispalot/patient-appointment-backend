using Panda.WebApi.Validators;
using FluentAssertions;

namespace Panda.Tests.UnitTests
{
    public class NhsNumberValidatorTests
    {
        [TestCase("1234567889", false)] // Invalid checksum
        [TestCase("9876543210", true)] // Valid NHS number
        [TestCase("4010232137", true)]  // Valid NHS number
        [TestCase("2223334444", false)] // Invalid checksum
        [TestCase("123 456 7890", false)] // Invalid checksum with spaces
        [TestCase("401 023 2137", true)]  // Valid NHS number with spaces
        [TestCase("abcdefghij", false)]   // Non-numeric input
        [TestCase("", false)]             // Empty string
        [TestCase("401023213", false)]    // Less than 10 digits
        [TestCase("40102321371", false)]  // More than 10 digits
        public void IsValid_TestCases(string nhsNumber, bool expectedResult)
        {
            var nhsNumberValidator = new NhsNumberValidator();

            bool result = nhsNumberValidator.IsValid(nhsNumber);
            result.Should().Be(expectedResult);
        }
    }
}
