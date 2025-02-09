using Panda.Model;

namespace Panda.Services.Mappers
{
    public class AppointmentMapper
    {
        public Model.Appointment MapFromClientAppointment(ClientModel.Appointment clientAppointment, Patient patient)
        {
            var mappedAppointment = new Model.Appointment
            {
                Id = clientAppointment.Id,
                Patient = patient,
                ClinicianCode = clientAppointment.ClinicianCode,
                DepartmentCode = clientAppointment.DepartmentCode,
                AppointmentDateTime = clientAppointment.AppointmentDateTime,
                AttendedDateTime = null,
                CancelledDateTime = null,
                Status = AppointmentStatus.Booked
            };

            return mappedAppointment;
        }

        public ClientModel.Appointment MapToClientAppointment(Model.Appointment appointment)
        {
            var clientAppointment = new ClientModel.Appointment
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,
                ClinicianCode = appointment.ClinicianCode,
                DepartmentCode = appointment.DepartmentCode,
                PatientId = appointment.Patient.Id,
                PatientNhsNumber = appointment.Patient.NhsNumber,
                Status = appointment.Status.ToString()
            };

            return clientAppointment;
        }
    }
}
