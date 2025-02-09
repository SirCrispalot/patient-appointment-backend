namespace Panda.Services.Mappers
{
    public class PatientMapper
    {
        public Model.Patient MapFromClientPatient(ClientModel.Patient clientPatient)
        {
            var patient = new Model.Patient
            {
                NhsNumber = clientPatient.NhsNumber,
                DateOfBirth = clientPatient.DateOfBirth,
                Name = clientPatient.Name,
                Postcode = clientPatient.Postcode
            };

            return patient;
        }

        public ClientModel.Patient MapToClientPatient(Model.Patient patient)
        {
            var clientPatient = new ClientModel.Patient
            {
                Id = patient.Id,
                NhsNumber = patient.NhsNumber,
                DateOfBirth = patient.DateOfBirth,
                Name = patient.Name,
                Postcode = patient.Postcode
            };

            return clientPatient;
        }
    }
}
