namespace Panda.Model
{
    public class Appointment
    {
        public int Id { get; set; }

        public Patient Patient { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public Department Department { get; set; }

        public Clinician Clinician { get; set; }

        public DateTime? CancelledDateTime { get; set; }

        public DateTime? AttendedDateTime { get; set; }

        public AppointmentStatus Status { get; set; }
    }
}
