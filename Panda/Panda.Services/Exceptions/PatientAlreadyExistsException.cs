namespace Panda.Services.Exceptions
{
    public class PatientAlreadyExistsException : ApplicationException
    {
        public PatientAlreadyExistsException(string nhsNumber)
            : base($"Patient with NHS number {nhsNumber} already exists")
        {
        }
    }
}
