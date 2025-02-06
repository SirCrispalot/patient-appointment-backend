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

        public async Task<Patient?> GetPatientById(int id, CancellationToken cancellationToken)
        {
            var patient = await _pandaRepository.GetPatientById(id, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var clientPatient = new Patient { Id = patient.Id };

            return clientPatient;
        }

        public async Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var patient = await _pandaRepository.GetPatientByNhsNumber(nhsNumber, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var clientPatient = new Patient { Id = patient.Id, NhsNumber = nhsNumber};

            return clientPatient;
        }

        public async Task<Patient> CreatePatient(Patient patient, CancellationToken cancellationToken)
        {
            var newPatient = await _pandaRepository.CreatePatient(
                patient.NhsNumber,
                patient.DateOfBirth,
                (Model.SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                (Model.GenderIdentity)(int)patient.GenderIdentity,
                patient.Surname, 
                patient.Forename, 
                patient.MiddleNames, 
                patient.Title, 
                cancellationToken);

            var mappedPatient = new Patient
            {
                Id = newPatient.Id,
                NhsNumber = newPatient.NhsNumber,
                DateOfBirth = newPatient.DateOfBirth,
                SexAssignedAtBirth = (SexAssignedAtBirth)(int)newPatient.SexAssignedAtBirth,
                GenderIdentity = (GenderIdentity)(int)newPatient.GenderIdentity,
                Surname = newPatient.Surname,
                Forename = newPatient.Forename,
                MiddleNames = newPatient.MiddleNames,
                Title = newPatient.Title
            };

            return mappedPatient;
        }
    }
}
