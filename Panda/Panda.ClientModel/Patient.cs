namespace Panda.ClientModel
{
    public class Patient
    {
        public int Id { get; set; }

        public string NhsNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }

        // TODO: Change to string, and then map with validation
        public SexAssignedAtBirth SexAssignedAtBirth { get; set; }

        // TODO: Change to string, and then map with validation
        public GenderIdentity GenderIdentity { get; set; }

        public string Surname { get; set; }

        public string Forename { get; set; }

        public string MiddleNames { get; set; }

        public string Title { get; set; }
    }
}
