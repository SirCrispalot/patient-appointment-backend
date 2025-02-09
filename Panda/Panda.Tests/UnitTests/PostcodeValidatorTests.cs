using Panda.ClientModel.Validators;
using FluentAssertions;

namespace Panda.Tests.UnitTests
{
    public class PostcodeValidatorTests
    {
        [TestCase("SW1A1AA", "SW1A 1AA")]   // Standard format
        [TestCase("sw1a1aa", "SW1A 1AA")]   // Lowercase input
        [TestCase(" SW1A 1AA ", "SW1A 1AA")] // Input with extra spaces
        [TestCase("EC1A1BB", "EC1A 1BB")]   // Valid format without space
        [TestCase("W1A0AX", "W1A 0AX")]     // Single-character area code
        [TestCase("M11AE", "M1 1AE")]       // Short outward code
        [TestCase("CR26XH", "CR2 6XH")]     // Two-character outward code
        [TestCase("DN551PT", "DN55 1PT")]   // Longer outward code
        public void TryValidateAndFormatPostcode_ValidInputs_ReturnsTrueAndFormattedPostcode(string input, string expectedOutput)
        {
            var postcodeFormatter = new PostcodeValidator();
            bool result = postcodeFormatter.TryValidateAndFormatPostcode(input, out var formattedPostcode);
            result.Should().BeTrue();
            formattedPostcode.Should().Be(expectedOutput);
        }

        [TestCase("INVALID")]       // Completely invalid
        [TestCase("123456")]        // Numeric-only
        [TestCase("ABCDE")]         // Alpha-only
        [TestCase("SW1A 1A")]       // Too short
        [TestCase("SW1A1AAA")]      // Too long
        [TestCase("")]              // Empty string
        public void TryValidateAndFormatPostcode_InvalidInputs_ReturnsFalse(string input)
        {
            var postcodeFormatter = new PostcodeValidator();
            bool result = postcodeFormatter.TryValidateAndFormatPostcode(input, out var formattedPostcode);
            result.Should().BeFalse();
        }
    }
}
