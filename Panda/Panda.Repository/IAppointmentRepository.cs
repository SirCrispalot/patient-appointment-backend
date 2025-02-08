using Panda.Model;

namespace Panda.Repository
{
    public interface IAppointmentRepository
    {
        Task<Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken);

        Task<Appointment?> GetAppointmentByPatientNhsNumberAndDateTime(string nhsNumber, DateTime appointmentDateTime, CancellationToken cancellationToken);

        Task<IEnumerable<Appointment>> GetAppointmentsByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken);

        Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(int id, CancellationToken cancellationToken);

        Task<Appointment> CreateAppointment(Appointment appointment, CancellationToken cancellationToken);

        Task<Appointment> UpdateAppointmentById(Appointment appointment, CancellationToken cancellationToken);

        Task<bool> CancelAppointmentById(int id, CancellationToken cancellationToken);

        Task<bool> AttendAppointmentById(int id, CancellationToken cancellationToken);
    }
}
