namespace Panda.ClientModel
{
    public class Patient
    {
        public int Id { get; set; }

        public string NhsNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string Name { get; set; }

        public string Postcode { get; set; }
    }
}
