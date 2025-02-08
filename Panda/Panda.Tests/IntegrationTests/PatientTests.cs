using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.WebApi.Controllers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Panda.Services.Exceptions;
using Panda.Tests.Builders;
using Microsoft.AspNetCore.Mvc;

namespace Panda.Tests.IntegrationTests
{
    public class PatientTests
    {
        private PatientController _patientController;
        private ClientPatientBuilder _clientPatientBuilder;
        private PatientBuilder _patientBuilder;
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
            _patientBuilder = new PatientBuilder();

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
            await _patientController.Create(clientPatient, CancellationToken.None);

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
            await _patientController.Create(clientPatient, CancellationToken.None);

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.NhsNumber.Should().Be(clientPatient.NhsNumber);
            savedPatient.DateOfBirth.Should().Be(clientPatient.DateOfBirth);
            savedPatient.Forename.Should().Be(clientPatient.Forename);
            savedPatient.Surname.Should().Be(clientPatient.Surname);
            savedPatient.MiddleNames.Should().Be(clientPatient.MiddleNames);
            savedPatient.Title.Should().Be(clientPatient.Title);
            savedPatient.SexAssignedAtBirth.Should()
                .Be((Model.SexAssignedAtBirth)(int)clientPatient.SexAssignedAtBirth);
            savedPatient.GenderIdentity.Should().Be((Model.GenderIdentity)(int)clientPatient.GenderIdentity);
            savedPatient.DeletedDateTime.Should().BeNull();
        }

        [Test]
        public async Task
            Given_EmptyRepository_When_CreateAndUpdatePatient_Should_UpdateExistingPatientWithCorrectData()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, CancellationToken.None);

            var newSurname = "Smith";
            clientPatient.Surname = newSurname;
            await _patientController.Update(clientPatient, CancellationToken.None);

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
            await _patientController.Create(clientPatient, CancellationToken.None);
            await _patientController.Delete(clientPatient.NhsNumber, CancellationToken.None);

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.DeletedDateTime.Should().NotBeNull();
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateSamePatientTwice_Should_NotCreateDuplicate()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, CancellationToken.None);
            await _patientController.Create(clientPatient, CancellationToken.None);

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.NhsNumber.Should().Be(clientPatient.NhsNumber);
        }

        [Test]
        public async Task Given_EmptyRepository_When_UpdateNonExistentPatient_Should_CreatePatientWithCorrectData()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Update(clientPatient, CancellationToken.None);

            // Assert
            _dbContext.Patients.Count().Should().Be(1);
            var savedPatient = _dbContext.Patients.Single();
            savedPatient.NhsNumber.Should().Be(clientPatient.NhsNumber);
        }

        [Test]
        public async Task Given_RepositoryWithMultiplePatients_When_DeleteNonExistentPatient_Should_HaveNoEffect()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            var patientB = _patientBuilder.CreatePatientB();
            _dbContext.Patients.Add(patientB);
            var patientC = _patientBuilder.CreatePatientC();
            _dbContext.Patients.Add(patientC);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var nhsNumber = "1111111111";
            await _patientController.Delete(nhsNumber, CancellationToken.None);

            // Assert
            _dbContext.Patients.Count().Should().Be(3);
            _dbContext.Patients.Should().AllSatisfy(patient => { patient.DeletedDateTime.Should().BeNull(); });
        }
    }
}