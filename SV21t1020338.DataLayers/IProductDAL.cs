using SV21t1020338.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21t1020338.DataLayers
{
    public interface IProductDAL
    {

        /// <summary>
        /// tìm kiếm và lấy danh sách hiển thị dưới dạng phân trang
        /// </summary>
        /// <param name="page">trang cần hiển thị</param>
        /// <param name="pageSize">số dòng hiển thị trên mỗi trang</param>
        /// <param name="searchValue">giá trị tìm kiếm trong trang (rỗng = không tìm kiếm)</param>
        /// <param name="categoryId">Mã loại tìm kiếm trong trang (0 nếu không tìm kiếm)</param>
        /// <param name="supplierId">Mã nhà cung cấp cần tìm kiếm (0 nếu không tìm kiếm)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng giá cần tìm</param>
        /// <returns></returns>
        IList<Product> ListProducts(int page = 1, int pageSize = 0, string searchValue = "",
            int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Đếm số lượng mặt hàng tìm kiếm được
        /// </summary>
        /// <param name="searchValue">giá trị tìm kiếm trong trang (rỗng = không tìm kiếm)</param>
        /// <param name="categoryId">Mã loại tìm kiếm trong trang (0 nếu không tìm kiếm)</param>
        /// <param name="supplierId">Mã nhà cung cấp cần tìm kiếm (0 nếu không tìm kiếm)</param>
        /// <param name="minPrice">Mức giá nhỏ nhất trong khoảng giá cần tìm</param>
        /// <param name="maxPrice">Mức giá lớn nhất trong khoảng giá cần tìm</param>
        /// <returns></returns>
        int CountProducts(string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0);
        /// <summary>
        /// Lấy một bảng ghi/dòng dữ liệu dựa trên ID của dữ liệu đó
        /// </summary>
        /// <param name="id">Mã dữ liệu cần lấy</param>
        /// <returns></returns>
        Product? GetProduct(int id);
        /// <summary>
        /// Bổ sung dữ liệu vào cơ sở dữ liệu. Hàm trả về id (mã) của dữ liệu được bổ sung.
        /// </summary>
        /// <param name="item">Dữ liệu cần bổ sung</param>
        /// <returns></returns>
        int AddProduct(Product item);
        /// <summary>
        /// Cập nhật dữ liệu
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool UpdateProduct(Product item);
        /// <summary>
        /// Xoá 1 dòng dữ liệu/bảng ghi với Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteProduct(int id);
        /// <summary>
        /// Kiểm tra xem một dòng dữ liệu có Id hiện đang có dữ liệu liên quan ở các bảng khác hay không
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool InUsed(int id);
        /*****************************************************************/
        /// <summary>
        /// Lấy danh sách ảnh của mặt hàng (sắp xếp theo thứ tự của DisplayOder)
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductPhoto> ListPhotos(int productID);
        /// <summary>
        /// Lấy thông tin 1 ảnh dựa vào ID
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductPhoto? GetPhoto(long productID);
        /// <summary>
        /// Bổ sung 1 ảnh cho mặt hàng (hàm trả về mã ảnh được bổ sung)
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddPhoto(ProductPhoto item);
        /// <summary>
        /// Cập nhật ảnh của mặt hàng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool UpdatePhoto(ProductPhoto item);
        /// <summary>
        /// Xoá ảnh mặt hàng
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeletePhoto(long id);
        /***************************************************************/
        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        IList<ProductAttribute> ListAttributes(int productID);
        /// <summary>
        /// Lấy thông tin của thuộc tính theo mã thuộc tính
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        ProductAttribute? GetAttribute(long productID);
        /// <summary>
        /// bổ sung thuộc tính cho mặt hàng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        long AddAttribute(ProductAttribute item);
        /// <summary>
        /// cập nhật thuộc tính của mặt hàng
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        bool UpdateAttribute(ProductAttribute item);
        /// <summary>
        /// xoá thuộc tính 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteAttribute(long id);


    }
}
