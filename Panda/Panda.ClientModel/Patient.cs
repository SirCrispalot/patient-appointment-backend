namespace Panda.ClientModel
{
    public class Patient
    {
        public required string Identifier { get; set; }

        public string Forename { get; set; }

        public string Surname { get; set; }

        public DateTime DateOfBirth { get; set; }
    }
}
