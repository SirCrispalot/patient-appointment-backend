using Microsoft.EntityFrameworkCore;
using Panda.Model;

namespace Panda.Repository.EntityFramework
{
    public class PandaRepository : IPandaRepository
    {
        private readonly PandaDbContext _pandaDbContext;

        public PandaRepository(PandaDbContext pandaDbContext)
        {
            _pandaDbContext = pandaDbContext;
        }

        public async Task<Patient?> GetPatientById(string identifier, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.Identifier == identifier,
                cancellationToken);
        }
    }
}
