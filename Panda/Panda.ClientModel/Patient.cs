namespace Panda.ClientModel
{
    public class Patient
    {
        public int Id { get; set; }

        public string NhsNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public SexAssignedAtBirth SexAssignedAtBirth { get; set; }

        public GenderIdentity GenderIdentity { get; set; }

        public string Surname { get; set; }

        public string Forename { get; set; }

        public string MiddleNames { get; set; }

        public string Title { get; set; }
    }
}
