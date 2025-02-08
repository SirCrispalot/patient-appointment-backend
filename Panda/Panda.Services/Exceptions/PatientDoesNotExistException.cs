namespace Panda.Services.Exceptions
{
    public class PatientDoesNotExistException : ApplicationException
    {
        public PatientDoesNotExistException(string nhsNumber)
            : base($"Patient with NHS number {nhsNumber} does not exist")
        {
        }
    }
}
