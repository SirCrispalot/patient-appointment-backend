using System.Text.Json.Serialization;

namespace Panda.ClientModel
{
    public class Patient
    {
        public int Id { get; set; }

        [JsonPropertyName("nhs_number")]
        public string NhsNumber { get; set; }

        [JsonPropertyName("date_of_birth")]
        public DateOnly DateOfBirth { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("postcode")]
        public string Postcode { get; set; }
    }
}
