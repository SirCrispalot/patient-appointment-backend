using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Panda.ClientModel;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.WebApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Panda.Tests.EndToEndTests
{
    public class Tests
    {
        private PatientController _patientController;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IPandaRepository, PandaRepository>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddDbContext<PandaDbContext>(opt => opt.UseInMemoryDatabase("PandaDb"));
            services.AddLogging(conf => conf.AddConsole());
            var provider = services.BuildServiceProvider();

            //_patientController = Activator.CreateInstance<PatientController>();
            _patientController = ActivatorUtilities.CreateInstance<PatientController>(provider);
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreatePatient_Should_AddPatientToRepository()
        {
            // Arrange
            var clientPatient = new Patient
            {
                NhsNumber = "4444444444",
                DateOfBirth = new DateOnly(1995, 1, 1),
                Forename = "Ada",
                Surname = "Lovelace",
                MiddleNames = "Matilda",
                SexAssignedAtBirth = SexAssignedAtBirth.Female,
                GenderIdentity = GenderIdentity.Woman,
                Title = "Miss"
            };

            // Act
            var actionResult = await _patientController.Create(clientPatient, new CancellationToken(false));

            // Assert
            actionResult.Result.Should().BeOfType<CreatedResult>();
            
            var result = (CreatedResult)actionResult.Result;
            result.Value.Should().BeOfType<Patient>();
            
            var patient = (Patient)result.Value;
            patient.NhsNumber.Should().Be(clientPatient.NhsNumber);
            patient.Id.Should().Be(1);
        }
    }
}