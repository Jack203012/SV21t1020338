using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;

namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class ShipperController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "shipper_search"; //Tên biến dùng để lưu trong session
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
            var data = CommonDataService.ListOfShippers(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ShipperSearchResult()
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
            ViewBag.Title = "Bổ Sung Đơn Vị Vận Chuyển";
            Shipper shipper = new Shipper()
            {
                ShipperID = 0
            };
            return View("Edit", shipper);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập Nhật Thông Tin Đơn Vị Vận Chuyển";
            Shipper? customer = CommonDataService.GetShipper(id);
            if (customer == null)
            {
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult Save(Shipper data)
        {
            ViewBag.Title = data.ShipperID == 0 ? "Bổ sung đơn vị vận chuyển" : "Cập nhật thông tin đơn vị vận chuyển";
            if (string.IsNullOrWhiteSpace(data.ShipperName))
            {
                ModelState.AddModelError(nameof(data.ShipperName), "Tên khách hàng không được để trống");
            }
            
            data.Phone = data.Phone ?? "";
            if (!ModelState.IsValid)
            {
                return View("Edit", data);
            }

            // gọi chức năng xử lí dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi


            if (data.ShipperID == 0)
            {
                CommonDataService.AddShipper(data);
            }
            else
            {
                CommonDataService.UpdateShipper(data);
            }
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteShipper(id);
                return RedirectToAction("Index");
            }
            var shipper = CommonDataService.GetShipper(id);
            if (shipper == null)
            {
                return RedirectToAction("Index");
            }
            return View(shipper);
        }
    }
}
