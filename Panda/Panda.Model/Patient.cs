namespace Panda.Model
{
    public class Patient
    {
        public int Id { get; set; }

        public string NhsNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Name { get; set; }

        public string Postcode { get; set; }

        public IList<Appointment> Appointments { get; set; }

        public DateTime? DeletedDateTime { get; set; }

        // TODO: Clearly this is not a fully comprehensive list of fields.  Check with business what is required.
    }
}
