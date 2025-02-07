using Microsoft.AspNetCore.Mvc;
using Panda.ClientModel;
using Panda.Model;
using Panda.Services;
using Panda.Services.Exceptions;
using Patient = Panda.ClientModel.Patient;

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
            Patient newPatient;
            try
            {
                newPatient = await _patientService.CreatePatient(patient, cancellationToken);
            }
            catch (PatientAlreadyExistsException exception)
            {
                return BadRequest(exception.Message);
            }

            return Created($"/patient/{newPatient.Id}", newPatient);
        }

        /// <summary>
        /// Updates an existing patient, or creates them if they do not exist
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("patient/")]
        public async Task<ActionResult<Patient>> Update(Patient patient, CancellationToken cancellationToken)
        {
            Patient existingPatient = null;
            if (patient.Id != 0)
            {
                existingPatient = await _patientService.GetPatientById(patient.Id, cancellationToken);
            }

            if (!string.IsNullOrWhiteSpace(patient.NhsNumber))
            {
                existingPatient = await _patientService.GetPatientByNhsNumber(patient.NhsNumber, cancellationToken);
            }

            if (existingPatient != null)
            {
                return Ok(await _patientService.UpdatePatient(patient, cancellationToken));
            }

            // Still need to wrap this in a try/catch.  Another consumer could have created this patient since we checked.
            Patient newPatient;
            try
            {
                newPatient = await _patientService.CreatePatient(patient, cancellationToken);
            }
            catch (PatientAlreadyExistsException exception)
            {
                return BadRequest(exception.Message);
            }

            return Created($"/patient/{newPatient.Id}", newPatient);
        }

        /// <summary>
        /// Deletes a patient by NHS number
        /// </summary>
        [HttpDelete]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [Route("patient/{nhsNumber}")]
        public async Task<ActionResult> Delete(string nhsNumber, CancellationToken cancellationToken)
        {
            bool isDeleted = await _patientService.DeletePatientByNhsNumber(nhsNumber, cancellationToken);

            if (!isDeleted)
            {
                return NotFound($"Patient with NHS number {nhsNumber} not found.");
            }

            return NoContent();
        }
    }
}
