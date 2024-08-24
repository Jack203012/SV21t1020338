using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;


namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class CustomerController : Controller
    {
        const int PAGE_SIZE = 20;
        private const string SEARCH_CONDITION = "customer_search"; //Tên biến dùng để lưu trong session


        public IActionResult Index()
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
            var data = CommonDataService.GetCustomerList(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new CustomerSearchResult()
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
            ViewBag.Title = "Bo Sung Khach Hang";
            Customer customer = new Customer()
            {
                CustomerId = 0
            };
            return View("Edit",customer);
        }
        public IActionResult Edit( int id=0)
        {
            ViewBag.Title = "Cap Nhat Thong Tin Khach Hang";

            Customer? customer = CommonDataService.GetCustomer(id);
            if (customer == null)
            {
                return RedirectToAction("Index");
            }
            return View(customer);
        }
        [HttpPost]
        public IActionResult Save(Customer data)
        {
            ViewBag.Title = data.CustomerId == 0 ? "Bổ sung khách hàng" : "Cập nhật thông tin khách hàng";
            if (string.IsNullOrWhiteSpace(data.CustomerName))
            {
                ModelState.AddModelError(nameof(data.CustomerName),"Tên khách hàng không được để trống");
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

            if(!ModelState.IsValid)
            {
                return View("Edit",data);
            }

            // gọi chức năng xử lí dưới tầng tác nghiệp nếu quá trình kiểm soát lỗi không phát hiện lỗi
            
            
            if(data.CustomerId == 0)
            {
                CommonDataService.AddCustomer(data);
            }
            else
            {
                CommonDataService.UpdateCustomer(data);
            }
            return RedirectToAction("Index");
        }


        public IActionResult Delete(int id = 0)
        {
            if (Request.Method == "POST")
            {
                CommonDataService.DeleteCustomer(id);
                return RedirectToAction("Index");
            }
            
            var customer = CommonDataService.GetCustomer(id);
            if(customer == null)
            {
                return RedirectToAction("Index"); 
            }
            ViewBag.AllowDelete = !CommonDataService.InUseCustomer(id);
            return View(customer);  
        }
        
    }
}
