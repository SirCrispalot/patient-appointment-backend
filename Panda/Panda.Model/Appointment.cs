namespace Panda.Model
{
    public class Appointment
    {
        public Patient Patient { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public Department Department { get; set; }

        public Clinician Clinician { get; set; }
    }
}
