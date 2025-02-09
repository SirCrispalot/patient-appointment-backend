using Microsoft.Extensions.DependencyInjection;
using Panda.Repository;
using Panda.Repository.EntityFramework;
using Panda.Services;
using Panda.Tests.Builders;
using Panda.WebApi.Controllers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using FluentAssertions;
using Panda.Model;

namespace Panda.Tests.IntegrationTests
{
    public class AppointmentIntegrationTests
    {
        private AppointmentController _appointmentController;
        private ClientAppointmentBuilder _clientAppointmentBuilder;
        private PatientBuilder _patientBuilder;
        private AppointmentBuilder _appointmentBuilder;
        private PandaDbContext _dbContext;

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            services.AddScoped<IPatientRepository, PatientRepository>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddDbContext<PandaDbContext>(opt => opt.UseInMemoryDatabase("PandaDb"));
            services.AddLogging(conf => conf.AddConsole());
            var provider = services.BuildServiceProvider();

            _appointmentController = ActivatorUtilities.CreateInstance<AppointmentController>(provider);
            _clientAppointmentBuilder = new ClientAppointmentBuilder();
            _patientBuilder = new PatientBuilder();
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
        public async Task Given_RepositoryWithPatient_When_CreateAppointment_Should_AddAppointmentToRepository()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            await _appointmentController.Create(clientAppointmentA, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            _dbContext.Appointments.Single().Patient.NhsNumber.Should().Be(patientA.NhsNumber);
        }

        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateAppointment_Should_AddAppointmentToRepositoryWithCorrectData()
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

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            var savedAppointment = _dbContext.Appointments.Single();
            savedAppointment.AppointmentDateTime.Should().Be(clientAppointmentA.AppointmentDateTime);
            savedAppointment.AttendedDateTime.Should().Be(null);
            savedAppointment.CancelledDateTime.Should().Be(null);
            savedAppointment.ClinicianCode.Should().Be(clientAppointmentA.ClinicianCode);
            savedAppointment.DepartmentCode.Should().Be(clientAppointmentA.DepartmentCode);

            savedAppointment.Status.Should().Be(AppointmentStatus.Booked);
        }

        [Test]
        public async Task
            Given_RepositoryWithPatient_When_CreateAndUpdateAppointment_Should_UpdateExistingAppointmentWithCorrectData()
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
            var appointmentId = _dbContext.Appointments.Single().Id;

            var newAppointmentDateTime = new DateTime(2025, 10, 11);
            clientAppointmentA.Id = appointmentId;
            clientAppointmentA.AppointmentDateTime = newAppointmentDateTime;
            
            await _appointmentController.Update(clientAppointmentA, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            var savedAppointment = _dbContext.Appointments.Single();
            savedAppointment.AppointmentDateTime.Should().Be(newAppointmentDateTime);
        }

        [Test]
        public async Task
            Given_RepositoryWithPatient_When_CreateAndCancelAppointment_Should_MarkExistingAppointmentCancelled()
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
            var appointmentId = _dbContext.Appointments.Single().Id;
            await _appointmentController.Cancel(appointmentId, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            var savedAppointment = _dbContext.Appointments.Single();
            savedAppointment.CancelledDateTime.Should().NotBeNull();
            savedAppointment.Status.Should().Be(AppointmentStatus.Cancelled);
        }

        [Test]
        public async Task
            Given_RepositoryWithPatient_When_CreateAndAttendAppointment_Should_MarkExistingAppointmentAttended()
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
            var appointmentId = _dbContext.Appointments.Single().Id;
            await _appointmentController.Attend(appointmentId, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            var savedAppointment = _dbContext.Appointments.Single();
            savedAppointment.AttendedDateTime.Should().NotBeNull();
            savedAppointment.Status.Should().Be(AppointmentStatus.Attended);
        }

        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateSameAppointmentTwice_Should_NotCreateDuplicate()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            await _appointmentController.Create(clientAppointmentA, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            _dbContext.Appointments.Single().Patient.NhsNumber.Should().Be(patientA.NhsNumber);
        }

        [Test]
        public async Task Given_RepositoryWithPatient_When_CreateTwoAppointments_Should_CreateBoth()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            var clientAppointmentB = _clientAppointmentBuilder.CreateClientAppointmentB();
            clientAppointmentB.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentB.PatientId = patientAId;

            // Act
            await _appointmentController.Create(clientAppointmentA, CancellationToken.None);
            await _appointmentController.Create(clientAppointmentB, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(2);
            _dbContext.Appointments.Should().AllSatisfy(appointment => { appointment.Patient.NhsNumber.Should().Be(patientA.NhsNumber); });
        }

        [Test]
        public async Task Given_RepositoryWithPatient_When_UpdateNonExistentAppointment_Should_CreateAppointmentWithCorrectData()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            _dbContext.Patients.Add(patientA);
            _dbContext.SaveChangesAsync(CancellationToken.None);
            var patientAId = _dbContext.Patients.Single().Id;

            var clientAppointmentA = _clientAppointmentBuilder.CreateClientAppointmentA();
            clientAppointmentA.PatientNhsNumber = patientA.NhsNumber;
            clientAppointmentA.PatientId = patientAId;

            // Act
            await _appointmentController.Update(clientAppointmentA, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(1);
            var savedAppointment = _dbContext.Appointments.Single();
            savedAppointment.Patient.NhsNumber.Should().Be(patientA.NhsNumber);
        }

        [Test]
        public async Task Given_RepositoryWithMultipleAppointments_When_CancelNonExistentAppointment_Should_HaveNoEffect()
        {
            // Arrange
            var patientA = _patientBuilder.CreatePatientA();
            var appointmentA = _appointmentBuilder.CreateAppointmentA(patientA);
            var patientB = _patientBuilder.CreatePatientB();
            var appointmentB = _appointmentBuilder.CreateAppointmentA(patientB);

            _dbContext.Patients.Add(patientA);
            _dbContext.Patients.Add(patientB);
            _dbContext.Appointments.Add(appointmentA);
            _dbContext.Appointments.Add(appointmentB);
            
            await _dbContext.SaveChangesAsync(CancellationToken.None);

            // Act
            await _appointmentController.Cancel(999999, CancellationToken.None);

            // Assert
            _dbContext.Appointments.Count().Should().Be(2);
            _dbContext.Appointments.Should().AllSatisfy(appointment => { appointment.CancelledDateTime.Should().BeNull(); });
            _dbContext.Appointments.Should().AllSatisfy(appointment => { appointment.Status.Should().Be(AppointmentStatus.Booked); });
        }
    }
}
