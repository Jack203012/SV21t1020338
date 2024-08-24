using Dapper;
using SV21t1020338.DomainModels;

using System.Data;


namespace SV21t1020338.DataLayers.SQLServer
{
    public class SupplierDAL : _BaseDAL, ICommonDAL<Supplier>
    {
        private string connectionString { get; set; } = "";
        public SupplierDAL(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }

        public IList<Supplier> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            var listSuperliers = new List<Supplier>();

            using (var conn = OpenConnection())
            {
                string sql = @"select * from 
                                        (
	                                        select *, ROW_NUMBER() over (order by SupplierName) as RowNumber
	                                        from Suppliers
	                                        where SupplierName like @searchValue or ContactName like @searchValue 
                                        ) as t
                                        where @pageSize = 0
	                                        or (RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                                        order by RowNumber;";
                var paramaters = new { searchValue = $"%{searchValue}%", page = page, pageSize = pageSize };
                listSuperliers = conn.Query<Supplier>(sql, param: paramaters, commandType: CommandType.Text).ToList();
                conn.Close();
            }
            return listSuperliers;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"select count(*) from Suppliers where (SupplierName like @searchValue) or (ContactName like @searchValue);";
                var paramaters = new { searchValue = $"%{searchValue}%" };
                count = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return count;
        }

        public Supplier? Get(int id)
        {
            var supplier = new Supplier();
            using (var conn = OpenConnection())
            {
                string sql = "select * from Suppliers where SupplierID = @SupplierID;";
                var paramaters = new { SupplierID = id };
                supplier = conn.QueryFirstOrDefault<Supplier>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return supplier;
        }

        public int Add(Supplier item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into Suppliers (SupplierName, ContactName, Province, Address, Phone, Email, Photo) 
                                   values (@SupplierName, @ContactName, @Province, @Address, @Phone, @Email, @Photo);
                                   select @@IDENTITY;";
                var paramaters = new
                {
                    SupplierName = item.SupplierName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Photo = item.Photo ??""
                };
                id = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return id;
        }

        public bool Update(Supplier item)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update Suppliers set 
				                                SupplierName = @SupplierName, 
				                                ContactName = @ContactName, 
				                                Province = @Province, 
				                                Address = @Address, 
				                                Phone = @Phone, 
				                                Email = @Email,
                                                Photo = @Photo
                               where SupplierID = @SupplierID;";
                var paramaters = new
                {
                    SupplierID = item.SupplierID,
                    SupplierName = item.SupplierName ?? "",
                    ContactName = item.ContactName ?? "",
                    Province = item.Province ?? "",
                    Address = item.Address ?? "",
                    Phone = item.Phone ?? "",
                    Email = item.Email ?? "",
                    Photo = item.Photo ?? ""
                };
                int count = conn.Execute(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from Suppliers where SupplierID = @SupplierID;";
                int count = conn.Execute(sql, param: new { SupplierID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = "if exists (select * from Products where SupplierID = @SupplierID) select 1 else select 0;";
                int count = conn.ExecuteScalar<int>(sql, param: new { SupplierID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
            }
}
