namespace Panda.Model
{
    // TODO: Clearly there are other fields we may wish to capture for this system.
    // However, without further requirements I would be guessing, so have captured the basics to demonstrate the functionality.
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

        public IList<Appointment> Appointments { get; set; }

        public DateTime? DeletedDateTime { get; set; }
    }
}
