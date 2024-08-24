﻿
using Dapper;
using SV21t1020338.DomainModels;
using System.Data;

namespace SV21t1020338.DataLayers.SQLServer
{
    public class CustomerDAL : _BaseDAL, ICommonDAL<Customer>
    {
        public CustomerDAL(string connectionString) : base(connectionString)
        {
        }

        public int Add(Customer data)
        {
            int id = 0;
            using (var connection = OpenConnection())
            {
                var sql = @"insert into Customers(CustomerName, ContactName, Province, Address, Phone, Email, IsLocked)
                            values(@CustomerName, @ContactName, @Province, @Address, @Phone, @Email, @IsLocked);
                            select @@IDENTITY ";
                var parameters = new
                {
                    CustomerName = data.CustomerName ?? "",
                    ContactName = data.ContactName ?? "",
                    Province = data.Province ?? "",
                    Address = data.Address ?? "",
                    Phone = data.Phone ?? "",
                    Email = data.Email ?? "",
                    IsLocked = data.IsLocked
                };
                id = connection.ExecuteScalar<int>(sql:sql, param: parameters, commandType: CommandType.Text);
               
                connection.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var connection = OpenConnection())
            {
                string sql = @"select count(*) from Customers where (CustomerName like @searchValue) or (ContactName like @searchValue)";
                var parameters = new { searchValue = $"%{searchValue}%" };
                count = connection.ExecuteScalar<int>(sql :sql, param :  parameters , commandType : CommandType.Text);
                connection.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"delete from Customers where CustomerId = @CustomerId"; ;
                var parameters = new 
                { 
                    CustomerId = id 
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }

        public Customer? Get(int id)
        {
            Customer? data = null;
            using (var connection = OpenConnection())
            {
                var sql = @"select * from Customers where CustomerId = @CustomerId";
                var parameters = new
                {
                    CustomerId = id
                };
                data = connection.QueryFirstOrDefault<Customer>(sql: sql, param: parameters , commandType: CommandType.Text);
                connection.Close();
            }
            return data;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var connection = OpenConnection())
            {
                var sql = @"IF EXISTS (SELECT * FROM Orders WHERE CustomerId = @CustomerId)
                                SELECT 1
                            ELSE 
                                SELECT 0";
                var parameters = new
                {
                    CustomerId = id
                };
                result = connection.ExecuteScalar<bool>(sql: sql, param: parameters, commandType: CommandType.Text);
            }


            return result;
        }

        public IList<Customer> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            List<Customer> list = new List<Customer>();
            using (var connection = OpenConnection())
            {
                var sql = @"select * from 
                            (
	                            select *, ROW_NUMBER() over (order by CustomerName) as RowNumber
	                            from Customers
	                            where CustomerName like @searchValue or ContactName like @searchValue 
                            ) as t
                            where @pageSize = 0
	                            or (RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                            order by RowNumber;";

                var parameters = new
                {
                    page = page,
                    pageSize = pageSize,
                    searchValue = $"%{searchValue}%",

                };
                list = connection.Query<Customer>(sql: sql, param: parameters, commandType: CommandType.Text).ToList();
                connection.Close();
            }
            return list;
        }
        public bool Update(Customer data)
        {
            bool result = false;

            using (var connection = OpenConnection())
            {
                var sql = @"update Customers 
                                set CustomerName = @CustomerName,
                                    ContactName = @ContactName,
                                    Province = @Province,
                                    Address = @Address,
                                    Phone = @Phone,
                                    Email = @Email,
                                    IsLocked = @IsLocked
                                where CustomerId = @CustomerId";
                var parameters = new 
                { 
                    CustomerId = data.CustomerId,
                    CustomerName = data.CustomerName,
                    ContactName = data.ContactName,
                    Province = data.Province,
                    Address = data.Address,
                    Phone = data.Phone,
                    Email = data.Email,
                    IsLocked = data.IsLocked,
                };
                result = connection.Execute(sql: sql, param: parameters, commandType: CommandType.Text) > 0;
                connection.Close();
            }
            return result;
        }
    }
}