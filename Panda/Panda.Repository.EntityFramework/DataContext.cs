using Microsoft.EntityFrameworkCore;
using Panda.Model;

namespace Panda.Repository
{
    public class DataContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
