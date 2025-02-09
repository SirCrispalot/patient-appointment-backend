using Panda.ClientModel;
using Panda.Model;
using Panda.Repository;
using Panda.Services.Exceptions;
using Panda.Services.Mappers;
using Appointment = Panda.ClientModel.Appointment;

namespace Panda.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _appointmentRepository;
        private IPatientRepository _patientRepository;
        private AppointmentMapper _appointmentMapper;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, AppointmentMapper appointmentMapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _appointmentMapper = appointmentMapper;
        }

        public async Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id, cancellationToken);

            if (appointment == null)
            {
                return null;
            }

            var clientAppointment = _appointmentMapper.MapToClientAppointment(appointment);

            return clientAppointment;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int id, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientId(id, cancellationToken);

            var clientAppointments = new List<Appointment>();

            foreach (var appointment in appointments)
            {
                var clientAppointment = _appointmentMapper.MapToClientAppointment(appointment);

                clientAppointments.Add(clientAppointment);
            }

            return clientAppointments;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientNhsNumber(nhsNumber, cancellationToken);

            var clientAppointments = new List<Appointment>();

            foreach (var appointment in appointments)
            {
                var clientAppointment = _appointmentMapper.MapToClientAppointment(appointment);

                clientAppointments.Add(clientAppointment);
            }

            return clientAppointments;
        }

        public async Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken)
        {
            var existingAppointment =
                await _appointmentRepository.GetAppointmentByPatientNhsNumberAndDateTime(appointment.PatientNhsNumber,
                    appointment.AppointmentDateTime, cancellationToken);

            if (existingAppointment != null)
            {
                throw new AppointmentAlreadyExistsException(appointment.PatientNhsNumber,
                    appointment.AppointmentDateTime);
            }

            var patient = await _patientRepository.GetPatientByNhsNumber(appointment.PatientNhsNumber, cancellationToken);
            if (patient == null)
            {
                throw new PatientDoesNotExistException(appointment.PatientNhsNumber);
            }

            var mappedAppointment = new Model.Appointment
            {
                Patient = patient,
                ClinicianCode = appointment.ClinicianCode,
                DepartmentCode = appointment.DepartmentCode,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AttendedDateTime = null,
                CancelledDateTime = null,
                Status = AppointmentStatus.Booked
            };

            var newAppointment = await _appointmentRepository.CreateAppointment(mappedAppointment, cancellationToken);

            var returnAppointment = _appointmentMapper.MapToClientAppointment(newAppointment);

            return returnAppointment;
        }

        public async Task<Appointment> UpdateAppointment(Appointment appointment, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByNhsNumber(appointment.PatientNhsNumber, cancellationToken);
            if (patient == null)
            {
                throw new PatientDoesNotExistException(appointment.PatientNhsNumber);
            }

            var existingAppointment = await _appointmentRepository.GetAppointmentById(appointment.Id, cancellationToken);

            if (existingAppointment.Status == AppointmentStatus.Attended || existingAppointment.Status == AppointmentStatus.Cancelled)
            {
                throw new AppointmentStatusException(appointment.Id, existingAppointment.Status);
            }

            var mappedAppointment = _appointmentMapper.MapFromClientAppointment(appointment, patient);
            
            var updatedAppointment = await _appointmentRepository.UpdateAppointmentById(mappedAppointment, cancellationToken);

            var returnAppointment = _appointmentMapper.MapToClientAppointment(updatedAppointment);

            return returnAppointment;
        }

        public async Task<bool> CancelAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id, cancellationToken);
            if (appointment == null)
            {
                throw new AppointmentDoesNotExistException(id);
            }
            if (appointment.Status != AppointmentStatus.Booked)
            {
                throw new AppointmentStatusException(id, appointment.Status);
            }

            return await _appointmentRepository.CancelAppointmentById(id, cancellationToken);
        }

        public async Task<bool> AttendAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id, cancellationToken);
            if (appointment == null)
            {
                throw new AppointmentDoesNotExistException(id);
            }
            if (appointment.Status != AppointmentStatus.Booked)
            {
                throw new AppointmentStatusException(id, appointment.Status);
            }

            return await _appointmentRepository.AttendAppointmentById(id, cancellationToken);
        }

        public async Task<MissedAppointmentReportResponse> GetMissedAppointments(MissedAppointmentReportRequest request, CancellationToken cancellationToken)
        {
            var missedAppointments = await _appointmentRepository.GetMissedAppointments(request.ReportFrom, request.ReportTo,
                request.DepartmentCode, request.ClinicianCode, cancellationToken);

            var response = new MissedAppointmentReportResponse();

            response.ClinicianCode = request.ClinicianCode;
            response.DepartmentCode = request.DepartmentCode;
            response.ReportFrom = request.ReportFrom;
            response.ReportTo = request.ReportTo;
            response.Appointments = new List<Appointment>();

            foreach (var appointment in missedAppointments)
            {
                var clientAppointment = _appointmentMapper.MapToClientAppointment(appointment);

                response.Appointments.Add(clientAppointment);
            }

            return response;
        }
    }
}
