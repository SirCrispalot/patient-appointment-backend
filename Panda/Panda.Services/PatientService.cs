using Panda.ClientModel;
using Panda.Repository;
using Panda.Services.Exceptions;
using Panda.Services.Mappers;

namespace Panda.Services
{
    public class PatientService : IPatientService
    {
        private IPatientRepository _patientRepository;
        private PatientMapper _patientMapper;

        public PatientService(IPatientRepository patientRepository, PatientMapper patientMapper)
        {
            _patientRepository = patientRepository;
            _patientMapper = patientMapper;
        }

        public async Task<Patient?> GetPatientById(int id, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientById(id, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var clientPatient = _patientMapper.MapToClientPatient(patient);

            return clientPatient;
        }

        public async Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByNhsNumber(nhsNumber, cancellationToken);

            if (patient == null)
            {
                return null;
            }

            var clientPatient = _patientMapper.MapToClientPatient(patient);

            return clientPatient;
        }

        public async Task<Patient> CreatePatient(Patient patient, CancellationToken cancellationToken)
        {
            var existingPatient = await _patientRepository.GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);

            if (existingPatient != null)
            {
                throw new PatientAlreadyExistsException(patient.NhsNumber);
            }

            var newPatient = await _patientRepository.CreatePatient(
                patient.NhsNumber,
                patient.DateOfBirth,
                (Model.SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                (Model.GenderIdentity)(int)patient.GenderIdentity,
                patient.Surname, 
                patient.Forename, 
                patient.MiddleNames, 
                patient.Title, 
                cancellationToken);

            var mappedPatient = _patientMapper.MapToClientPatient(newPatient);

            return mappedPatient;
        }

        public async Task<Patient> UpdatePatient(Patient patient, CancellationToken cancellationToken)
        {
            var updatedPatient = await _patientRepository.UpdatePatientByNhsNumber(
                patient.NhsNumber,
                patient.DateOfBirth,
                (Model.SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                (Model.GenderIdentity)(int)patient.GenderIdentity,
                patient.Surname,
                patient.Forename,
                patient.MiddleNames,
                patient.Title,
                cancellationToken);

            var mappedPatient = _patientMapper.MapToClientPatient(updatedPatient);

            return mappedPatient;
        }

        public async Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            return await _patientRepository.DeletePatientByNhsNumber(nhsNumber, cancellationToken);
        }
    }
}
