using Microsoft.EntityFrameworkCore;
using Panda.Model;

namespace Panda.Repository
{
    public class PandaDbContext : DbContext
    {
        public PandaDbContext(DbContextOptions<PandaDbContext> options)
            : base(options)
        {
        }

        public DbSet<Patient> Patients { get; set; }

        public DbSet<Appointment> Appointments { get; set; }
    }
}
