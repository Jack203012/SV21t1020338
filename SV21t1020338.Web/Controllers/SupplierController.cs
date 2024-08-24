using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;

namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class SupplierController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public SupplierController(IWebHostEnvironment webHostEnvironment)
        {

            _webHostEnvironment = webHostEnvironment;

        }
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "supplier_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index(int page = 1, string searchValue = "")
        {
            PaginationSearchInput? input = ApplicationContext.GetSessionData<PaginationSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new PaginationSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = ""
                };
            }
            return View(input);
        }
        public IActionResult Search(PaginationSearchInput input)
        {
            int rowCount = 0;
            var data = CommonDataService.ListOfSuppliers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new SupplierSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Create()
        {
            ViewBag.Title = "Bổ Sung Nhà Cung Cấp";
            Supplier supplier = new Supplier()
            {
                SupplierID = 0
            };

            return View("Edit", supplier);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập Nhật Thông Tin Nhà Cung Cấp";
            Supplier? Supplier = CommonDataService.GetSupplier(id);
            if (Supplier == null)
            {
                return RedirectToAction("Index");
            }
            return View(Supplier);
        }
        [HttpPost]
        public IActionResult Save(Supplier data, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.SupplierID == 0 ? "Bổ sung nhà cung cấp" : "Cập nhật thông tin nhà cung cấp";
            if (string.IsNullOrWhiteSpace(data.SupplierName))
            {
                ModelState.AddModelError(nameof(data.SupplierName), "Tên nhà cung cấp không được để trống");
            }
            if (string.IsNullOrEmpty(data.ContactName))
            {
                ModelState.AddModelError(nameof(data.ContactName), "Tên giao dịch không được đê trống");
            }
            if (string.IsNullOrWhiteSpace(data.Province))
            {
                ModelState.AddModelError(nameof(data.Province), "Vui lòng chọn tỉnh/thành");
            }
            data.Phone = data.Phone ?? "";
            data.Email = data.Email ?? "";
            data.Address = data.Address ?? "";

            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            if (uploadPhoto != null)
            {
                data.Photo = UploadImage(uploadPhoto);
            }


            if (data.SupplierID == 0)
            {
                CommonDataService.AddSupplier(data);
            }
            else
            {
                CommonDataService.UpdateSupplier(data);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteSupplier(id);
                return RedirectToAction("Index");
            }
            var supplier =  CommonDataService.GetSupplier(id);
            if (supplier == null)
            {
                return RedirectToAction("Index");
            }
            return View(supplier);
        }
        private string UploadImage(IFormFile file)
        {
            var uniqueFileName = "";
            var folderPath = Path.Combine(_webHostEnvironment.WebRootPath, "images/suppliers");
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
