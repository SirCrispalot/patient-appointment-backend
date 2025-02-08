using Panda.ClientModel;

namespace Panda.Services
{
    public interface IPatientService
    {
        Task<ClientModel.Patient?> GetPatientById(int id, CancellationToken cancellationToken);

        Task<ClientModel.Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);

        Task<ClientModel.Patient> CreatePatient(ClientModel.Patient patient, CancellationToken cancellationToken);

        Task<ClientModel.Patient> UpdatePatient(Patient patient, CancellationToken cancellationToken);

        Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);
    }
}