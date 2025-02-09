using Panda.Model;

namespace Panda.Repository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientById(int id, CancellationToken cancellationToken);

        Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);

        Task<Patient> CreatePatient(Patient patient, CancellationToken cancellationToken);

        Task<Patient> UpdatePatientByNhsNumber(Patient patient, CancellationToken cancellationToken);

        Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);
    }
}
