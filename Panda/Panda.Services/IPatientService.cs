using Panda.ClientModel;

namespace Panda.Services;

public interface IPatientService
{
    Task<Patient?> GetPatientById(string identifier, CancellationToken cancellationToken);
}