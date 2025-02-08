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
                AppointmentDateTime = new DateTime(2025, 6, 15),
                //Clinician = "FOS",
                //ClinicianName = "Dr Foster",
                //DepartmentCode = "NEPH",
                //DepartmentName = "Nephrology"
                Status = AppointmentStatus.Booked,
                AttendedDateTime = null,
                CancelledDateTime = null
            };
        }

        public Appointment CreateAppointmentB(Patient patient)
        {
            return new Appointment
            {
                Patient = patient,
                AppointmentDateTime = new DateTime(2025, 10, 10),
                //Clinician = "FOS",
                //ClinicianName = "Dr Foster",
                //DepartmentCode = "NEPH",
                //DepartmentName = "Nephrology"
                Status = AppointmentStatus.Booked,
                AttendedDateTime = null,
                CancelledDateTime = null
            };
        }
    }
}
