using Microsoft.EntityFrameworkCore;
using MvcNetCorePaginacion.Models;

namespace MvcNetCorePaginacion.Data
{
    public class HospitalContext: DbContext
    {
        public HospitalContext(DbContextOptions<HospitalContext> options) : base(options)
        {
        }

        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Departamento> Departamento  { get; set; }
        public DbSet<VistaDepartamento> VistaDepartamento { get; set; }
    }
}
