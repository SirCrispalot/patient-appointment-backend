using Panda.Model;
using Panda.Repository;
using Panda.Services.Exceptions;
using Appointment = Panda.ClientModel.Appointment;

namespace Panda.Services
{
    public class AppointmentService : IAppointmentService
    {
        private IAppointmentRepository _appointmentRepository;
        private IPatientRepository _patientRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetAppointmentById(id, cancellationToken);

            if (appointment == null)
            {
                return null;
            }

            // TODO: AppointmentMapper
            var clientAppointment = new Appointment
            {
                Id = appointment.Id,
                AppointmentDateTime = appointment.AppointmentDateTime,

                // TODO: Add clinicians and depts
                //ClinicianCode = appointment.Clinician.Code,
                //ClinicianName = appointment.Clinician.Name,
                //DepartmentCode = appointment.Department.Code,
                //DepartmentName = appointment.Department.Name,

                PatientId = appointment.Patient.Id,
                PatientNhsNumber = appointment.Patient.NhsNumber
            };

            return clientAppointment;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int id, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientId(id, cancellationToken);

            var clientAppointments = new List<Appointment>();

            // TODO: AppointmentMapper
            foreach (var appointment in appointments)
            {
                var clientAppointment = new Appointment
                {
                    Id = appointment.Id,
                    AppointmentDateTime = appointment.AppointmentDateTime,
                    ClinicianCode = appointment.Clinician.Code,
                    ClinicianName = appointment.Clinician.Name,
                    DepartmentCode = appointment.Department.Code,
                    DepartmentName = appointment.Department.Name,
                    PatientId = appointment.Patient.Id,
                    PatientNhsNumber = appointment.Patient.NhsNumber
                };

                clientAppointments.Add(clientAppointment);
            }

            return clientAppointments;
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentRepository.GetAppointmentsByPatientNhsNumber(nhsNumber, cancellationToken);

            var clientAppointments = new List<Appointment>();

            // TODO: AppointmentMapper
            foreach (var appointment in appointments)
            {
                var clientAppointment = new Appointment
                {
                    Id = appointment.Id,
                    AppointmentDateTime = appointment.AppointmentDateTime,
                    ClinicianCode = appointment.Clinician.Code,
                    ClinicianName = appointment.Clinician.Name,
                    DepartmentCode = appointment.Department.Code,
                    DepartmentName = appointment.Department.Name,
                    PatientId = appointment.Patient.Id,
                    PatientNhsNumber = appointment.Patient.NhsNumber
                };

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

            // TODO: Call clinician and dept repos and upsert them...

            var mappedAppointment = new Model.Appointment
            {
                Patient = patient,
                Clinician = null,
                Department = null,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AttendedDateTime = null,
                CancelledDateTime = null,
                Status = AppointmentStatus.Booked
            };

            var newAppointment = await _appointmentRepository.CreateAppointment(mappedAppointment, cancellationToken);

            // TODO: AppointmentMapper
            // TODO: Clinician and Dept
            var returnAppointment = new Appointment
            {
                PatientId = newAppointment.Patient.Id,
                PatientNhsNumber = newAppointment.Patient.NhsNumber,
                AppointmentDateTime = newAppointment.AppointmentDateTime,
                Id = newAppointment.Id,
                Status = newAppointment.Status.ToString()
            };

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


            // TODO: AppointmentMapper
            // TODO: Attend/cancel/booked behaviour
            var mappedAppointment = new Model.Appointment
            {
                Id = appointment.Id,
                Patient = patient,
                Clinician = null,
                Department = null,
                AppointmentDateTime = appointment.AppointmentDateTime,
                AttendedDateTime = null,
                CancelledDateTime = null,
                Status = AppointmentStatus.Booked
            };

            var updatedAppointment = await _appointmentRepository.UpdateAppointmentById(mappedAppointment, cancellationToken);

            // TODO: AppointmentMapper
            var returnAppointment = new Appointment
            {
                PatientId = updatedAppointment.Patient.Id,
                PatientNhsNumber = updatedAppointment.Patient.NhsNumber,
                AppointmentDateTime = updatedAppointment.AppointmentDateTime,
                Id = updatedAppointment.Id,
                Status = updatedAppointment.Status.ToString()
            };

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
    }
}
