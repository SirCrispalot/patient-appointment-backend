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
            return await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.Id == id,
                cancellationToken);
        }

        public async Task<Patient?> GetPatientByNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            return await _pandaDbContext.Patients.SingleOrDefaultAsync(patient => patient.NhsNumber == nhsNumber,
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
    }
}
