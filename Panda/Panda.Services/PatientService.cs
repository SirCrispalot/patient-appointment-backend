using Panda.ClientModel;
using Panda.Repository;
using Panda.Services.Exceptions;

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

            // TODO: PatientMapper
            var clientPatient = new Patient
            {
                Id = patient.Id,
                NhsNumber = patient.NhsNumber,
                DateOfBirth = patient.DateOfBirth,
                SexAssignedAtBirth = (SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                GenderIdentity = (GenderIdentity)(int)patient.GenderIdentity,
                Surname = patient.Surname,
                Forename = patient.Forename,
                MiddleNames = patient.MiddleNames,
                Title = patient.Title
            };

            return clientPatient;
        }

        public async Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var patient = await _pandaRepository.GetPatientByNhsNumber(nhsNumber, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            // TODO: PatientMapper
            var clientPatient = new Patient
            {
                Id = patient.Id,
                NhsNumber = patient.NhsNumber,
                DateOfBirth = patient.DateOfBirth,
                SexAssignedAtBirth = (SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                GenderIdentity = (GenderIdentity)(int)patient.GenderIdentity,
                Surname = patient.Surname,
                Forename = patient.Forename,
                MiddleNames = patient.MiddleNames,
                Title = patient.Title
            };

            return clientPatient;
        }

        public async Task<Patient> CreatePatient(Patient patient, CancellationToken cancellationToken)
        {
            var existingPatient = await _pandaRepository.GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);

            if (existingPatient != null)
            {
                throw new PatientAlreadyExistsException(patient.NhsNumber);
            }

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

            // TODO: PatientMapper
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

        public async Task<Patient> UpdatePatient(Patient patient, CancellationToken cancellationToken)
        {
            var updatedPatient = await _pandaRepository.UpdatePatientByNhsNumber(
                patient.NhsNumber,
                patient.DateOfBirth,
                (Model.SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                (Model.GenderIdentity)(int)patient.GenderIdentity,
                patient.Surname,
                patient.Forename,
                patient.MiddleNames,
                patient.Title,
                cancellationToken);

            // TODO: PatientMapper
            var mappedPatient = new Patient
            {
                Id = updatedPatient.Id,
                NhsNumber = updatedPatient.NhsNumber,
                DateOfBirth = updatedPatient.DateOfBirth,
                SexAssignedAtBirth = (SexAssignedAtBirth)(int)updatedPatient.SexAssignedAtBirth,
                GenderIdentity = (GenderIdentity)(int)updatedPatient.GenderIdentity,
                Surname = updatedPatient.Surname,
                Forename = updatedPatient.Forename,
                MiddleNames = updatedPatient.MiddleNames,
                Title = updatedPatient.Title
            };

            return mappedPatient;
        }

        public async Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            return await _pandaRepository.DeletePatientByNhsNumber(nhsNumber, cancellationToken);
        }
    }
}
