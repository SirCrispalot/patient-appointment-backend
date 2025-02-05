namespace Panda.Model
{
    public class Patient
    {
        public int Id { get; set; }
        public string Identifier { get; set; }
        public IList<Appointment> Appointments { get; set; }
    }
}
