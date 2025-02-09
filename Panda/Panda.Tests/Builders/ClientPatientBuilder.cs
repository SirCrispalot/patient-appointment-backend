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
                Postcode = "EX8 3DT",
                Name = "Ada Lovelace"
            };
        }

        public Patient CreatePatientB()
        {
            return new Patient
            {
                NhsNumber = "6666666666",
                DateOfBirth = new DateOnly(1980, 12, 31),
                Postcode = "BS5 6PY",
                Name = "Chris Jones"
            };
        }

        public Patient CreatePatientC()
        {
            return new Patient
            {
                NhsNumber = "9999999999",
                DateOfBirth = new DateOnly(1952, 11, 16),
                Postcode = "PO6 1AZ",
                Name = "Shigeru Miyamoto"
            };
        }
    }
}
