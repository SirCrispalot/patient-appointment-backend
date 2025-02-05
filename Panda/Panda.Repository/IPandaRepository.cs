using Panda.Model;

namespace Panda.Repository
{
    public interface IPandaRepository
    {
        Task<Patient?> GetPatientById(string identifier, CancellationToken cancellationToken);
    }
}
