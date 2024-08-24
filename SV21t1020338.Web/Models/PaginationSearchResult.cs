using SV21t1020338.DomainModels;

namespace SV21t1020338.Web.Models
{
    /// <summary>
    /// lớp cơ sở cho kết quả tìm kiếm hiển thị dữ liệu phân trang
    /// </summary>
    public class PaginationSearchResult
    {
        public int Page { get; set; } = 1;
        public int RowCount { get; set; } = 0;
        public int PageSize { get; set; }
        public string SearchValue { get; set; } = "";
        public int PageCount
        {
            get
            {
                if (PageSize == 0)
                    return 1;
                int n = RowCount / PageSize;
                if (RowCount % PageSize > 0)
                    n++;
                return n;
            }
        }

    }
    /// <summary>
    /// kết quả tìm kiếm khách hàng
    /// </summary>
    public class CustomerSearchResult : PaginationSearchResult
    {
        public required List<Customer> Data { get; set; }

    }
    /// <summary>
    /// kết quả tìm kiếm nhà cung cấp
    /// </summary>
    public class SupplierSearchResult : PaginationSearchResult
    {
        public required List<Supplier> Data { get; set; }

    }
    /// <summary>
    /// kết quả tìm kiếm đơn vị vận chuyển
    /// </summary>
    public class ShipperSearchResult : PaginationSearchResult
    {
        public required List<Shipper> Data { get; set; }

    }
    /// <summary>
    /// kết quả tìm kiếm mặt hàng
    /// </summary>
    public class CategorySearchResult : PaginationSearchResult
    {
        public required List<Category> Data { get; set; }
    }
    /// <summary>
    /// kết quả tìm kiếm nhan vien
    /// </summary>
    public class EmployeeSearchResult : PaginationSearchResult
    {
        public required List<Employee> Data { get; set; }
    }
    public class ProductSearchResult : PaginationSearchResult
    {
        public required List<Product> Data { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int SupplierID { get; set; }
        public int CategoryID { get; set; }


    }
    public class OrderSearchResult : PaginationSearchResult
    {
        public int Status { get; set; } = 0;
        public string TimeRange { get; set; } = "";
        public List<Order> Data { get; set; } = new List<Order>();
    }

}
