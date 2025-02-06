using Microsoft.AspNetCore.Mvc;
using Panda.ClientModel;
using Panda.Services;

namespace Panda.WebApi.Controllers
{
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly ILogger<PatientController> _logger;
        private IPatientService _patientService;

        public PatientController(ILogger<PatientController> logger, IPatientService patientService)
        {
            _logger = logger;
            _patientService = patientService;
        }

        /// <summary>
        /// Gets a patient by identifier
        /// </summary>
        [HttpGet]
        [Route("patient/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> Get(int id, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientById(id, cancellationToken);

            if (patient == null)
            {
                return NotFound($"Patient with id {id} not found.");
            }

            return Ok(patient);
        }

        /// <summary>
        /// Gets a patient by NHS number
        /// </summary>
        [HttpGet]
        [Route("patient/nhs-number/{nhsNumber}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Patient>> Get(string nhsNumber, CancellationToken cancellationToken)
        {
            var patient = await _patientService.GetPatientByNhsNumber(nhsNumber, cancellationToken);

            if (patient == null)
            {
                return NotFound($"Patient with NHS number {nhsNumber} not found.");
            }

            return Ok(patient);
        }

        /// <summary>
        /// Creates a new patient
        /// </summary>
        [HttpPost]
        [Route("patient/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Patient>> Create(Patient patient, CancellationToken cancellationToken)
        {
            // TODO: Validation, maybe return 400
            var newPatient = await _patientService.CreatePatient(patient, cancellationToken);

            return Created($"/patient/{newPatient.Id}", newPatient);
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
