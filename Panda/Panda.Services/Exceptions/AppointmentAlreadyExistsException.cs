namespace Panda.Services.Exceptions
{
    public class AppointmentAlreadyExistsException : ApplicationException
    {
        public AppointmentAlreadyExistsException(string nhsNumber, DateTime appointmentDateTime)
            : base($"Appointment on {appointmentDateTime:D} at {appointmentDateTime:T} for patient with NHS number {nhsNumber} already exists")
        {
        }
    }
}
