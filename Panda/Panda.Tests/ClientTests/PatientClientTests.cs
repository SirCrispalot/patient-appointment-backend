using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Panda.ClientModel;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.Tests.Builders;
using Panda.WebApi.Controllers;

namespace Panda.Tests.ClientTests
{
    public class PatientClientTests
    {
        private PatientController _patientController;
        private ClientPatientBuilder _clientPatientBuilder;
        private PatientBuilder _patientBuilder;
        private PandaDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IPatientRepository, PatientRepository>();
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
        public async Task Given_EmptyRepository_When_CreatePatient_Should_ReturnHttp201WithPatient()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            var actionResult = await _patientController.Create(clientPatient, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<CreatedResult>();
            
            var result = (CreatedResult)actionResult.Result;
            result.Value.Should().BeOfType<Patient>();
            ((Patient)result.Value).NhsNumber.Should().Be(clientPatient.NhsNumber);
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateAndUpdatePatient_Should_ReturnHttp200WithUpdatedPatient()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, CancellationToken.None);
            var newSurname = "Smith";
            clientPatient.Surname = newSurname;
            var actionResult = await _patientController.Update(clientPatient, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<Patient>();
            ((Patient)result.Value).Surname.Should().Be(newSurname);
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateAndDeletePatient_Should_ReturnHttp204()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, CancellationToken.None);
            var actionResult = await _patientController.Delete(clientPatient.NhsNumber, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<NoContentResult>();
        }

        [Test]
        public async Task Given_RepositoryWithMultiplePatients_When_GetPatient_Should_ReturnHttp200WithCorrectPatient()
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
            var actionResult = await _patientController.Get(patientB.NhsNumber, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<Patient>();
            ((Patient)result.Value).NhsNumber.Should().Be(patientB.NhsNumber);
            ((Patient)result.Value).Surname.Should().Be(patientB.Surname);
        }

        [Test]
        public async Task Given_RepositoryWithMultiplePatients_When_GetDeletedPatient_Should_ReturnHttp404WithMessage()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            var patientB = _patientBuilder.CreatePatientB();
            _dbContext.Patients.Add(patientB);
            var patientC = _patientBuilder.CreatePatientC();
            patientC.DeletedDateTime = DateTime.UtcNow;
            _dbContext.Patients.Add(patientC);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var actionResult = await _patientController.Get(patientC.NhsNumber, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Patient with NHS number {patientC.NhsNumber} not found.");
        }

        [Test]
        public async Task Given_RepositoryWithMultiplePatients_When_GetNonExistentPatient_Should_ReturnHttp404WithMessage()
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
            var actionResult = await _patientController.Get(nhsNumber, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Patient with NHS number {nhsNumber} not found.");
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateSamePatientTwice_Should_ReturnHttp400WithMessage()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            await _patientController.Create(clientPatient, CancellationToken.None);
            var actionResult = await _patientController.Create(clientPatient, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
            var result = (BadRequestObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Patient with NHS number {clientPatient.NhsNumber} already exists");
        }

        [Test]
        public async Task Given_EmptyRepository_When_UpdateNonExistentPatient_Should_ReturnHttp201WithPatient()
        {
            // Arrange
            var clientPatient = _clientPatientBuilder.CreateClientPatientA();

            // Act
            var actionResult = await _patientController.Update(clientPatient, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<CreatedResult>();

            var result = (CreatedResult)actionResult.Result;
            result.Value.Should().BeOfType<Patient>();
            ((Patient)result.Value).NhsNumber.Should().Be(clientPatient.NhsNumber);
        }

        [Test]
        public async Task Given_RepositoryWithMultiplePatients_When_DeleteNonExistentPatient_Should_ReturnHttp404WithMessage()
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
            var actionResult = await _patientController.Delete(nhsNumber, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Patient with NHS number {nhsNumber} not found.");
        }
    }
}