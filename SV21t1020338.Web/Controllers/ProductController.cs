using Microsoft.AspNetCore.Mvc;
using SV21t1020338.DomainModels;
using SV21t1020338.BusinessLayers;
using SV21t1020338.Web.Models;
using Microsoft.AspNetCore.Authorization;
using SV21t1020338.Web.AppCodes;

namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IWebHostEnvironment webHostEnvironment )
        {

            _webHostEnvironment = webHostEnvironment;

        }
        const int PAGE_SIZE = 10;
        private const string SEARCH_CONDITION = "product_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            ProductSearchInput? input = ApplicationContext.GetSessionData<ProductSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {

                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    MaxPrice = 0,
                    MinPrice = 0,
                    CategoryId = 0,
                    SupplierId = 0
                };
            }
            return View(input);
        }
        public IActionResult Search(ProductSearchInput input)
        {
            int rowCount = 0;
            var data = CommonProductService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "",
                                                            input.CategoryId, input.SupplierId, input.MinPrice, input.MaxPrice);
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                CategoryID = input.CategoryId,
                SupplierID = input.SupplierId,
                MinPrice = input.MinPrice,
                MaxPrice = input.MaxPrice,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ sung mặt hàng";
            Product product = new Product()
            {
                ProductID = 0
            };
            return View("Edit", product);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Chỉnh sửa thông tin mặt hàng";
            var product = CommonProductService.GetProduct(id);
            return View(product);
        }
        public IActionResult Delete(int id = 0)
        {
            var product = CommonProductService.GetProduct(id);
            if (Request.Method == "POST")
            {
                var PhotoPath = product.Photo;
                //Kiểm tra xem chuỗi có rỗng không
                if (!string.IsNullOrEmpty(PhotoPath))
                {
                    //Lấy đường dẫn thư mục lưu tệp
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Products");
                    //Kết hợp lại để tạo một đường dẫn đầy đủ
                    var filePath = Path.Combine(uploadsFolder, PhotoPath);
                    //kiểm tra xem đường dẫn đó có tồn tại file ảnh không
                    if (System.IO.File.Exists(filePath))
                    {
                        //thực hiện xoá ảnh nếu tồn tại
                        System.IO.File.Delete(filePath);
                    }
                }
                CommonProductService.DeleteProduct(id);
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Xoá Mặt Hàng";
            if (product == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommonProductService.InUsedProduct(id);
            return View(product);
        }
        [HttpPost]
        public IActionResult SaveProduct(Product product, IFormFile uploadPhoto) //Sử dụng IFormFile để nhận tệp đã tải lên từ form.
        {
            ViewBag.Title = product.ProductID == 0 ? "Bổ sung mặt hàng" : "Chỉnh sửa thông tin mặt hàng";
            if (string.IsNullOrEmpty(product.ProductName))
            {
                ModelState.AddModelError(nameof(product.ProductName), "Tên mặt hàng không được để trống");
            }
            if (string.IsNullOrEmpty(product.ProductDescription))
            {
                ModelState.AddModelError(nameof(product.ProductDescription), "Mô tả mặt hàng không được để trống");
            }
            if (product.CategoryID == 0)
            {
                ModelState.AddModelError(nameof(product.CategoryID), "Vui lòng chọn loại hàng");
            }
            if (product.SupplierID == 0)
            {
                ModelState.AddModelError(nameof(product.SupplierID), "Vui lòng chọn nhà cung cấp");
            }
            if (string.IsNullOrEmpty(product.Unit))
            {
                ModelState.AddModelError(nameof(product.Unit), "Đơn vị tính không được để trống");
            }
            if (product.Price == 0)
            {
                ModelState.AddModelError(nameof(product.Price), "Vui lòng nhập giá mặt hàng hợp lệ");
            }

            if (product.ProductID == 0 && uploadPhoto == null && uploadPhoto.Length < 1) //kiểm tra nếu là thêm mới
            {
                ModelState.AddModelError(nameof(product.Photo), "Hãy chọn ảnh mặt hàng!");
            }
            if (ModelState.IsValid == false)
            {
                return View("Edit", product);
            }
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {
                ////Sử dụng form với phương thức POST và enctype="multipart/form-data" để cho phép tải lên tệp.
                ////Sử dụng Directory.GetCurrentDirectory() để lấy đường dẫn gốc của ứng dụng
                ////và kết hợp với wwwroot/images/employees để xác định thư mục lưu trữ tệp.
                //var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Products");
                //if (!string.IsNullOrEmpty(product.Photo))
                //{
                //    var filePathDel = Path.Combine(uploadsFolder, product.Photo);
                //    if (System.IO.File.Exists(filePathDel))
                //    {
                //        System.IO.File.Delete(filePathDel);
                //    }
                //}
                ////Tạo tên tệp duy nhất bằng cách sử dụng Guid.NewGuid().ToString().
                //var uniqueFileName = Guid.NewGuid().ToString() + "_" + uploadPhoto.FileName;
                //// Kết hợp đường dẫn thư mục và tên tệp để tạo đường dẫn đầy đủ
                //var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                //// Tạo thư mục nếu chưa tồn tại
                //if (!Directory.Exists(uploadsFolder))
                //{
                //    Directory.CreateDirectory(uploadsFolder);
                //}
                //// Sử dụng FileStream để lưu trữ tệp vào thư mục đích
                //using (var fileStream = new FileStream(filePath, FileMode.Create))
                //{
                //    uploadPhoto.CopyTo(fileStream);
                //}
                //// Đặt giá trị cho tham số photo là tên của file ảnh
                product.Photo = UploadImage(uploadPhoto);
            }
            if (product.ProductID == 0)
            {
                CommonProductService.AddProduct(product);
            }
            if (product.ProductID > 0)
            {
                CommonProductService.UpdateProduct(product);
            }
            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Photo(ProductPhoto photo, IFormFile uploadPhoto)
        {
            var formData = Request.Form;
            photo = new ProductPhoto()
            {
                ProductID = Convert.ToInt32(formData["ProductID"]),
                PhotoID = Convert.ToInt32(formData["PhotoID"]),
                Photo = formData["Photo"],
                Description = formData["Description"],
                DisplayOrder = Convert.ToInt32(formData["DisplayOrder"]),
                IsHidden = Convert.ToBoolean(formData["IsHidden"])
            };
            ViewBag.Title = photo.PhotoID == 0 ? "Bổ sung ảnh cho mặt hàng" : "Thay đổi ảnh mặt hàng";
            if (string.IsNullOrEmpty(photo.Description))
            {
                ModelState.AddModelError(nameof(photo.Description), "Hãy nhập mô tả ảnh mặt hàng!");
            }
            if (photo.DisplayOrder == 0)
            {
                ModelState.AddModelError(nameof(photo.DisplayOrder), "Hãy nhập thứ tự ảnh mặt hàng!");
            }
            if (uploadPhoto == null && string.IsNullOrEmpty(photo.Photo))
            {
                ModelState.AddModelError(nameof(photo.Photo), "Hãy chọn ảnh mặt hàng!");
            }
            if (ModelState.IsValid == false)
            {
                return View("Photo", photo);
            }
            if (uploadPhoto != null && uploadPhoto.Length > 0)
            {  
                photo.Photo = UploadImage(uploadPhoto);
            }
            if (photo.PhotoID == 0)
            {
                CommonProductService.AddProductPhoto(photo);
            }
            if (photo.PhotoID > 0)
            {
                CommonProductService.UpdateProductPhoto(photo);
            }
            return RedirectToAction("Edit", new { id = photo.ProductID });
        }
        [HttpPost]
        public IActionResult Attribute(ProductAttribute attribute)
        {
            var formData = Request.Form;
            if (formData != null)
            {
                attribute = new ProductAttribute()
                {
                    AttributeID = Convert.ToInt32(formData["AttributeID"]),
                    ProductID = Convert.ToInt32(formData["ProductID"]),
                    AttributeName = formData["AttributeName"],
                    AttributeValue = formData["AttributeValue"],
                    DisplayOrder = Convert.ToInt32(formData["DisplayOrder"])
                };
            }
            if (string.IsNullOrEmpty(attribute.AttributeName))
            {
                ModelState.AddModelError(nameof(attribute.AttributeName), "Hãy nhập tên thuộc tính mặt hàng!");
            }
            if (string.IsNullOrEmpty(attribute.AttributeValue))
            {
                ModelState.AddModelError(nameof(attribute.AttributeValue), "Hãy nhập giá trị thuộc tính mặt hàng!");
            }
            if (attribute.DisplayOrder == 0)
            {
                ModelState.AddModelError(nameof(attribute.DisplayOrder), "Hãy nhập thứ tự thuộc tính mặt hàng!");
            }
            if (ModelState.IsValid == false)
            {
                return View("Attribute", attribute);
            }
            if (attribute.AttributeID == 0)
            {
                CommonProductService.AddAttribute(attribute);
            }
            if (attribute.AttributeID > 0)
            {
                CommonProductService.UpdateAttribute(attribute);
            }
            return RedirectToAction("Edit", new { id = attribute.ProductID });
        }
        public IActionResult Photo(int id, string method, long photoId = 0)
        {
            switch (method)
            {
                case "add":
                    {
                        ViewBag.Title = "Bổ sung ảnh cho mặt hàng";
                        ProductPhoto productPhoto = new ProductPhoto()
                        {
                            ProductID = id,
                            PhotoID = 0
                        };
                        return View(productPhoto);
                    }
                case "edit":
                    {
                        ViewBag.Title = "Thay đổi ảnh mặt hàng";
                        ProductPhoto productPhoto = CommonProductService.GetProductPhoto(photoId);
                        return View(productPhoto);
                    }
                case "delete":
                    {
                        var photoPath = CommonProductService.GetProductPhoto(photoId)?.Photo;
                        if (!string.IsNullOrEmpty(photoPath))
                        {
                            //Lấy đường dẫn thư mục lưu tệp
                            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/Products");
                            //Kết hợp lại để tạo một đường dẫn đầy đủ
                            var filePath = Path.Combine(uploadsFolder, photoPath);
                            //kiểm tra xem đường dẫn đó có tồn tại file ảnh không
                            if (System.IO.File.Exists(filePath))
                            {
                                //thực hiện xoá ảnh nếu tồn tại
                                System.IO.File.Delete(filePath);
                            }
                        }
                        CommonProductService.DeleteProductPhoto(photoId);
                        return RedirectToAction("Edit", new { id = id });
                    }
                default:
                    return RedirectToAction("Index");
            }
        }
        public IActionResult Attribute(int id = 0, string method = "", long attributeId = 0)
        {
            switch (method)
            {
                case "add":
                    {
                        ViewBag.Title = "Bổ sung thuộc tính cho mặt hàng";
                        ProductAttribute attribute = new ProductAttribute()
                        {
                            ProductID = id,
                            AttributeID = 0
                        };
                        return View(attribute);
                    }
                case "edit":
                    {
                        ViewBag.Title = "Thay đổi thuộc tính cho mặt hàng";
                        ProductAttribute attribute = CommonProductService.GetAttribute(attributeId);
                        return View(attribute);
                    }
                case "delete":
                    {
                        CommonProductService.DeleteAttribute(attributeId);
                        return RedirectToAction("Edit", new { id = id });
                    }
                default:
                    return RedirectToAction("Index");
            }
        }
        private string UploadImage(IFormFile file)
        {
            var uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/Products");
            uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(folderPath, uniqueFileName);
            using (FileStream fileStream = System.IO.File.Create(filePath))
            {
                file.CopyTo(fileStream);
            }
            return uniqueFileName;
        }
    }
}
