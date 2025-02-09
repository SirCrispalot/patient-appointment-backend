namespace Panda.Model
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

        public IList<Appointment> Appointments { get; set; }

        public DateTime? DeletedDateTime { get; set; }

        // TODO: Clearly this is not a fully comprehensive list of fields.  Check with business what is required.
    }
}
