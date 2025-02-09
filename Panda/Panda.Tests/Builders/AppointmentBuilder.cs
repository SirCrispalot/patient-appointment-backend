using Panda.Model;

namespace Panda.Tests.Builders
{
    public class AppointmentBuilder
    {
        public Appointment CreateAppointmentA(Patient patient)
        {
            return new Appointment
            {
                Patient = patient,
                AppointmentDateTime = new DateTime(2025, 6, 15, 10, 30, 0),
                Clinician = "Dr Foster",
                Department = "Nephrology",
                Status = AppointmentStatus.Active,
                AttendedDateTime = null,
                CancelledDateTime = null
            };
        }

        public Appointment CreateAppointmentB(Patient patient)
        {
            return new Appointment
            {
                Patient = patient,
                AppointmentDateTime = new DateTime(2025, 10, 10, 15, 0, 0),
                Clinician = "Dr Strange",
                Department = "Dermatology",
                Status = AppointmentStatus.Active,
                AttendedDateTime = null,
                CancelledDateTime = null
            };
        }
    }
}
