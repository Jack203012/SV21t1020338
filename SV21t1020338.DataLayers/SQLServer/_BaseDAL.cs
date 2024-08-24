using Microsoft.Data.SqlClient;

namespace SV21t1020338.DataLayers.SQLServer
{
    //lớp đóng vai trò là lớp "Cha" cho các lớp cài đặt phép xử lí
    //dữ liệu trên CSDL SQL server


    public abstract class _BaseDAL
    {
        protected string _connectionString = "";
        public _BaseDAL(string connectionString) 
        {
            _connectionString = connectionString;
        }

        // tạo và mở kết nối cơ sở dữ liệu đến sql server
        protected SqlConnection OpenConnection()
        {
            SqlConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }

    }
}
