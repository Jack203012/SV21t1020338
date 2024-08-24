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
    public class CategoryDAL : _BaseDAL, ICommonDAL<Category>
    {
        private string connectionString { get; set; } = "";
        public CategoryDAL(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }

        public int Add(Category item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into Categories 
                                            (CategoryName, Description) 
                                    values 
                                            (@CategoryName, @Description); 
                                    select @@IDENTITY";
                var paramaters = new
                {
                    CategoryName = item.CategoryName ?? "",
                    Description = item.Description ?? ""
                };
                id = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return id;
        }

        public int Count(string searchValue = "")
        {
            int count = 0;
            using (var conn = OpenConnection())
            {
                string sql = "select count(*) from Categories where (CategoryName like @searchValue) or (Description like @searchValue)";
                var paramaters = new { searchValue = $"%{searchValue}%" };
                count = conn.ExecuteScalar<int>(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
            }
            return count;
        }

        public bool Delete(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from Categories where CategoryID = @CategoryID";
                int count = conn.Execute(sql, param: new { CategoryID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public Category? Get(int id)
        {
            Category? category = null;
            using (var conn = OpenConnection())
            {
                string sql = "select * from Categories where CategoryID = @CategoryID";
                category = conn.QueryFirstOrDefault<Category>(sql, param: new { CategoryID = id }, commandType: CommandType.Text);
                conn.Close();
            }
            return category;
        }

        public bool InUsed(int id)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = "if exists (select * from Products where CategoryID = @CategoryID) select 1 else select 0";
                int count = conn.ExecuteScalar<int>(sql, param: new { CategoryID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public IList<Category> List(int page = 1, int pageSize = 0, string searchValue = "")
        {
            var listCategory = new List<Category>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from 
                                        (
	                                        select *, ROW_NUMBER() over (order by CategoryName) as RowNumber
	                                        from Categories
	                                        where CategoryName like @searchValue or Description like @searchValue 
                                        ) as t
                                where @pageSize = 0
	                                or (RowNumber between (@page -1) * @pageSize +1 and @page * @pageSize)
                                order by RowNumber;";
                var paramaters = new { searchValue = $"%{searchValue}%", page = page, pageSize = pageSize };
                listCategory = conn.Query<Category>(sql, param: paramaters, commandType: CommandType.Text).ToList();
                conn.Close();
            }
            return listCategory;
        }

        public bool Update(Category item)
        {
            bool result = false;
            using (var conn = OpenConnection())
            {
                string sql = "update Categories set CategoryName = @CategoryName, Description = @Description where CategoryID = @CategoryID";
                var paramaters = new
                {
                    CategoryID = item.CategoryID,
                    CategoryName = item.CategoryName ?? "",
                    Description = item.Description ?? ""
                };
                int count = conn.Execute(sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
    }
}
