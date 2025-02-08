using Panda.ClientModel;

namespace Panda.Tests.Builders
{
    public class ClientPatientBuilder
    {
        public Patient CreateClientPatientA()
        {
            return new Patient
            {
                NhsNumber = "4444444444",
                DateOfBirth = new DateOnly(1995, 1, 1),
                Forename = "Ada",
                Surname = "Lovelace",
                MiddleNames = "Matilda",
                SexAssignedAtBirth = SexAssignedAtBirth.Female,
                GenderIdentity = GenderIdentity.Woman,
                Title = "Miss"
            };
        }
    }
}
