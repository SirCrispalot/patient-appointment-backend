using Microsoft.AspNetCore.Mvc;
using Panda.ClientModel;

namespace Panda.WebApi.Controllers
{
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;

        public PatientController(ILogger<PatientController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets a patient by identifier
        /// </summary>
        [HttpGet]
        [Route("patient/{id}")]
        public async Task<Patient> Get(string id, CancellationToken cancellationToken)
        {
            return new Patient
            {
                Identifier = id,
                Forename = "Xav",
                Surname = "Smith",
                DateOfBirth = new DateTime(2009, 10, 29)
            };
        }

        /// <summary>
        /// Creates a new patient
        /// </summary>
        [HttpPost]
        [Route("patient/")]
        public async Task Create(Patient patient, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing patient, or creates them if they do not exist
        /// </summary>
        [HttpPut]
        [Route("patient/{id}")]
        public async Task Update(string id, Patient patient, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes a patient by identifier
        /// </summary>
        [HttpDelete]
        [Route("patient/{id}")]
        public async Task Delete(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
