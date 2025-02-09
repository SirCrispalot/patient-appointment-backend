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
                Clinician = clientAppointment.Clinician,
                Department = clientAppointment.Department,
                AppointmentDateTime = clientAppointment.AppointmentDateTime,
                AttendedDateTime = null,
                CancelledDateTime = null,
                Status = AppointmentStatus.Active
            };

            return mappedAppointment;
        }

        public ClientModel.Appointment MapToClientAppointment(Model.Appointment appointment)
        {
            var clientAppointment = new ClientModel.Appointment
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,
                Clinician = appointment.Clinician,
                Department = appointment.Department,
                PatientId = appointment.Patient.Id,
                PatientNhsNumber = appointment.Patient.NhsNumber,
                Status = appointment.Status.ToString()
            };

            return clientAppointment;
        }
    }
}
