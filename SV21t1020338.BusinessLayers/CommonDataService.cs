

using SV21t1020338.DataLayers;
using SV21t1020338.DomainModels;

namespace SV21t1020338.BusinessLayers
{
   public static class CommonDataService
    {

        static readonly ICommonDAL<Province> provinceDb;
        static readonly ICommonDAL<Customer> customerDb;
        static readonly ICommonDAL<Employee> employeeDB;
        static readonly ICommonDAL<Category> categoryDB;
        static readonly ICommonDAL<Shipper> shipperDB;
        static readonly ICommonDAL<Supplier> supplierDB;

        static CommonDataService()
        {
            provinceDb = new DataLayers.SQLServer.ProvinceDAL(Configuration.ConnectionString);
            customerDb = new DataLayers.SQLServer.CustomerDAL(Configuration.ConnectionString);
            employeeDB = new DataLayers.SQLServer.EmployeeDAL(Configuration.ConnectionString);
            categoryDB = new DataLayers.SQLServer.CategoryDAL(Configuration.ConnectionString);
            shipperDB = new DataLayers.SQLServer.ShipperDAL(Configuration.ConnectionString);
            supplierDB = new DataLayers.SQLServer.SupplierDAL(Configuration.ConnectionString);
            
        }

        public static List<Province> GetProvinceList()
        {
            return provinceDb.List().ToList();
        }
        public static List<Customer> GetCustomerList(out int rowCount, int page =1, int pagSize = 0, string searchValue ="")
        {
            rowCount = customerDb.Count(searchValue);
            return customerDb.List(page, pagSize, searchValue).ToList();
        }
        public static List<Customer> GetCustomerList(string searchValue = "")
        {
            return customerDb.List(1,0,searchValue).ToList();
        }


        //lay thong tin cua 1 khach hang dua vao ID
        public static Customer? GetCustomer(int id)
        {
            if (id > 0)
            {
                return customerDb.Get(id);
            }
            return null;
        }

        //bo sung khach hang moi, tra ve ID cua khach hang da duoc bo sung
        public static int AddCustomer(Customer customer)
        {

             return customerDb.Add(customer); 
        }
        //cap nhat thong tin khach hang
        public static bool UpdateCustomer(Customer customer)
        {
            return customerDb.Update(customer);
        }

        //xoa 1 khach hang
        public static bool DeleteCustomer(int id)
        {
            return customerDb.Delete(id);
        }

        //kiem tra 1 khach hang co ID hien co du lieu lien quan hay khong
        public static bool InUseCustomer(int id)
        {
            return customerDb.InUsed(id);
        }

        #region Employee
        /// <summary>
        /// Danh sách nhân viên (tìm kiếm theo tên, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = employeeDB.Count(searchValue);
            return employeeDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Danh sách nhân viên (tìm kiếm theo tên, không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Employee> ListOfEmployees(string searchValue = "")
        {
            return employeeDB.List(1, 0, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một khách hàng dựa theo id-mã khách hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Employee? GetEmployee(int id)
        {
            if (id <= 0)
                return null;
            else
                return employeeDB.Get(id);
        }
        /// <summary>
        /// Thêm một nhân viên mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddEmployee(Employee data)
        {
            return employeeDB.Add(data);
        }
        /// <summary>
        /// Xóa một nhân viên dựa trên mã nhân viên
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteEmployee(int id)
        {
            return employeeDB.Delete(id);
        }
        /// <summary>
        /// Cập nhật một nhân viên dựa trên mã nhân viên
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateEmployee(Employee data)
        {
            return employeeDB.Update(data);
        }
        /// <summary>
        /// kiểm tra mã nhân viên hiện có dữ liệu liên quan không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedEmployee(int id)
        {
            return employeeDB.InUsed(id);
        }
        #endregion

        #region Category
        /// <summary>
        /// Danh sách loại hàng (tìm kiếm theo tên, mô tả, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = categoryDB.Count(searchValue);
            return categoryDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Danh sách loại hàng (tìm kiếm theo tên, mô tả, không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Category> ListOfCategories(string searchValue = "")
        {
            return categoryDB.List(1, 0, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một loại hàng dựa theo id-mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Category? GetCategory(int id)
        {
            if (id <= 0)
                return null;
            else
                return categoryDB.Get(id);
        }
        /// <summary>
        /// Thêm một loại hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddCategory(Category data)
        {
            return categoryDB.Add(data);
        }
        /// <summary>
        /// Xóa một loại hàng dựa trên mã loại hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteCategory(int id)
        {
            return categoryDB.Delete(id);
        }
        /// <summary>
        /// Cập nhật một loại hàng dựa trên mã loại hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateCategory(Category data)
        {
            return categoryDB.Update(data);
        }
        /// <summary> 
        /// kiểm tra mã loại hàng hiện có dữ liệu liên quan không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedCategory(int id)
        {
            return categoryDB.InUsed(id);
        }
        #endregion

        #region Shipper
        /// <summary>
        /// Danh sách người giao hàng (tìm kiếm theo tên, số điện thoại, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = shipperDB.Count(searchValue);
            return shipperDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Danh sách người giao hàng (tìm kiếm theo tên, sđt, không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Shipper> ListOfShippers(string searchValue = "")
        {
            return shipperDB.List(1, 0, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một người giao hàng dựa theo id - mã người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Shipper? GetShipper(int id)
        {
            if (id <= 0)
                return null;
            else
                return shipperDB.Get(id);
        }
        /// <summary>
        /// Thêm một người giao hàng mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddShipper(Shipper data)
        {
            return shipperDB.Add(data);
        }
        /// <summary>
        /// Xóa một người giao hàng dựa trên mã người giao hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteShipper(int id)
        {
            return shipperDB.Delete(id);
        }
        /// <summary>
        /// Cập nhật một người giao hàng dựa trên mã người giao hàng
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateShipper(Shipper data)
        {
            return shipperDB.Update(data);
        }
        /// <summary> 
        /// kiểm tra mã người giao hàng hiện có dữ liệu liên quan không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedShipper(int id)
        {
            return shipperDB.InUsed(id);
        }
        #endregion

        #region Supplier
        /// <summary>
        /// Danh sách nhà cung cấp (tìm kiếm theo tên, phân trang)
        /// </summary>
        /// <param name="rowCount"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(out int rowCount, int page = 1, int pageSize = 0, string searchValue = "")
        {
            rowCount = supplierDB.Count(searchValue);
            return supplierDB.List(page, pageSize, searchValue).ToList();
        }
        /// <summary>
        /// Danh sách nhà cung cấp (tìm kiếm theo tên, sđt, không phân trang)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <returns></returns>
        public static List<Supplier> ListOfSuppliers(string searchValue = "")
        {
            return supplierDB.List(1, 0, searchValue).ToList();
        }
        /// <summary>
        /// Lấy thông tin của một nhà cung cấp dựa theo id - mã nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Supplier? GetSupplier(int id)
        {
            if (id <= 0)
                return null;
            else
                return supplierDB.Get(id);
        }
        /// <summary>
        /// Thêm một nhà cung cấp mới
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static int AddSupplier(Supplier data)
        {
            return supplierDB.Add(data);
        }
        /// <summary>
        /// Xóa một nhà cung cấp dựa trên mã nhà cung cấp
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool DeleteSupplier(int id)
        {
            return supplierDB.Delete(id);
        }
        /// <summary>
        /// Cập nhật một nhà cung cấp dựa trên mã nhà cung cấp
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static bool UpdateSupplier(Supplier data)
        {
            return supplierDB.Update(data);
        }
        /// <summary> 
        /// kiểm tra mã nhà cung cấp hiện có dữ liệu liên quan không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool IsUsedSupplier(int id)
        {
            return shipperDB.InUsed(id);
        }
        #endregion
    }
}
