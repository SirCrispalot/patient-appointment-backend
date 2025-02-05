using Panda.ClientModel;
using Panda.Repository;

namespace Panda.Services
{
    public class PatientService : IPatientService
    {
        private IPandaRepository _pandaRepository;

        public PatientService(IPandaRepository pandaRepository)
        {
            _pandaRepository = pandaRepository;
        }

        public async Task<Patient?> GetPatientById(string identifier, CancellationToken cancellationToken)
        {
            var patient = await _pandaRepository.GetPatientById(identifier, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var clientPatient = new Patient { Identifier = patient.Identifier };

            return clientPatient;
        }
    }
}
