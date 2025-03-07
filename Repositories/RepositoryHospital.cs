using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using Microsoft.Win32;
using MvcNetCorePaginacion.Data;
using MvcNetCorePaginacion.Models;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace MvcNetCorePaginacion.Repositories
{
    #region views and procedures
    //    select * from
    //(SELECT ROW_NUMBER() over (order by DEPT_NO) as POSICION,
    //DEPT_NO, DNOMBRE, LOC from DEPT) QUERY
    //where QUERY.POSICION=1

    //create view V_DEPARTAMENTOS_INDIVIDUAL
    //as
    //	cast(SELECT ROW_NUMBER() over(order by DEPT_NO) as int) as POSICION,
    //DEPT_NO, DNOMBRE, LOC from DEPT
    //go

    //    alter procedure SP_GRUPO_DEPTS
    //(@pos int)
    //as
    //	select DEPT_NO, DNOMBRE, LOC from V_DEPARTAMENTOS_INDIVIDUAL

    //    where posicion >= @pos and posicion<(@pos + 2)
    //go


    //    Create view V_GRUPO_EMPLEADOS
    //AS
    //SELECT CAST(ROW_NUMBER() OVER (ORDER BY APELLIDO) AS INT) AS POSICION,
    //    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
    //GO

    //CREATE PROCEDURE SP_GRUPO_EMPS(@pos int)
    //as
    //	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM V_GRUPO_EMPLEADOS

    //    WHERE POSICION >= @pos AND POSICION<(@pos + 3)
    //GO



    /*Si no usas lo de ado net en vez de el sql raw en el repo, no puedes coger más que lo que mapeas del context.*/
    //    CREATE PROCEDURE SP_GRUPO_EMPS_OFICIO(@pos int, @jump int, @oficio nvarchar(50))
    //as
    //SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
    //(SELECT ROW_NUMBER() OVER (ORDER BY APELLIDO) AS POSICION,
    //    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP

    //    WHERE OFICIO = @oficio) QUERY
    //    WHERE POSICION >= @pos AND POSICION<(@pos + @jump)
    //GO


    /*almacenamos el número de registros del filtro, podrían ser varios.*/
    //    CREATE PROCEDURE SP_GRUPO_EMPS_OUT(@pos int, @jump int, @oficio nvarchar(50), @registros int out)
    //as
    //select @registros = count(EMP_NO) from EMP
    //SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
    //(SELECT ROW_NUMBER() OVER (ORDER BY APELLIDO) AS POSICION,
    //    EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP

    //    WHERE OFICIO = @oficio) QUERY
    //    WHERE POSICION >= @pos AND POSICION<(@pos + @jump)
    //GO



    //practica
    //create procedure SP_EMPLEADOS_DEPARTAMENTO_PAGINATION
    //(@deptno int, @pos int, @nroRegistros int out)
    //as
    // select @nroRegistros = cast(COUNT(EMP_NO) as int)
    //                           from EMP
    //                           where DEPT_NO = @deptno


    //    select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
    //    from(
    //        select cast(row_number() over (order by APELLIDO) as int) as POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO
    //        from EMP
    //        where DEPT_NO = @deptno
    // ) QUERY
    //    where POSICION >= @pos and POSICION<(@pos + 1)
    //go
    #endregion

    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepartamentosAsync()
        {
            return await this.context.VistaDepartamento.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento vd = await this.context.VistaDepartamento.Where(x => x.Posicion == posicion).FirstOrDefaultAsync();
            return vd;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var departamentos = await this.context.Departamento.ToListAsync();
            return departamentos;
        }

        public async Task<List<Empleado>> GetEmpleadosAsync(int idDepartamento)
        {
            var empleados = this.context.Empleados.Where(x => x.IdDepartamento == idDepartamento);

            if (empleados.Count() == 0) {
                return null;
            }

            return await empleados.ToListAsync();
        }

        public async Task<List<VistaDepartamento>> GetGrupoVistaDepartamentoAsync(int pos)
        {
            //VistaDepartamento vd = await this.context.VistaDepartamento.Where(x => x.Posicion == posicion && x.Posicion == posicion + 2).FirstOrDefaultAsync();
            List<VistaDepartamento> vd = await this.context.VistaDepartamento
                                  .Where(x => x.Posicion >= pos && x.Posicion < (pos + 2))
                                  .ToListAsync();
            return vd;
        }

        public async Task<List<Departamento>> GetGrupoDepatamentosAsync(int pos) { 
            string sql = "SP_GRUPO_DEPTS @pos";
            SqlParameter param = new SqlParameter("@pos", pos);
            var consulta = this.context.Departamento.FromSqlRaw(sql, param);

            return await consulta.ToListAsync();
        }


        //empleados



        public async Task<int> GetEmpleadosCountAsync()
        {
            return await this.context.Empleados.CountAsync();
        }


        public async Task<List<Empleado>> GetGrupoEmpleadosAsync(int pos)
        {
            string sql = "SP_GRUPO_EMPS @pos";
            SqlParameter param = new SqlParameter("@pos", pos);
            var consulta = this.context.Empleados.FromSqlRaw(sql, param);

            return await consulta.ToListAsync();
        }



        public async Task<int> GetEmpleadosOficioCountAsync(string oficio)
        {
            return await this.context.Empleados.Where(x => x.Oficio == oficio).CountAsync();
        }

        public async Task<List<Empleado>> GetEmpleadosOficioAsync(int posicion, int jump, string oficio)
        {
            string sql = "SP_GRUPO_EMPS_OFICIO @pos, @jump, @oficio";
            SqlParameter paramPos = new SqlParameter("@pos", posicion);
            SqlParameter paramJump = new SqlParameter("@jump", jump);
            SqlParameter paramOficio = new SqlParameter("@oficio", oficio);
            var consulta = this.context.Empleados.FromSqlRaw(sql, paramPos, paramJump, paramOficio);

            return await consulta.ToListAsync();
        }


        //ahora con params de salida

        public async Task<ModelEmpleadosOficio> GetEmpleadosOficioOutAsync(int pos, int jump, string oficio)
        {
            string sql = "SP_GRUPO_EMPS_OUT @pos, @jump, @oficio, @registros out";
            SqlParameter paramPos = new SqlParameter("@pos", pos);
            SqlParameter paramJump = new SqlParameter("@jump", jump);
            SqlParameter paramOficio = new SqlParameter("@oficio", oficio);
            SqlParameter paramRegistros = new SqlParameter("@registros", 0);
            paramRegistros.Direction = System.Data.ParameterDirection.Output;
            var consulta = 
                this.context.Empleados.FromSqlRaw(sql, paramPos, paramJump, paramOficio, paramRegistros);
            //ejecutamos la consulta y ya luego podemos recuperar los params de salid
            List<Empleado> empleados = await consulta.ToListAsync();

            int registros = int.Parse(paramRegistros.Value.ToString());
            ModelEmpleadosOficio modelempleados = new ModelEmpleadosOficio()
            {
                NumeroRegistros = registros,
                Empleados = empleados
            };

            return modelempleados;

        }


        }
}
