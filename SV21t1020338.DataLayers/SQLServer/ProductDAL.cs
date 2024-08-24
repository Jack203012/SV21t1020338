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
    public class ProductDAL : _BaseDAL, IProductDAL
    {
        private string connectionString { get; set; } = "";
        public ProductDAL(string connectionString) : base(connectionString)
        {
            this.connectionString = connectionString;
        }
        #region Thêm Mới
        public int AddProduct(Product item)
        {
            int id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into Products(ProductName, ProductDescription, SupplierID, CategoryID, Unit, Price, Photo, IsSelling)
                                values (@ProductName, @ProductDescription, @SupplierID, @CategoryID, @Unit, @Price, @Photo, @IsSelling);
                                select @@Identity;";
                var param = new
                {
                    ProductName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    SupplierID = item.SupplierID,
                    CategoryID = item.CategoryID,
                    Unit = item.Unit,
                    Price = item.Price,
                    Photo = item.Photo,
                    IsSelling = item.IsSelling
                };
                id = conn.ExecuteScalar<int>(sql: sql, param: param, commandType: CommandType.Text);
            }
            return id;
        }
        public long AddAttribute(ProductAttribute item)
        {
            long id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into ProductAttributes(ProductID, AttributeName, AttributeValue, DisplayOrder)
                                values (@ProductID, @AttributeName, @AttributeValue, @DisplayOrder);
                                select @@Identity;";
                var param = new
                {
                    ProductID = item.ProductID,
                    AttributeName = item.AttributeName,
                    AttributeValue = item.AttributeValue,
                    DisplayOrder = item.DisplayOrder
                };
                id = conn.ExecuteScalar<long>(sql: sql, param: param, commandType: CommandType.Text);
            }
            return id;
        }
        public long AddPhoto(ProductPhoto item)
        {
            long id = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"insert into ProductPhotos(ProductID, Photo, Description, DisplayOrder, IsHidden)
                                values (@ProductID, @Photo, @Description, @DisplayOrder, @IsHidden);
                                select @@Identity;";
                var param = new
                {
                    ProductID = item.ProductID,
                    Photo = item.Photo,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    IsHidden = item.IsHidden
                };
                id = conn.ExecuteScalar<long>(sql: sql, param: param, commandType: CommandType.Text);
            }
            return id;
        }
        #endregion

        #region Các chức năng khác
        /// <summary>
        /// Bảng product được sử dụng bởi 3 bảng là OrderDetails, ProductAttributes và ProductPhotos
        /// </summary>
        /// <param name="id">ProductID</param>
        /// <returns></returns>
        public bool InUsed(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"IF EXISTS (
                                        SELECT * 
                                        FROM OrderDetails 
                                        WHERE ProductID = @ProductID
                                    ) OR EXISTS (
                                        SELECT * 
                                        FROM ProductPhotos 
                                        WHERE ProductID = @ProductID
                                    ) OR EXISTS (
                                        SELECT * 
                                        FROM ProductAttributes 
                                        WHERE ProductID = @ProductID
                                    )
                                        SELECT 1
                                    ELSE
                                        SELECT 0;";
                var param = new { ProductID = id };
                int count = conn.ExecuteScalar<int>(sql, param, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        /// <summary>
        /// Lấy và đếm tất cả có bao nhiêu dòng dữ liệu rồi trả về là int (tổng số dữ liệu)
        /// </summary>
        /// <param name="searchValue"></param>
        /// <param name="categoryId"></param>
        /// <param name="supplierId"></param>
        /// <param name="minPrice"></param>
        /// <param name="maxPrice"></param>
        /// <returns></returns>
        public int CountProducts(string searchValue = "", int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var count = 0;
            using (var conn = OpenConnection())
            {
                string sql = @"select count(*) from Products where (@searchValue = N'' or ProductName like @searchValue)
                                and (@categoryId = 0 or CategoryId = @categoryId)
                                and (@supplierId = 0 or SupplierId = @supplierId)
                                and (Price >= @minPrice)
                                and (@maxPrice <= 0 or Price <= @maxPrice)";
                count = conn.ExecuteScalar<int>(sql, param: new
                {
                    searchValue = $"%{searchValue}%",
                    categoryId = categoryId,
                    supplierId = supplierId,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                }, commandType: CommandType.Text);
            }
            return count;
        }
        #endregion

        #region Xoá
        public bool DeleteProduct(int id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from Products where ProductID = @ProductID;";
                int count = conn.Execute(sql, param: new { ProductID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        public bool DeleteAttribute(long id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from ProductAttributes where AttributeID = @AttributeID;";
                int count = conn.Execute(sql, param: new { AttributeID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        public bool DeletePhoto(long id)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = "delete from ProductPhotos where PhotoID = @PhotoID;";
                int count = conn.Execute(sql, param: new { PhotoID = id }, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        #endregion

        #region Lấy 1 dữ liệu theo id 
        public Product? GetProduct(int id)
        {
            var product = new Product();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from Products where ProductID = @productID";
                product = conn.QueryFirstOrDefault<Product>(sql, new { productID = id }, commandType: CommandType.Text);
            }
            return product;
        }
        public ProductAttribute? GetAttribute(long attributeID)
        {
            var attribute = new ProductAttribute();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductAttributes where AttributeID = @attributeID";
                attribute = conn.QueryFirstOrDefault<ProductAttribute>(sql, new { attributeID = attributeID }, commandType: CommandType.Text);
            }
            return attribute;
        }
        public ProductPhoto? GetPhoto(long photoID)
        {
            var photo = new ProductPhoto();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductPhotos where PhotoID = @photoID";
                photo = conn.QueryFirstOrDefault<ProductPhoto>(sql, new { photoID = photoID }, commandType: CommandType.Text);
            }
            return photo;
        }
        #endregion

        #region Lấy list các dữ liệu

        public IList<Product> ListProducts(int page = 1, int pageSize = 0, string searchValue = "",
            int categoryId = 0, int supplierId = 0, decimal minPrice = 0, decimal maxPrice = 0)
        {
            var listProducts = new List<Product>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * 
                                from( select *, 
                                ROW_NUMBER() over (order by ProductName) as RowNumber
                                from Products
                                where (@searchValue = N'' or ProductName like @searchValue)
                                and (@categoryId = 0 or CategoryId = @categoryId)
                                and (@supplierId = 0 or SupplierId = @supplierId)
                                and (Price >= @minPrice)
                                and (@maxPrice <= 0 or Price <= @maxPrice)
                                ) as t
                            where ((@pageSize = 0) or RowNumber between (@page - 1) * @pageSize + 1 and @page * @pageSize)";

                var paramaters = new
                {
                    searchValue = $"%{searchValue}%",
                    page = page,
                    pageSize = pageSize,
                    categoryId = categoryId,
                    supplierId = supplierId,
                    minPrice = minPrice,
                    maxPrice = maxPrice
                };
                listProducts = conn.Query<Product>(sql, param: paramaters, commandType: CommandType.Text).ToList();
            }
            return listProducts;
        }

        public IList<ProductAttribute> ListAttributes(int productID)
        {
            var listAttributes = new List<ProductAttribute>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductAttributes where ProductID = @productID";
                listAttributes = conn.Query<ProductAttribute>(sql, new { productID = productID }, commandType: CommandType.Text).ToList();
            }
            return listAttributes;
        }

        public IList<ProductPhoto> ListPhotos(int productID)
        {
            var listPhotos = new List<ProductPhoto>();
            using (var conn = OpenConnection())
            {
                string sql = @"select * from ProductPhotos where ProductID = @productID";
                listPhotos = conn.Query<ProductPhoto>(sql, new { productID = productID }, commandType: CommandType.Text).ToList();
            }
            return listPhotos;
        }

        #endregion

        #region Cập nhật dữ liệu
        public bool UpdateProduct(Product item)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update Products set 
                                        ProductName = @ProductName, 
                                        ProductDescription = @ProductDescription, 
                                        SupplierID = @SupplierID, 
                                        CategoryID = @CategoryID, 
                                        Unit = @Unit, 
                                        Price = @Price,
                                        Photo = @Photo, 
                                        IsSelling = @IsSelling
                                        where ProductID = @ProductID";
                var paramaters = new
                {
                    ProductID = item.ProductID,
                    ProductName = item.ProductName,
                    ProductDescription = item.ProductDescription,
                    SupplierID = item.SupplierID,
                    CategoryID = item.CategoryID,
                    Unit = item.Unit,
                    Price = item.Price,
                    Photo = item.Photo,
                    IsSelling = item.IsSelling
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool UpdateAttribute(ProductAttribute item)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update ProductAttributes set 
                                        AttributeName = @AttributeName, 
                                        AttributeValue = @AttributeValue,
                                        DisplayOrder = @DisplayOrder
                                        where ProductID = @ProductID and AttributeID = @AttributeID";
                var paramaters = new
                {
                    ProductID = item.ProductID,
                    AttributeName = item.AttributeName,
                    AttributeValue = item.AttributeValue,
                    DisplayOrder = item.DisplayOrder,
                    AttributeID = item.AttributeID
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }

        public bool UpdatePhoto(ProductPhoto item)
        {
            var result = false;
            using (var conn = OpenConnection())
            {
                string sql = @"update ProductPhotos set 
                                        Photo = @Photo,
                                        Description = @Description,
                                        DisplayOrder = @DisplayOrder,
                                        IsHidden = @IsHidden
                                        where ProductID = @ProductID and PhotoID = @PhotoID";
                var paramaters = new
                {
                    Photo = item.Photo,
                    PhotoID = item.PhotoID,
                    Description = item.Description,
                    DisplayOrder = item.DisplayOrder,
                    IsHidden = item.IsHidden,
                    ProductID = item.ProductID
                };
                int count = conn.Execute(sql: sql, param: paramaters, commandType: CommandType.Text);
                conn.Close();
                result = count > 0;
            }
            return result;
        }
        #endregion
    }
}
