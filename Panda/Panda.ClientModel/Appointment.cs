namespace Panda.ClientModel
{
    public class Appointment
    {
        public int Id { get; set; }

        public int PatientId { get; set; }

        public string PatientNhsNumber { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public string DepartmentCode { get; set; }

        public string ClinicianCode { get; set; }
    }
}
