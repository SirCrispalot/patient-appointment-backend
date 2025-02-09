using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Panda.ClientModel;
using Panda.Model;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.Services.Mappers;
using Panda.Tests.Builders;
using Panda.WebApi.Controllers;
using Panda.WebApi.Validators;
using Appointment = Panda.ClientModel.Appointment;

namespace Panda.Tests.ClientTests
{
    public class AppointmentClientTests
    {
        private AppointmentController _appointmentController;
        private ClientPatientBuilder _clientPatientBuilder;
        private ClientAppointmentBuilder _clientAppointmentBuilder;
        private PatientBuilder _patientBuilder;
        private AppointmentBuilder _appointmentBuilder;
        private PandaDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<MissedAppointmentReportRequestValidator>();
            services.AddScoped<AppointmentMapper>();
            services.AddDbContext<PandaDbContext>(opt => opt.UseInMemoryDatabase("PandaDb"));
            services.AddLogging(conf => conf.AddConsole());
            var provider = services.BuildServiceProvider();

            _appointmentController = ActivatorUtilities.CreateInstance<AppointmentController>(provider);
            _clientPatientBuilder = new ClientPatientBuilder();
            _patientBuilder = new PatientBuilder();
            _clientAppointmentBuilder = new ClientAppointmentBuilder();
            _appointmentBuilder = new AppointmentBuilder();

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
        public async Task Given_RepositoryWithPatient_When_CreateAppointment_Should_ReturnHttp201WithAppointment()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            var actionResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<CreatedResult>();

            var result = (CreatedResult)actionResult.Result;
            result.Value.Should().BeOfType<Appointment>();
            ((Appointment)result.Value).PatientNhsNumber.Should().Be(patientA.NhsNumber);
            ((Appointment)result.Value).Status.Should().Be(nameof(AppointmentStatus.Booked));
        }

        [Test]
        public async Task Given_EmptyRepository_When_CreateAppointment_Should_ReturnHttp400()
        {
            // Arrange
            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            
            // Act
            var actionResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();

            var result = (BadRequestObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Patient with NHS number {clientAppointmentA.PatientNhsNumber} does not exist");
        }

        
        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateAndUpdateAppointment_Should_ReturnHttp200WithUpdatedAppointment()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            var createdAppointmentResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            var createdAppointment = (CreatedResult)createdAppointmentResult.Result;
            var appointmentId = ((Appointment)createdAppointment.Value).Id;
            var newAppointmentDateTime = new DateTime(2026, 2, 28, 15, 30, 0);
            clientAppointmentA.AppointmentDateTime = newAppointmentDateTime;
            clientAppointmentA.Id = appointmentId;

            var actionResult = await _appointmentController.Update(clientAppointmentA, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<Appointment>();
            ((Appointment)result.Value).AppointmentDateTime.Should().Be(newAppointmentDateTime);
        }

        
        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateAndCancelAndAttendAppointment_Should_ReturnHttp400()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            var createdAppointmentResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            var createdAppointment = (CreatedResult)createdAppointmentResult.Result;
            var appointmentId = ((Appointment)createdAppointment.Value).Id;
            
            await _appointmentController.Cancel(appointmentId, CancellationToken.None);
            var actionResult = await _appointmentController.Attend(appointmentId, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var result = (BadRequestObjectResult)actionResult;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Appointment with id {appointmentId} cannot be changed from state {AppointmentStatus.Cancelled}");
        }

        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateAndAttendAndCancelAppointment_Should_ReturnHttp400()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            var createdAppointmentResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            var createdAppointment = (CreatedResult)createdAppointmentResult.Result;
            var appointmentId = ((Appointment)createdAppointment.Value).Id;

            await _appointmentController.Attend(appointmentId, CancellationToken.None);
            var actionResult = await _appointmentController.Cancel(appointmentId, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<BadRequestObjectResult>();
            var result = (BadRequestObjectResult)actionResult;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Appointment with id {appointmentId} cannot be changed from state {AppointmentStatus.Attended}");
        }

        
        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateAndCancelAndReinstateAppointment_Should_ReturnHttp400()
        {
            // Arrange
           var patientA = _patientBuilder.CreatePatientA();
           _dbContext.Patients.Add(patientA);
           await _dbContext.SaveChangesAsync(CancellationToken.None);
           var patientAId = _dbContext.Patients.Single().Id;
           
           var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
           clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
           clientAppointmentA.PatientId = patientAId;
           
           // Act
           var createdAppointmentResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
           var createdAppointment = (CreatedResult)createdAppointmentResult.Result;
           var appointmentId = ((Appointment)createdAppointment.Value).Id;
           clientAppointmentA.Id = appointmentId;
           
           await _appointmentController.Cancel(appointmentId, CancellationToken.None);
           
           var actionResult = await _appointmentController.Update(clientAppointmentA, CancellationToken.None);
           
           // Assert
           actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
           ((BadRequestObjectResult)actionResult.Result).Value.Should().BeOfType<string>();
           var stringResult = (string)((BadRequestObjectResult)actionResult.Result).Value;
           stringResult.Should()
               .Be($"Appointment with id {appointmentId} cannot be changed from state {AppointmentStatus.Cancelled}");
        }

        
        [Test]
        public async Task Given_RepositoryWithMultipleAppointments_When_GetAppointment_Should_ReturnHttp200WithCorrectAppointment()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            _dbContext.Patients.Add(patientA);
            _dbContext.Appointments.Add(appointmentA);
            var patientB = _patientBuilder.CreatePatientB();
            var appointmentB = _appointmentBuilder.CreateAppointmentB(patientB);
            _dbContext.Patients.Add(patientB);
            _dbContext.Appointments.Add(appointmentB);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            var actionResult = await _appointmentController.Get(appointmentB.Id, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();

            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<Appointment>();
            ((Appointment)result.Value).PatientNhsNumber.Should().Be(patientB.NhsNumber);
            ((Appointment)result.Value).AppointmentDateTime.Should().Be(appointmentB.AppointmentDateTime);
        }


        [Test]
        public async Task
            Given_RepositoryWithMultipleAppointments_When_GetNonExistentAppointment_Should_ReturnHttp404WithMessage()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            _dbContext.Patients.Add(patientA);
            _dbContext.Appointments.Add(appointmentA);
            var patientB = _patientBuilder.CreatePatientB();
            var appointmentB = _appointmentBuilder.CreateAppointmentB(patientB);
            _dbContext.Patients.Add(patientB);
            _dbContext.Appointments.Add(appointmentB);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            int nonExistentId = 98765;
            var actionResult = await _appointmentController.Get(nonExistentId, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Appointment with id {nonExistentId} not found.");
        }

        
        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateSameAppointmentTwice_Should_ReturnHttp400WithMessage()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            var actionResult = await _appointmentController.Create(clientAppointmentA, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<BadRequestObjectResult>();
            var result = (BadRequestObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should()
                .Be(
                    $"Appointment on {clientAppointmentA.AppointmentDateTime:D} at {clientAppointmentA.AppointmentDateTime:T} for patient with NHS number {clientAppointmentA.PatientNhsNumber} already exists");
        }

        
        [Test]
        public async Task Given_RepositoryWithPatient_When_UpdateNonExistentAppointment_Should_ReturnHttp201WithAppointment()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            var actionResult = await _appointmentController.Update(clientAppointmentA, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<CreatedResult>();

            var result = (CreatedResult)actionResult.Result;
            result.Value.Should().BeOfType<Appointment>();
            ((Appointment)result.Value).PatientNhsNumber.Should().Be(patientA.NhsNumber);
            ((Appointment)result.Value).Status.Should().Be(nameof(AppointmentStatus.Booked));
        }

        
        [Test]
        public async Task Given_RepositoryWithMultipleAppointments_When_CancelNonExistentAppointment_Should_ReturnHttp404WithMessage()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            _dbContext.Patients.Add(patientA);
            _dbContext.Appointments.Add(appointmentA);
            var patientB = _patientBuilder.CreatePatientB();
            var appointmentB = _appointmentBuilder.CreateAppointmentB(patientB);
            _dbContext.Patients.Add(patientB);
            _dbContext.Appointments.Add(appointmentB);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            int nonExistentId = 668877;
            var actionResult = await _appointmentController.Cancel(nonExistentId, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Appointment with id {nonExistentId} does not exist");
        }
        
        [Test]
        public async Task Given_RepositoryWithMultipleAppointments_When_AttendNonExistentAppointment_Should_ReturnHttp404WithMessage()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            _dbContext.Patients.Add(patientA);
            _dbContext.Appointments.Add(appointmentA);
            var patientB = _patientBuilder.CreatePatientB();
            var appointmentB = _appointmentBuilder.CreateAppointmentB(patientB);
            _dbContext.Patients.Add(patientB);
            _dbContext.Appointments.Add(appointmentB);
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            int nonExistentId = 668877;
            var actionResult = await _appointmentController.Attend(nonExistentId, CancellationToken.None);

            // Assert
            actionResult.Should().BeOfType<NotFoundObjectResult>();
            var result = (NotFoundObjectResult)actionResult;
            result.Value.Should().BeOfType<string>();
            ((string)result.Value).Should().Be($"Appointment with id {nonExistentId} does not exist");
        }

        [Test]
        public async Task Given_EmptyRepository_When_RequestMissedAppointmentReport_Should_ReturnHttp200WithNoAppointments()
        {
            // Arrange
            var request = new MissedAppointmentReportRequest();
            request.ClinicianCode = "";
            request.DepartmentCode = "";
            request.ReportFrom = new DateTime(2025, 1, 10);
            request.ReportTo = new DateTime(2025, 1, 17);

            // Act
            var actionResult = await _appointmentController.GetMissedAppointments(request, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<MissedAppointmentReportResponse>();
            var missedAppointmentReport = (MissedAppointmentReportResponse)result.Value;
            ((MissedAppointmentReportResponse)result.Value).Appointments.Should().BeEmpty();
        }

        [Test]
        public async Task Given_RepositoryWithMissedAppointment_When_RequestMissedAppointmentReport_Should_ReturnHttp200WithThatAppointment()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            appointmentA.AppointmentDateTime = new DateTime(2025, 1, 12);
            appointmentA.Status = AppointmentStatus.Booked;
            _dbContext.Patients.Add(patientA);
            _dbContext.Appointments.Add(appointmentA);
            await _dbContext.SaveChangesAsync(CancellationToken.None);


            var request = new MissedAppointmentReportRequest();
            request.ClinicianCode = "";
            request.DepartmentCode = "";
            request.ReportFrom = new DateTime(2025, 1, 10);
            request.ReportTo = new DateTime(2025, 1, 17);

            // Act
            var actionResult = await _appointmentController.GetMissedAppointments(request, CancellationToken.None);

            // Assert
            actionResult.Result.Should().BeOfType<OkObjectResult>();
            var result = (OkObjectResult)actionResult.Result;
            result.Value.Should().BeOfType<MissedAppointmentReportResponse>();
            var missedAppointmentReport = (MissedAppointmentReportResponse)result.Value;
            missedAppointmentReport.Appointments.Should().HaveCount(1);
            missedAppointmentReport.Appointments.Single().AppointmentDateTime.Should()
                .Be(appointmentA.AppointmentDateTime);
        }
    }
}
