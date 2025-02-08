using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.WebApi.Controllers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Panda.Tests.Builders;

namespace Panda.Tests.IntegrationTests
{
    public class PatientTests
    {
        private PatientController _patientController;
        private ClientPatientBuilder _clientPatientBuilder;
        private PandaDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IPandaRepository, PandaRepository>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddDbContext<PandaDbContext>(opt => opt.UseInMemoryDatabase("PandaDb"));
            services.AddLogging(conf => conf.AddConsole());
            var provider = services.BuildServiceProvider();

            _patientController = ActivatorUtilities.CreateInstance<PatientController>(provider);
            _clientPatientBuilder = new ClientPatientBuilder();

            _dbContext = (PandaDbContext)provider.GetService(typeof(PandaDbContext));
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Patients.RemoveRange(_dbContext.Patients);
            _dbContext.Appointments.RemoveRange(_dbContext.Appointments);
            _dbContext.SaveChangesAsync();
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreatePatient_Should_AddPatientToRepository()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, new CancellationToken(false));

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            _dbContext.Patients.Single().NhsNumber.Should().Be(clientPatient.NhsNumber);
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreatePatient_Should_AddPatientToRepositoryWithCorrectData()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, new CancellationToken(false));

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.NhsNumber.Should().Be(clientPatient.NhsNumber);
            savedPatient.DateOfBirth.Should().Be(clientPatient.DateOfBirth);
            savedPatient.Forename.Should().Be(clientPatient.Forename);
            savedPatient.Surname.Should().Be(clientPatient.Surname);
            savedPatient.MiddleNames.Should().Be(clientPatient.MiddleNames);
            savedPatient.Title.Should().Be(clientPatient.Title);
            savedPatient.SexAssignedAtBirth.Should().Be((Model.SexAssignedAtBirth)(int)clientPatient.SexAssignedAtBirth);
            savedPatient.GenderIdentity.Should().Be((Model.GenderIdentity)(int)clientPatient.GenderIdentity);
            savedPatient.DeletedDateTime.Should().BeNull();
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateAndUpdatePatient_Should_UpdateExistingPatientWithCorrectData()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, new CancellationToken(false));

            var newSurname = "Smith";
            clientPatient.Surname = newSurname;
            await _patientController.Update(clientPatient, new CancellationToken(false));

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.Surname.Should().Be(newSurname);
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateAndDeletePatient_Should_MarkExistingPatientAsDeleted()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, new CancellationToken(false));
            await _patientController.Delete(clientPatient.NhsNumber, new CancellationToken(false));

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.DeletedDateTime.Should().NotBeNull();
        }
    }
}