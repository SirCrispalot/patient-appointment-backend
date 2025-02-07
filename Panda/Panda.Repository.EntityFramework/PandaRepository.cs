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

        public async Task<Patient?> GetPatientById(int id, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.Id == id && !patient.DeletedDateTime.HasValue,
                cancellationToken);
        }

        public async Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.NhsNumber == nhsNumber && !patient.DeletedDateTime.HasValue,
                cancellationToken);
        }

        public async Task<Patient> CreatePatient(string nhsNumber, DateOnly dateOfBirth, SexAssignedAtBirth sexAssignedAtBirth,
            GenderIdentity genderIdentity, string surname, string forename, string middleNames,
            string title, CancellationToken cancellationToken)
        {
            var patient = new Patient
            {
                NhsNumber = nhsNumber,
                DateOfBirth = dateOfBirth,
                SexAssignedAtBirth = sexAssignedAtBirth,
                GenderIdentity = genderIdentity,
                Surname = surname,
                Forename = forename,
                MiddleNames = middleNames,
                Title = title
            };

            _pandaDbContext.Patients.Add(patient);

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var newPatient = await GetPatientByNhsNumber(nhsNumber, cancellationToken);

            return newPatient;
        }

        public async Task<Patient> UpdatePatientByNhsNumber(string nhsNumber, DateOnly dateOfBirth, SexAssignedAtBirth sexAssignedAtBirth,
            GenderIdentity genderIdentity, string surname, string forename, string middleNames,
            string title, CancellationToken cancellationToken)
        {
            var existingPatient = await GetPatientByNhsNumber(nhsNumber, cancellationToken);

            existingPatient.DateOfBirth = dateOfBirth;
            existingPatient.SexAssignedAtBirth = sexAssignedAtBirth;
            existingPatient.GenderIdentity = genderIdentity;
            existingPatient.Surname = surname;
            existingPatient.Forename = forename;
            existingPatient.MiddleNames = middleNames;
            existingPatient.Title = title;

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var updatedPatient = await GetPatientByNhsNumber(nhsNumber, cancellationToken);

            return updatedPatient;
        }

        public async Task<bool> DeletePatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var patientToDelete = await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.NhsNumber == nhsNumber,
                cancellationToken);

            if (patientToDelete == null)
            {
                return false;
            }

            // TODO: Decide whether we're hard or soft deleting
            var softDelete = true;

            if (softDelete)
            {
                patientToDelete.DeletedDateTime = DateTime.Now;
            }
            else
            {
                _pandaDbContext.Patients.Remove(patientToDelete);
            }

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
