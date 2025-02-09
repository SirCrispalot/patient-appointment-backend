using Microsoft.EntityFrameworkCore;
using Panda.Model;

namespace Panda.Repository.EntityFramework
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly PandaDbContext _pandaDbContext;

        public AppointmentRepository(PandaDbContext pandaDbContext)
        {
            _pandaDbContext = pandaDbContext;
        }

        public async Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Appointments.SingleOrDefaultAsync(appointment => appointment.Id == id,
                cancellationToken);
        }

        // TODO: Assumption made that patient can only have one appointment at a given time. Check rules with business.
        public async Task<Appointment?> GetAppointmentByPatientNhsNumberAndDateTime(string nhsNumber,
            DateTime appointmentDateTime,
            CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Appointments.SingleOrDefaultAsync(
                appointment => appointment.Patient.NhsNumber == nhsNumber &&
                               appointment.AppointmentDateTime == appointmentDateTime,
                cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Appointments.Where(appointment => appointment.Patient.NhsNumber == nhsNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int id, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Appointments.Where(appointment => appointment.Patient.Id == id)
                .ToListAsync(cancellationToken);
        }

        public async Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken)
        {
            _pandaDbContext.Appointments.Add(appointment);

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var newAppointment = await GetAppointmentByPatientNhsNumberAndDateTime(appointment.Patient.NhsNumber,
                appointment.AppointmentDateTime, cancellationToken);

            return newAppointment;
        }

        public async Task<Appointment> UpdateAppointmentById(Appointment appointment, CancellationToken cancellationToken)
        {
            var existingAppointment = await GetAppointmentById(appointment.Id, cancellationToken);

            existingAppointment.AppointmentDateTime = appointment.AppointmentDateTime;

            _pandaDbContext.Entry(existingAppointment).CurrentValues.SetValues(appointment);

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var updatedPatient = await GetAppointmentById(existingAppointment.Id, cancellationToken);

            return updatedPatient;
        }

        public async Task<bool> CancelAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointmentToCancel = await _pandaDbContext.Appointments.SingleOrDefaultAsync(appointment => appointment.Id == id,
                cancellationToken);

            if (appointmentToCancel == null)
            {
                return false;
            }

            appointmentToCancel.CancelledDateTime = DateTime.UtcNow;
            appointmentToCancel.Status = AppointmentStatus.Cancelled;

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<bool> AttendAppointmentById(int id, CancellationToken cancellationToken)
        {
            var appointmentToAttend = await _pandaDbContext.Appointments.SingleOrDefaultAsync(appointment => appointment.Id == id,
                cancellationToken);

            if (appointmentToAttend == null)
            {
                return false;
            }

            appointmentToAttend.AttendedDateTime = DateTime.UtcNow;
            appointmentToAttend.Status = AppointmentStatus.Attended;

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IEnumerable<Appointment>> GetMissedAppointments(DateTime fromDateTime, DateTime toDateTime, string departmentCode, string clinicianCode,
            CancellationToken cancellationToken)
        {
            var missedAppointments = await _pandaDbContext
                .Appointments
                .Where(a =>
                    a.AppointmentDateTime >= fromDateTime &&
                    a.AppointmentDateTime <= toDateTime &&
                    a.Status == AppointmentStatus.Active &&
                    (string.IsNullOrWhiteSpace(departmentCode) || a.Department == departmentCode) &&
                    (string.IsNullOrWhiteSpace(clinicianCode) || a.Clinician == clinicianCode))
                .ToListAsync(cancellationToken);

            return missedAppointments;
        }
    }
}
