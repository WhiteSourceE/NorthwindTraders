using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Northwind.Application.Employees.Commands.DeleteEmployee;
using Northwind.Application.Employees.Commands.UpsertEmployee;
using Northwind.Application.Employees.Queries.GetEmployeeDetail;
using Northwind.Application.Employees.Queries.GetEmployeesList;
using System.Collections.Generic;
using System.Threading.Tasks;
using Northwind.Application.Util;
using Northwind.Domain.Entities;

namespace Northwind.WebUI.Controllers
{
    public class EmployeesController : BaseController
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IList<EmployeeLookupDto>>> GetAll()
        {
            return Ok(await Mediator.Send(new GetEmployeesListQuery()));
        }

        
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<EmployeeDetailVm>> Get(int id)
        {

            return Ok(await Mediator.Send(new GetEmployeeDetailQuery { Id = id }));
        }
        
        [HttpGet("{id}")]
        public string GetSqlUnsafe(string id)
        {
            DbUtil dbUtil = new DbUtil();
            Employee employee = dbUtil.getEmployee(id);
            return employee.FirstName;
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> Upsert(UpsertEmployeeCommand command)
        {
            var id = await Mediator.Send(command);

            return Ok(id);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpsertSqlSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            dbUtil.updateEmployeeSafe(employee);

            return Ok(employee.FirstName + " employee updated");
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpsertSqlUnsafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            dbUtil.updateEmployeeUnsafe(employee);

            return Ok(employee.FirstName + " employee updated");
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await Mediator.Send(new DeleteEmployeeCommand { Id = id });

            return NoContent();
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpsertOdbcSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.updateEmployeeUsingOdbcSafe(employee);

            return Ok(employee.FirstName + " employee updated");
        }

        /// <summary>
        /// Upserts the ODBC unsafe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Updated Employee name with message</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> UpsertOdbcUnsafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.updateEmployeeUsingOdbcUnSafe(employee).ConfigureAwait(false);

            return Ok(employee.FirstName + " employee updated");
        }


        /// <summary>
        /// Upserts the ODBC unsafe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Employee details</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEmployeeDetailsOdbcUnsafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOdbcUnsafe(employee.FirstName).ConfigureAwait(false));
        }


        /// <summary>
        /// Gets the employee details ODBC safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Employee details</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesDefaultResponseType]
        public async Task<IActionResult> GetEmployeeDetailsOdbcSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOdbcSafe(employee.FirstName).ConfigureAwait(false));
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeUsingOracleClientSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.UpdateEmployeeUsingOracleClientSafe(employee);

            return Ok(employee.FirstName + " employee updated");
        }

        /// <summary>
        /// Upserts the ODBC unsafe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Updated Employee name with message</returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeUsingOracleClientUnSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.UpdateEmployeeUsingOracleClientUnSafe(employee).ConfigureAwait(false);

            return Ok(employee.FirstName + " employee updated");
        }

        /// <summary>
        /// Gets the employee details ODBC safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Employee details</returns>
        [HttpPost]
        public async Task<IActionResult> GetEmployeeDetailsOracleClientSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOracleClientSafe(employee.FirstName).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets the employee details ODBC safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns>Employee details</returns>
        [HttpPost]
        public async Task<IActionResult> GetEmployeeDetailsOracleClientUnsafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOracleClientUnsafe(employee.FirstName).ConfigureAwait(false));
        }

        /// <summary>
        /// Updates the employee using OLE database safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeUsingOleDbSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.UpdateEmployeeUsingOleDbSafe(employee);

            return Ok(employee.FirstName + " employee updated");
        }

        /// <summary>
        /// Upserts the OLE database unsafe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateEmployeeUsingOleDbUnSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            await dbUtil.UpdateEmployeeUsingOleDbUnSafe(employee).ConfigureAwait(false);

            return Ok(employee.FirstName + " employee updated");
        }

        /// <summary>
        /// Gets the employee details OLE database unsafe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetEmployeeDetailsOleDbUnsafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOleDbUnsafe(employee.FirstName).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets the employee details OLE database safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetEmployeeDetailsOleDbSafe(Employee employee)
        {
            DbUtil dbUtil = new DbUtil();
            return Ok(await dbUtil.GetEmployeeDetailsOleDbSafe(employee.FirstName).ConfigureAwait(false));
        }
    }
}
