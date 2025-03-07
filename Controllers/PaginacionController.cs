using Microsoft.AspNetCore.Mvc;
using MvcNetCorePaginacion.Repositories;
using MvcNetCorePaginacion.Models;

namespace MvcNetCorePaginacion.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> PaginarRegistroVDept(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

            int next = posicion.Value + 1;

            if(next > numRegistros)
            {
                next = 1;
            }

            int prev = posicion.Value - 1;

            if (prev < 1)
            {
                prev = numRegistros;
            }

            ViewBag.NumRegistros = numRegistros;
            ViewBag.Next = next;
            ViewBag.Prev = prev;

            VistaDepartamento vd = await this.repo.GetVistaDepartamentoAsync(posicion.Value);

            return View(vd);
        }

        //public async Task<IActionResult> PaginarRegistroGrupoVDept(int? posicion)
        //{

        //    if (posicion == null)
        //    {
        //        posicion = 1;
        //    }

        //    int numPag = 1;
        //    int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();

        //    string html = "<div>";
        //    for (int i = 1; i <= numRegistros; i += 2)
        //    {
        //        html += "<a class='btn btn-outline-primary' href='/Paginacion/PaginarRegistroGrupoVDept?posicion=" + i + "'> Página: " + numPag + "</a> | ";
        //        numPag++;
        //    }
        //    html += "</div>";

        //    ViewBag.Links = html;
        //    List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);

        //    return View(departamentos);
        //}


        public async Task<IActionResult> PaginarRegistroGrupoVDept(int? posicion)
        {

            if (posicion == null)
            {
                posicion = 1;
            }

            int numPag = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            ViewBag.Numregistros = numRegistros;

            List<VistaDepartamento> departamentos = await this.repo.GetGrupoVistaDepartamentoAsync(posicion.Value);

            return View(departamentos);
        }

        public async Task<IActionResult> PaginarGrupoDept(int? posicion)
        {

            if (posicion == null)
            {
                posicion = 1;
            }

            int numPag = 1;
            int numRegistros = await this.repo.GetNumeroRegistrosVistaDepartamentosAsync();
            ViewBag.Numregistros = numRegistros;

            List<Departamento> departamentos = await this.repo.GetGrupoDepatamentosAsync(posicion.Value);


            return View(departamentos);
        }

        //Empleados

        public async Task<IActionResult> PaginarGrupoEmp(int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }
            int numPag = 1;
            int numRegistros = await this.repo.GetEmpleadosCountAsync();

            ViewBag.Numregistros = numRegistros;
            List<Empleado> empleados = await this.repo.GetGrupoEmpleadosAsync(posicion.Value);
            return View(empleados);
        }

        public async Task<IActionResult> EmpleadosOficio (int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }

            List<Empleado> empleados = await this.repo.GetEmpleadosOficioAsync(posicion.Value, 3, oficio);

            int numRegistros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewBag.Numregistros = numRegistros;
            ViewBag.Oficio = oficio;

            return View(empleados);

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficio(string oficio)
        {

            List<Empleado> empleados = await this.repo.GetEmpleadosOficioAsync(1, 3, oficio);

            int numRegistros = await this.repo.GetEmpleadosOficioCountAsync(oficio);
            ViewBag.Numregistros = numRegistros;
            ViewBag.Oficio = oficio;

            return View(empleados);
        }


        public async Task<IActionResult> EmpleadosOficioOut(int? posicion, string oficio)
        {
            if (posicion == null)
            {
                posicion = 1;
                return View();
            }

            ModelEmpleadosOficio empleados = await this.repo.GetEmpleadosOficioOutAsync(posicion.Value, 3, oficio);

            ViewBag.Numregistros = empleados.NumeroRegistros;
            ViewBag.Oficio = oficio;

            return View(empleados);

        }

        [HttpPost]
        public async Task<IActionResult> EmpleadosOficioOut(string oficio)
        {

            ModelEmpleadosOficio empleados = await this.repo.GetEmpleadosOficioOutAsync(1, 3, oficio);

            ViewBag.Numregistros = empleados.NumeroRegistros;
            ViewBag.Oficio = oficio;

            return View(empleados);
        }
    }
}
