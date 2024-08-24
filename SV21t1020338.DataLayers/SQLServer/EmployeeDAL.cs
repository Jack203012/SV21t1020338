using Dapper;
using SV21t1020338.DomainModels;
using System.Data;

namespace SV21t1020338.DataLayers.SQLServer
{
    public class EmployeeDAL : _BaseDAL, ICommonDAL<Employee>
    {
        private string connectionString { get; set; } = "";
        public EmployeeDAL(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }

        public int Add(Employee item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into Employees 
                                           (FullName, BirthDate, Address, Phone, Email, Photo, IsWorking) 
                                     values 
                                           (@FullName, @BirthDate, @Address, @Phone, @Email, @Photo, @IsWorking); 
                                     select scope_identity();";
                var paramaters = new
                {
                    FullName = item.FullName ?? "",
                    BirthDate = item.BirthDate,
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Photo = item.Photo ?? "",
                    IsWorking = item.IsWorking
                };
                id = conn.ExecuteScalar<int>(sql, paramaters, commandType: CommandType.Text);
                //id = conn.ExecuteScalar<int>("sp_InsertEmployee", param: paramaters, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return id;
        }

        public int Count(string searchValue = "") //Lấy tổng số đối tượng trong dữ liệu, có tìm kiếm theo tên
        {
            int count = 0;
            using (var conn = OpenConnection())
            {
                string sql = "select count(*) from Employees where FullName like @searchValue";
                var paramaters = new { searchValue = $"%{searchValue}%" };
                count = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                //count = conn.ExecuteScalar<int>("sp_GetEmployeeCount", param: paramaters, commandType: CommandType.StoredProcedure);
            }
            return count;
        }

        public bool Delete(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from Employees where EmployeeID = @EmployeeID";
                int count = conn.Execute(sql, param: new { EmployeeID = id }, commandType: CommandType.Text);
                //int count = conn.Execute("sp_DeleteEmployee", param: new { EmployeeID = id }, commandType: CommandType.StoredProcedure);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public Employee? Get(int id)
        {
            Employee? employee = null;
            using (var conn = OpenConnection())
            {
                string sql = "select * from Employees where EmployeeID = @EmployeeID";
                employee = conn.QueryFirstOrDefault<Employee>(sql, param: new { EmployeeID = id }, commandType: CommandType.Text);
                //employee = conn.QueryFirstOrDefault<Customer>("sp_GetEmployee", param: new { CustomerID = id }, commandType: CommandType.StoredProcedure);
                conn.Close();
            }
            return employee;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"if exists (select * from Orders where EmployeeID = @EmployeeID) select 1
                               else select 0;";
                int count = conn.ExecuteScalar<int>(sql, param: new { EmployeeID = id }, commandType: CommandType.Text);
                //int count = conn.ExecuteScalar<int>("sp_CheckEmployeeUsage", param: new { CustomerID = id }, commandType: CommandType.StoredProcedure);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public IList<Employee> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            var listEmployee = new List<Employee>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from 
                                        (
	                                        select *, ROW_NUMBER() over (order by FullName) as RowNumber
	                                        from Employees
	                                        where FullName like @searchValue
                                        ) as t
                                        where @pageSize = 0
	                                        or (RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                                        order by RowNumber;";

                var paramaters = new { searchValue = $"%{searchValue}%", page = page, pageSize = pageSize };
                listEmployee = conn.Query<Employee>(sql, param: paramaters, commandType: CommandType.Text).ToList();
                //listEmployee = conn.Query<Employee>("sp_GetEmployeesPaging", param: paramaters, commandType: CommandType.StoredProcedure).ToList();
                conn.Close();
            }
            return listEmployee;
        }

        public bool Update(Employee item)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update Employees 
                               set 
                                    FullName = @FullName, 
                                    BirthDate = @BirthDate, 
                                    Address = @Address, 
                                    Phone = @Phone, 
                                    Email = @Email, 
                                    Photo = @Photo, 
                                    IsWorking = @IsWorking 
                              where EmployeeID = @EmployeeID;";
                var paramaters = new
                {
                    EmployeeID = item.EmployeeID,
                    FullName = item.FullName ?? "",
                    BirthDate = item.BirthDate,
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Photo = item.Photo ?? "",
                    IsWorking = item.IsWorking
                };
                int count = conn.Execute(sql, param: paramaters, commandType: CommandType.Text);
                //int count = conn.Execute("sp_UpdateEmployee", param: paramaters, commandType: CommandType.StoredProcedure);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
    }
}
