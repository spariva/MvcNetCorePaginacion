using MvcNetCorePaginacion.Models;

namespace MvcNetCorePaginacion.Models
{
    public class DepartamentoDetails
    {
        public Departamento Dept { get; set; }
        public Empleado Emp { get; set; }
        public int NroRegistros { get; set; }
    }
}
