using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;


namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "category_search"; //Tên biến dùng để lưu trong session
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
            var data = CommonDataService.ListOfCategories(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CategorySearchResult()
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
            ViewBag.Title = "Bổ Sung Loại Hàng";
            Category category = new Category()
            {
                CategoryID = 0
            };
            return View("Edit", category);
            
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin loại hàng";
            Category? category = CommonDataService.GetCategory(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
            return View(category);
        }
        [HttpPost]
        public IActionResult Save(Category data)
        {
            ViewBag.Title = data.CategoryID == 0 ? "Bổ sung loại hàng" : "Cập nhật thông tin loại hàng";
            if (string.IsNullOrWhiteSpace(data.CategoryName))
            {
                ModelState.AddModelError(nameof(data.CategoryName), "Tên loại hàng không được để trống");
            }
            if (string.IsNullOrEmpty(data.Description))
            {
                ModelState.AddModelError(nameof(data.Description), "Mô tả loại hàng không được đê trống");
            }
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            // gọi chức năng xử lí dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi


            if (data.CategoryID == 0)
            {
                CommonDataService.AddCategory(data);
            }
            else
            {
                CommonDataService.UpdateCategory(data);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCategory(id);
                return RedirectToAction("Index");
            }

            var category = CommonDataService.GetCategory(id);
            if (category == null)
            {
                return RedirectToAction("Index");
            }
           
            return View(category);
        }
    
    }
}
