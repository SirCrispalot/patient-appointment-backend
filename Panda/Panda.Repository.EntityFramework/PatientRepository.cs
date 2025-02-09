using Microsoft.EntityFrameworkCore;
using Panda.Model;

namespace Panda.Repository.EntityFramework
{
    public class PatientRepository : IPatientRepository
    {
        private readonly PandaDbContext _pandaDbContext;

        public PatientRepository(PandaDbContext pandaDbContext)
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

        public async Task<Patient> CreatePatient(Patient patient, CancellationToken cancellationToken)
        {
            _pandaDbContext.Patients.Add(patient);

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var newPatient = await GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);

            return newPatient;
        }

        public async Task<Patient> UpdatePatientByNhsNumber(Patient patient, CancellationToken cancellationToken)
        {
            var existingPatient = await GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);

            existingPatient.DateOfBirth = patient.DateOfBirth;
            existingPatient.Name = patient.Name;
            existingPatient.Postcode = patient.Postcode;

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            var updatedPatient = await GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);

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
            
            // TODO: Assumption made that we are soft deleting patients, because you can't just delete clinical data.  Check retention requirements with business.
            patientToDelete.DeletedDateTime = DateTime.UtcNow;

            await _pandaDbContext.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
