using Microsoft.AspNetCore.Mvc;
using Panda.ClientModel;
using Panda.Services;
using Panda.Services.Exceptions;
using Panda.WebApi.Validators;

namespace Panda.WebApi.Controllers
{
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private IAppointmentService _appointmentService;
        private MissedAppointmentReportRequestValidator _missedAppointmentReportRequestValidator;

        public AppointmentController(ILogger<AppointmentController> logger, IAppointmentService appointmentService,
            MissedAppointmentReportRequestValidator missedAppointmentReportRequestValidator)
        {
            _logger = logger;
            _appointmentService = appointmentService;
            _missedAppointmentReportRequestValidator = missedAppointmentReportRequestValidator;
        }

        /// <summary>
        /// Gets all appointments by patient id
        /// </summary>
        [HttpGet]
        [Route("appointment/patient/{id}")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetByPatientId(int id, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientId(id, cancellationToken);

            return Ok(appointments);
        }

        /// <summary>
        /// Gets all appointments by patient NHS number
        /// </summary>
        [HttpGet]
        [Route("appointment/patient/nhs-number/{nhsNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetByPatientNhsNumber(string nhsNumber, CancellationToken cancellationToken)
        {
            var appointments = await _appointmentService.GetAppointmentsByPatientNhsNumber(nhsNumber, cancellationToken);

            return Ok(appointments);
        }

        /// <summary>
        /// Gets an appointment by id
        /// </summary>
        [HttpGet]
        [Route("appointment/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Appointment>> Get(int id, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentService.GetAppointmentById(id, cancellationToken);

            if (appointment == null)
            {
                return NotFound($"Appointment with id {id} not found.");
            }

            return Ok(appointment);
        }

        /// <summary>
        /// Creates a new appointment
        /// </summary>
        [HttpPost]
        [Route("appointment/")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Appointment>> Create(Appointment appointment, CancellationToken cancellationToken)
        {
            // TODO: Appointment validation has not yet been built. See PatientController.Create() for example of how FluentValidation would be used.

            Appointment newAppointment;
            try
            {
                newAppointment = await _appointmentService.CreateAppointment(appointment, cancellationToken);
            }
            catch (PatientDoesNotExistException exception)
            {
                return BadRequest(exception.Message);
            }
            catch (AppointmentAlreadyExistsException exception)
            {
                return BadRequest(exception.Message);
            }

            return Created($"/appointment/{newAppointment.Id}", newAppointment);
        }

        /// <summary>
        /// Updates an existing appointment, or creates it if it does not exist
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("appointment/")]
        public async Task<ActionResult<Appointment>> Update(Appointment appointment, CancellationToken cancellationToken)
        {
            Appointment existingAppointment = null;
            if (appointment.Id != 0)
            {
                existingAppointment = await _appointmentService.GetAppointmentById(appointment.Id, cancellationToken);
            }
            
            if (existingAppointment != null)
            {
                try
                {
                    return Ok(await _appointmentService.UpdateAppointment(appointment, cancellationToken));
                }
                catch (AppointmentStatusException exception)
                {
                    return BadRequest(exception.Message);
                }
            }

            // Still need to wrap this in a try/catch.  Another consumer could have created this patient since we checked.
            Appointment newAppointment;
            try
            {
                newAppointment = await _appointmentService.CreateAppointment(appointment, cancellationToken);
            }
            catch (AppointmentAlreadyExistsException exception)
            {
                return BadRequest(exception.Message);
            }

            return Created($"/appointment/{newAppointment.Id}", newAppointment);
        }

        /// <summary>
        /// Cancels an existing appointment
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("appointment/{id}/cancel")]
        public async Task<ActionResult> Cancel(int id, CancellationToken cancellationToken)
        {
            try
            {
                await _appointmentService.CancelAppointmentById(id, cancellationToken);
            }
            catch (AppointmentDoesNotExistException exception)
            {
                return NotFound(exception.Message);
            }
            catch (AppointmentStatusException exception)
            {
                return BadRequest(exception.Message);
            }

            return NoContent();
        }

        /// <summary>
        /// Attends an existing appointment
        /// </summary>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [Route("appointment/{id}/attend")]
        public async Task<ActionResult> Attend(int id, CancellationToken cancellationToken)
        {
            bool isAttended;
            try
            {
                isAttended = await _appointmentService.AttendAppointmentById(id, cancellationToken);
            }
            catch (AppointmentDoesNotExistException exception)
            {
                return NotFound(exception.Message);
            }
            catch (AppointmentStatusException exception)
            {
                return BadRequest(exception.Message);
            }

            if (!isAttended)
            {
                return NotFound($"Appointment with id {id} not found.");
            }

            return NoContent();
        }

        /// <summary>
        /// Gets a report of missed appointments by date, department and clinician
        /// </summary>
        [HttpPost]
        [Route("appointment/missed-appointment-report")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MissedAppointmentReportResponse>> GetMissedAppointments(MissedAppointmentReportRequest request, CancellationToken cancellationToken)
        {
            var validationResult = await _missedAppointmentReportRequestValidator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }

            return Ok(await _appointmentService.GetMissedAppointments(request, cancellationToken));
        }
    }
}
