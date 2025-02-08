using Panda.Model;

namespace Panda.Repository
{
    public interface IPatientRepository
    {
        Task<Patient?> GetPatientById(int id, CancellationToken cancellationToken);

        Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);

        // TODO: Just use model
        Task<Patient> CreatePatient(string nhsNumber, DateOnly dateOfBirth,
            SexAssignedAtBirth sexAssignedAtBirth, GenderIdentity genderIdentity, string surname,
            string forename, string middleNames, string title, CancellationToken cancellationToken);

        // TODO: Just use model
        Task<Patient> UpdatePatientByNhsNumber(string patientNhsNumber, DateOnly patientDateOfBirth,
            SexAssignedAtBirth patientSexAssignedAtBirth, GenderIdentity patientGenderIdentity, string patientSurname,
            string patientForename, string patientMiddleNames, string patientTitle,
            CancellationToken cancellationToken);

        Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken);
    }
}
