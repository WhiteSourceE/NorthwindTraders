using System;
using System.IO;
using System.Linq;
using Northwind.Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Northwind.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Northwind.Persistence;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Data.Odbc;
using System.Threading.Tasks;
using System.Data.OleDb;

namespace Northwind.Application.Util
{
    public class DbUtil
    {
        string connectionString = null;
        private readonly string odbcConnectionString, oracleClientConnectionString, oleDbConnectionString;
        public DbUtil()
        {
            connectionString = "Server=.;Database=NorthwindTraders;Trusted_Connection=True;MultipleActiveResultSets=true;Application Name=NorthwindTraders;";
            oracleClientConnectionString = "User Id=ADMIN;Password=P@ssw0rd121!; Data Source=wsoracledb_high;";
            odbcConnectionString = "DSN=NorthwindTraders";
            oleDbConnectionString = "Provider=SQLOLEDB;Data Source=.;Initial Catalog=NorthwindTraders;Integrated Security=SSPI";
        }

        public Employee getEmployee(string Id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {

                var employeeList = connection.Query<Employee>("SELECT * FROM Employees Where EmployeeId = " + Id).ToList();

                foreach (var employee in employeeList)
                {
                    return employee;
                }
            }

            return null;
        }


        public void updateEmployeeSafe(Employee employee)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {


                    command.CommandText = "UPDATE Employees " +
                                          "SET FirstName = @FirstName, LastName = @LastName " +
                                          "WHERE EmployeeID = @EmployeeID";

                    Console.WriteLine("id: " + employee.EmployeeId + " name: " + employee.FirstName);
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);
                    command.Parameters.AddWithValue("@LastName", employee.LastName);

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        public void updateEmployeeUnsafe(Employee employee)
        {

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = connection.CreateCommand())
                {


                    command.CommandText = "UPDATE Employees " +
                                          "SET FirstName = @FirstName, LastName = '" + employee.LastName + "' " +
                                          "WHERE EmployeeID = @EmployeeID";

                    Console.WriteLine("id: " + employee.EmployeeId + " name: " + employee.FirstName);
                    command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);
                    command.Parameters.AddWithValue("@FirstName", employee.FirstName);

                    connection.Open();

                    command.ExecuteNonQuery();

                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Updates the employee using ODBC safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task updateEmployeeUsingOdbcSafe(Employee employee)
        {
            using OdbcConnection connection = new OdbcConnection(odbcConnectionString);
            using OdbcCommand command = connection.CreateCommand();

            // Odbc interface does not recognise the use of @named variables, position is all that is used.
            command.CommandText = "UPDATE Employees " +
                                  "SET FirstName = ?, LastName = ? " +
                                  "WHERE EmployeeID = ?";

            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@LastName", employee.LastName);
            command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Updates the employee using ODBC un safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task updateEmployeeUsingOdbcUnSafe(Employee employee)
        {

            using OdbcConnection connection = new OdbcConnection(odbcConnectionString);
            using OdbcCommand command = connection.CreateCommand();

            // Odbc interface does not recognise the use of @named variables, position is all that is used.
            command.CommandText = "UPDATE Employees " +
                                  "SET FirstName = ?, LastName = '" + employee.LastName + "' " +
                                  "WHERE EmployeeID = ?";

            command.Parameters.AddWithValue("@FirstName", employee.FirstName);
            command.Parameters.AddWithValue("@EmployeeID", employee.EmployeeId);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Gets the employee details ODBC unsafe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Employee information</returns>
        public async Task<Employee> GetEmployeeDetailsOdbcUnsafe(string firstName)
        {
            // if a person passes the firstname as  { "FirstName": "test';update Employees set Region=NULL --" } , it will update Region to null value.
            using OdbcConnection connection = new OdbcConnection(odbcConnectionString);
            using OdbcCommand command = connection.CreateCommand();

            return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE FirstName = '" + firstName + "'");
        }

        /// <summary>
        /// Gets the employee details ODBC safe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns>Employee information</returns>
        public async Task<Employee> GetEmployeeDetailsOdbcSafe(string firstName)
        {
            using OdbcConnection connection = new OdbcConnection(odbcConnectionString);
            using OdbcCommand command = connection.CreateCommand();

            return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE FirstName = ?", new {
                FirstName = firstName
            }, null, null, CommandType.Text);
        }

        /// <summary>
        /// Updates the employee using oracle client safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task UpdateEmployeeUsingOracleClientSafe(Employee employee)
        {
            using (Oracle.ManagedDataAccess.Client.OracleConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(oracleClientConnectionString))
            {
                using (Oracle.ManagedDataAccess.Client.OracleCommand command = connection.CreateCommand())
                {
                    if (string.IsNullOrEmpty(Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin))
                    {
                        Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin = @"C:\wallet_WSORACLEDB";
                        Oracle.ManagedDataAccess.Client.OracleConfiguration.WalletLocation = @"C:\wallet_WSORACLEDB";
                    }

                    command.CommandText = "UPDATE Employees " +
                                          "SET FirstName = :firstName, LastName = :lastName " +
                                          "WHERE EmployeeID = :employeeId";

                    command.Parameters.Add("firstName", employee.FirstName);
                    command.Parameters.Add("lastName", employee.LastName);
                    command.Parameters.Add("employeeID", employee.EmployeeId);
                    connection.Open();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Updates the employee using oracle client un safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task UpdateEmployeeUsingOracleClientUnSafe(Employee employee)
        {
            using (Oracle.ManagedDataAccess.Client.OracleConnection connection = new Oracle.ManagedDataAccess.Client.OracleConnection(oracleClientConnectionString))
            {
                using (Oracle.ManagedDataAccess.Client.OracleCommand command = connection.CreateCommand())
                {
                    if (string.IsNullOrEmpty(Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin))
                    {
                        Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin = @"C:\wallet_WSORACLEDB";
                        Oracle.ManagedDataAccess.Client.OracleConfiguration.WalletLocation = @"C:\wallet_WSORACLEDB";
                    }

                    command.CommandText = "UPDATE Employees " +
                                          "SET FirstName = :firstName WHERE LastName = '" + employee.LastName + "'";

                    command.Parameters.Add("firstName", employee.FirstName);
                    connection.Open();

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        /// <summary>
        /// Gets the employee details oracle client unsafe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns></returns>
        public async Task<Employee> GetEmployeeDetailsOracleClientUnsafe(string firstName)
        {
            // if a person passes the firstname as { "FirstName": "sdf' OR '1' = '1"} in api post data
            using var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(oracleClientConnectionString);
            if (string.IsNullOrEmpty(Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin))
            {
                Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin = @"C:\wallet_WSORACLEDB";
                Oracle.ManagedDataAccess.Client.OracleConfiguration.WalletLocation = @"C:\wallet_WSORACLEDB";
            }
            return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE FirstName = '" + firstName + "'");
        }

        /// <summary>
        /// Gets the employee details oracle client safe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns></returns>
        public async Task<Employee> GetEmployeeDetailsOracleClientSafe(string firstName)
        {
            using (var connection = new Oracle.ManagedDataAccess.Client.OracleConnection(oracleClientConnectionString))
            {
                if (string.IsNullOrEmpty(Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin))
                {
                    Oracle.ManagedDataAccess.Client.OracleConfiguration.TnsAdmin = @"C:\wallet_WSORACLEDB";
                    Oracle.ManagedDataAccess.Client.OracleConfiguration.WalletLocation = @"C:\wallet_WSORACLEDB";
                }
                var parameters = new Dapper.Oracle.OracleDynamicParameters();
                parameters.Add("firstName", firstName, Dapper.Oracle.OracleMappingType.Varchar2, ParameterDirection.Input);

                return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM Employees WHERE FirstName = :firstName", parameters, null, null, CommandType.Text);
            }
        }

        /// <summary>
        /// Updates the employee using OLE database safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task UpdateEmployeeUsingOleDbSafe(Employee employee)
        {
            using OleDbConnection connection = new OleDbConnection(oleDbConnectionString);
            using OleDbCommand command = connection.CreateCommand();

            // Odbc interface does not recognise the use of @named variables, position is all that is used.
            command.CommandText = "UPDATE dbo.Employees " +
                                  "SET FirstName = ?, LastName = ? " +
                                  "WHERE EmployeeID = ?";
            
            command.Parameters.Clear();
            command.Parameters.AddWithValue("FirstName", employee.FirstName);
            command.Parameters.AddWithValue("LastName", employee.LastName);
            command.Parameters.AddWithValue("EmployeeID", employee.EmployeeId);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Updates the employee using OLE database un safe.
        /// </summary>
        /// <param name="employee">The employee.</param>
        public async Task UpdateEmployeeUsingOleDbUnSafe(Employee employee)
        {
            //"FirstName": "Fake", "LastName" : "Last' OR '1'='1" 
            using OleDbConnection connection = new OleDbConnection(oleDbConnectionString);
            using OleDbCommand command = connection.CreateCommand();

            // oleDbCommand interface does not recognise the use of @named variables, position is all that is used.
            command.CommandText = "UPDATE dbo.Employees " +
                                  "SET FirstName = ? WHERE LastName = '" + employee.LastName + "' ";

            command.Parameters.Clear();
            command.Parameters.AddWithValue("FirstName", employee.FirstName);

            connection.Open();
            await command.ExecuteNonQueryAsync();
            connection.Close();
        }

        /// <summary>
        /// Gets the employee details OLE database unsafe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns></returns>
        public async Task<Employee> GetEmployeeDetailsOleDbUnsafe(string firstName)
        {
            // if a person passes the firstname as  { "FirstName": "test';update Employees set Region=NULL --" } , it will update Region to null value.
            using OleDbConnection connection = new OleDbConnection(oleDbConnectionString);
            using OleDbCommand command = connection.CreateCommand();

            return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM dbo.Employees WHERE FirstName = '" + firstName + "'");
        }

        /// <summary>
        /// Gets the employee details OLE database safe.
        /// </summary>
        /// <param name="firstName">The first name.</param>
        /// <returns></returns>
        public async Task<Employee> GetEmployeeDetailsOleDbSafe(string firstName)
        {
            using OleDbConnection connection = new OleDbConnection(oleDbConnectionString);
            using OleDbCommand command = connection.CreateCommand();

            return await connection.QueryFirstOrDefaultAsync<Employee>("SELECT * FROM dbo.Employees WHERE FirstName = ?FirstName?", new
            {
                FirstName = firstName
            }, null, null, CommandType.Text);
        }
    }
}
