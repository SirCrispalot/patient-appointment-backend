using Panda.ClientModel;

namespace Panda.Services
{
    public interface IAppointmentService
    {
        Task<ClientModel.Appointment?> GetAppointmentById(int id, CancellationToken cancellationToken);

        Task<IEnumerable<ClientModel.Appointment>> GetAppointmentsByPatientId(int id, CancellationToken cancellationToken);

        Task<IEnumerable<ClientModel.Appointment>> GetAppointmentsByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken);

        Task<ClientModel.Appointment> CreateAppointment(ClientModel.Appointment appointment, CancellationToken cancellationToken);

        Task<ClientModel.Appointment> UpdateAppointment(Appointment appointment, CancellationToken cancellationToken);

        Task<bool> CancelAppointmentById(int id, CancellationToken cancellationToken);

        Task<bool> AttendAppointmentById(int id, CancellationToken cancellationToken);
    }
}