﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;

namespace SV21t1020338.Web.Controllers
{
    [Authorize(Roles ="admin")]
    public class EmployeeController : Controller
    {
        private const int PAGE_SIZE = 9;
        private const string SEARCH_CONDITION = "employee_search"; //Tên biến dùng để lưu trong session
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
            var data = CommonDataService.ListOfEmployees(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new EmployeeSearchResult()
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
            ViewBag.Title = "Bổ Sung Nhân Sự";
            Employee employee = new Employee()
            {
                EmployeeID = 0
            };
            return View("Edit", employee);
        }
        public IActionResult Edit(int id = 0)
        {
            ViewBag.Title = "Cập nhật thông tin nhân viên";
            var model = CommonDataService.GetEmployee(id);
            if (model == null)
                return RedirectToAction("Index");

            if (string.IsNullOrEmpty(model.Photo))
                model.Photo = "default.jpg";

            return View(model);
        }
        public IActionResult Delete(int id = 0, string PhotoPath = "")
        {
            if (Request.Method == "POST")
            {
                //Kiểm tra xem chuỗi có rỗng không
                if (!string.IsNullOrEmpty(PhotoPath))
                {
                    //Lấy đường dẫn thư mục lưu tệp
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/employees");
                    //Kết hợp lại để tạo một đường dẫn đầy đủ
                    var filePath = Path.Combine(uploadsFolder, PhotoPath);
                    //kiểm tra xem đường dẫn đó có tồn tại file ảnh không
                    if (System.IO.File.Exists(filePath))
                    {
                        //thực hiện xoá ảnh nếu tồn tại
                        System.IO.File.Delete(filePath);
                    }
                }
                CommonDataService.DeleteEmployee(id);
                return RedirectToAction("Index");
            }
            ViewBag.Title = "Xoá Nhân Viên";
            var employee = CommonDataService.GetEmployee(id);
            if (employee == null)
                return RedirectToAction("Index");
            ViewBag.AllowDelete = !CommonDataService.IsUsedEmployee(id);
            return View(employee);
        }

        public IActionResult Save(Employee data, string birthDateInput, IFormFile? uploadPhoto)
        {
            ViewBag.Title = data.EmployeeID == 0 ? "Bổ sung nhân viên" : "Cập nhật thông tin nhân viên";

            //Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrWhiteSpace(data.FullName))
                ModelState.AddModelError(nameof(data.FullName), "Họ tên nhân viên không được để trống");
            if (string.IsNullOrWhiteSpace(data.Email))
                ModelState.AddModelError(nameof(data.Email), "Vui lòng nhập email");
            if (string.IsNullOrWhiteSpace(data.Address))
                data.Address = "";
            if (string.IsNullOrWhiteSpace(data.Phone))
                data.Phone = "";

            //Xử lý ngày sinh
            DateTime? birthDate = birthDateInput.ToDateTime();
            if (birthDate.HasValue)
                data.BirthDate = birthDate.Value;

            //Xử lý với ảnh upload (nếu có ảnh upload thì lưu ảnh và gán lại tên file ảnh mới cho employee)
            if (uploadPhoto != null)
            {
                string fileName = $"{DateTime.Now.Ticks}_{uploadPhoto.FileName}"; //Tên file sẽ lưu
                string folder = Path.Combine(ApplicationContext.WebRootPath, @"images\employees"); //đường dẫn đến thư mục lưu file
                string filePath = Path.Combine(folder, fileName); //Đường dẫn đến file cần lưu D:\images\employees\photo.png

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadPhoto.CopyTo(stream);
                }
                data.Photo = fileName;
            }

            if (!ModelState.IsValid)
                return View("Edit", data);

            if (data.EmployeeID == 0)
            {
                CommonDataService.AddEmployee(data);
            }
            else
            {
                CommonDataService.UpdateEmployee(data);
            }
            return RedirectToAction("Index");
        }
    }
}