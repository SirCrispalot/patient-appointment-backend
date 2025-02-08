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

        public Patient CreatePatientB()
        {
            return new Patient
            {
                NhsNumber = "6666666666",
                DateOfBirth = new DateOnly(1980, 12, 31),
                Forename = "Chris",
                Surname = "Jones",
                MiddleNames = "Alex",
                SexAssignedAtBirth = SexAssignedAtBirth.Male,
                GenderIdentity = GenderIdentity.NonBinary,
                Title = "Mx"
            };
        }

        public Patient CreatePatientC()
        {
            return new Patient
            {
                NhsNumber = "9999999999",
                DateOfBirth = new DateOnly(1952, 11, 16),
                Forename = "Shigeru",
                Surname = "Miyamoto",
                MiddleNames = "",
                SexAssignedAtBirth = SexAssignedAtBirth.Male,
                GenderIdentity = GenderIdentity.Man,
                Title = "Mr"
            };
        }
    }
}
