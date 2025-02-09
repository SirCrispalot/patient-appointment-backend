using System.Text.Json.Serialization;

namespace Panda.ClientModel
{
    public class Appointment
    {
        
        public int Id { get; set; }

        public int PatientId { get; set; }

        [JsonPropertyName("patient")]
        public string PatientNhsNumber { get; set; }

        [JsonPropertyName("time")]
        public DateTime AppointmentDateTime { get; set; }

        [JsonPropertyName("department")]
        public string Department { get; set; }

        [JsonPropertyName("clinician")]
        public string Clinician { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
