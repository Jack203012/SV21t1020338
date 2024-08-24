using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21t1020338.BusinessLayers
{
    public static class Configuration
    {
        public static string ConnectionString { get; set; } = "";
        public static void Initialize(string connectionString)
        {

        ConnectionString = connectionString; 
        }
    }

 }

