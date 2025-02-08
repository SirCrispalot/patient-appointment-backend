namespace Panda.ClientModel
{
    public class MissedAppointmentReportResponse
    {
        public DateTime ReportFrom { get; set; }

        public DateTime ReportTo { get; set; }

        public string ClinicianCode { get; set; }

        public string DepartmentCode { get; set; }

        public List<Appointment> Appointments { get; set; }
    }
}
