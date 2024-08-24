using Dapper;
using SV21t1020338.DomainModels;
using System.Data;

namespace SV21t1020338.DataLayers.SQLServer
{

    public class EmployeeAccountDAL: _BaseDAL, IUserAccountDAL
    {
        public EmployeeAccountDAL(string connectionString) : base(connectionString)
        {
        }

        public UserAccount? Authorize(string userName, string password)
        {
            UserAccount? data = null;
            using (var cn = OpenConnection())
            {
                var sql = @"select EmployeeID as UserID, Email as UserName, FullName, Email, Photo, Password, RoleNames
                           from Employees where Email=@Email AND Password=@Password";
                var parameters = new
                {
                    Email = userName,
                    Password = password,
                };
                data = cn.QuerySingleOrDefault<UserAccount>(sql, parameters);
                cn.Close();
            }
            return data;
        }

        public bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = @"update Employees 
                            set Password=@NewPassword 
                            where Email=@Email and Password=@OldPassword";
                var parameters = new
                {
                    Email = userName,
                    OldPassword = oldPassword,
                    NewPassword = newPassword
                };
                result = cn.Execute(sql, parameters) > 0;
                cn.Close();
            }
            return result;
        }

        public bool ValidatePassword(string userName, string password)
        {
            bool result = false;
            using (var cn = OpenConnection())
            {
                var sql = "SELECT Password FROM Employees WHERE Email=@Email";
                var param = new
                {
                    Email = userName
                };
                var oldpass = cn.QueryFirstOrDefault<string>(sql: sql, param: param, commandType: CommandType.Text);
                result = oldpass != null && oldpass.Equals(password);

            }
            return result;
        }
    }
}

