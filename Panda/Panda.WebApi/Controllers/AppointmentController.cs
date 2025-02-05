using Microsoft.AspNetCore.Mvc;
using Panda.ClientModel;

namespace Panda.WebApi.Controllers
{
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(ILogger<AppointmentController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets an appointment by identifier
        /// </summary>
        [HttpGet]
        [Route("appointment/{id}")]
        public async Task<Appointment> Get(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a new appointment
        /// </summary>
        [HttpPost]
        [Route("appointment/")]
        public async Task Create(Appointment appointment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates an existing appointment, or creates it if it does not exist
        /// </summary>
        [HttpPut]
        [Route("appointment/{id}")]
        public async Task Update(string id, Appointment appointment, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes an appointment by identifier
        /// </summary>
        [HttpDelete]
        [Route("appointment/{id}")]
        public async Task Delete(string id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
