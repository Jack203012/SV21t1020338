using Dapper;
using SV21t1020338.DomainModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21t1020338.DataLayers.SQLServer
{
    public class ShipperDAL : _BaseDAL, ICommonDAL<Shipper>
    {
        private string connectionString { get; set; } = "";
        public ShipperDAL(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }

        public IList<Shipper> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            var listShipper = new List<Shipper>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from 
                                        (
	                                        select *, ROW_NUMBER() over (order by ShipperName) as RowNumber
	                                        from Shippers
	                                        where ShipperName like @searchValue or Phone like @searchValue 
                                        ) as t
                                        where @pageSize = 0
	                                        or (RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                                        order by RowNumber;";
                var paramaters = new { searchValue = $"%{searchValue}%", page = page, pageSize = pageSize };
                listShipper = conn.Query<Shipper>(sql, param: paramaters, commandType: CommandType.Text).ToList();
                conn.Close();
            }
            return listShipper;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var conn = OpenConnection())
            {
                string sql = "select count(*) from Shippers where (ShipperName like @searchValue) or (Phone like @searchValue);";
                var paramaters = new { searchValue = $"%{searchValue}%" };
                count = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return count;
        }

        public Shipper? Get(int id)
        {
            Shipper? shipper = null;
            using (var conn = OpenConnection())
            {
                string sql = "select * from Shippers where ShipperID = @ShipperID;";
                shipper = conn.QueryFirstOrDefault<Shipper>(sql, param: new { ShipperID = id }, commandType: CommandType.Text);
                conn.Close();
            }
            return shipper;
        }

        public int Add(Shipper item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into Shippers (ShipperName, Phone) 
                                           values (@ShipperName, @Phone); 
                                           select @@IDENTITY;";
                var paramaters = new
                {
                    ShipperName = item.ShipperName ?? "",
                    Phone = item.Phone ?? ""
                };
                id = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return id;
        }

        public bool Update(Shipper item)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update Shippers set 
                                               ShipperName = @ShipperName, 
                                               Phone = @Phone
                                               where ShipperID = @ShipperID;";
                var paramaters = new
                {
                    ShipperID = item.ShipperID,
                    ShipperName = item.ShipperName ?? "",
                    Phone = item.Phone ?? ""
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
                string sql = "delete from Shippers where ShipperID = @ShipperID;";
                int count = conn.Execute(sql, param: new { ShipperID = id }, commandType: CommandType.Text);
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
                string sql = "if exists (select * from Orders where ShipperID = @ShipperID) select 1 else select 0;";
                int count = conn.ExecuteScalar<int>(sql, param: new { ShipperID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

    }
}
