namespace Panda.Services.Mappers
{
    public class PatientMapper
    {
        public Model.Patient MapFromClientPatient(ClientModel.Patient clientPatient)
        {
            throw new NotImplementedException();
        }

        public ClientModel.Patient MapToClientPatient(Model.Patient patient)
        {
            var clientPatient = new ClientModel.Patient
            {
                Id = patient.Id,
                NhsNumber = patient.NhsNumber,
                DateOfBirth = patient.DateOfBirth,
                SexAssignedAtBirth = (ClientModel.SexAssignedAtBirth)(int)patient.SexAssignedAtBirth,
                GenderIdentity = (ClientModel.GenderIdentity)(int)patient.GenderIdentity,
                Surname = patient.Surname,
                Forename = patient.Forename,
                MiddleNames = patient.MiddleNames,
                Title = patient.Title
            };

            return clientPatient;
        }
    }
}
