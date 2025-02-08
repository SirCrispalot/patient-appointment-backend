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
                ClinicianCode = "FOS",
                ClinicianName = "Dr Foster",
                DepartmentCode = "NEPH",
                DepartmentName = "Nephrology"                
            };
        }

        public Appointment CreateClientAppointmentB()
        {
            return new Appointment
            {
                PatientId = 1,
                PatientNhsNumber = "8888888888",
                AppointmentDateTime = new DateTime(2025, 10, 10),
                ClinicianCode = "DOO",
                ClinicianName = "Dr Dolittle",
                DepartmentCode = "DERM",
                DepartmentName = "Dermatology"
            };
        }
    }
}
