using Panda.Model;

namespace Panda.Services.Exceptions
{
    public class AppointmentStatusException : ApplicationException
    {
        public AppointmentStatusException(int id, AppointmentStatus status)
            : base($"Appointment with id {id} cannot be changed from state {status}")
        {
        }
    }
}