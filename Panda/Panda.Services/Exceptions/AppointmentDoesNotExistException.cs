namespace Panda.Services.Exceptions
{
    public class AppointmentDoesNotExistException : ApplicationException
    {
        public AppointmentDoesNotExistException(int id)
            : base($"Appointment with id {id} does not exist")
        {
        }
    }
}
