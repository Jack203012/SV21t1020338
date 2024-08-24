using SV21t1020338.DataLayers.SQLServer;
using SV21t1020338.DataLayers;
using SV21t1020338.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21t1020338.BusinessLayers
{
    public   class UserAccountService
    {
        private static readonly IUserAccountDAL employeeAccountDB;

        static UserAccountService()
        {
            employeeAccountDB = new EmployeeAccountDAL(Configuration.ConnectionString);
        }

        public static UserAccount? Authorize(string userName, string password)
        {
            return employeeAccountDB.Authorize(userName, password);
        }

        public static bool ChangePassword(string userName, string oldPassword, string newPassword)
        {
            return employeeAccountDB.ChangePassword(userName, oldPassword, newPassword);
        }
        public static bool ValidatePassword(string  userName, string oldPassword)
        {
            return employeeAccountDB.ValidatePassword(userName, oldPassword);
        }

    }
}
