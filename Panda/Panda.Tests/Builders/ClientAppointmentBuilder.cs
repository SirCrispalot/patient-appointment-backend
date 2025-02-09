using Panda.ClientModel;

namespace Panda.Tests.Builders
{
    public class ClientAppointmentBuilder
    {
        public Appointment CreateClientAppointmentA()
        {
            return new Appointment
            {
                PatientId = 1,
                PatientNhsNumber = "8888888888",
                AppointmentDateTime = new DateTime(2025, 6, 15, 10, 30, 0),
                Clinician = "Dr Foster",
                Department = "Nephrology",
            };
        }

        public Appointment CreateClientAppointmentB()
        {
            return new Appointment
            {
                PatientId = 1,
                PatientNhsNumber = "8888888888",
                AppointmentDateTime = new DateTime(2025, 10, 10, 15, 0, 0),
                Clinician = "Dr Strange",
                Department = "Dermatology",
            };
        }
    }
}
