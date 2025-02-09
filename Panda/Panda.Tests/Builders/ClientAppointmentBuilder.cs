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
                AppointmentDateTime = new DateTime(2025, 6, 15),
                Clinician = "FOS",
                Department = "NEPH",
            };
        }

        public Appointment CreateClientAppointmentB()
        {
            return new Appointment
            {
                PatientId = 1,
                PatientNhsNumber = "8888888888",
                AppointmentDateTime = new DateTime(2025, 10, 10),
                Clinician = "DOO",
                Department = "DERM",
            };
        }
    }
}
