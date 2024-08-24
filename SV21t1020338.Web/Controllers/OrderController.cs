using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SV21t1020338.BusinessLayers;
using SV21t1020338.DomainModels;
using SV21t1020338.Web.AppCodes;
using SV21t1020338.Web.Models;

namespace SV21t1020338.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private const int PAGE_SIZE = 20;
        private const int Product_PAGE_SIZE = 5;
        private const string SEARCH_CONDITION = "order_search"; //Tên biến dùng để lưu trong session
        private const string Shopping_Cart = "shopping_cart"; //Tên biến dùng để lưu trong session
        private const string Product_Search = "product_search"; //Tên biến dùng để lưu trong session
        public IActionResult Index()
        {
            OrderSearchInput? input = ApplicationContext.GetSessionData<OrderSearchInput>(SEARCH_CONDITION);
            if (input == null)
            {
                input = new OrderSearchInput()
                {
                    Page = 1,
                    PageSize = PAGE_SIZE,
                    SearchValue = "",
                    Status = 0,
                    DateRange = string.Format("{0:dd/MM/yyyy} - {1:dd/MM/yyyy}",
                                                DateTime.Today.AddYears(-10),
                                                DateTime.Today)
                };
            }
            return View(input);

        }
        public IActionResult Search(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = OrderDataService.ListOrders(out rowCount, input.Page, input.PageSize, input.Status,
                                                        input.FromTime, input.ToTime, input.SearchValue ?? "");
            var model = new OrderSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Status = input.Status,
                TimeRange = input.DateRange ?? "",
                Data = data
            };
            ApplicationContext.SetSessionData(SEARCH_CONDITION, input);
            return View(model);
        }
        public IActionResult Details(int id = 0)
        {
            var order = OrderDataService.GetOrder(id);
            if (order == null)
                return RedirectToAction("Index");
            var detail = OrderDataService.ListOrderDetails(id);
            var model = new OrderDetailModel()
            {
                Order = order,
                Details = detail
            };
            return View(model);
        }
        [HttpGet]
        public IActionResult EditDetail(int id = 0, int productId = 0)
        {
            var model = OrderDataService.GetOrderDetail(id, productId);

            return View(model);
        }
        public IActionResult DeleteDetail(int id = 0, int productId = 0)
        {
            bool result = OrderDataService.DeleteOrderDetail(id, productId);
            if (!result)
                    TempData["Message"] = "Không thể xoá mặt hàng này ra khỏi đơn hàng";
            return RedirectToAction("Details", new { id = id });
        }
        [HttpPost]
        public IActionResult UpdateDetail(int orderId, int productId, int quantity, decimal saleprice)
        {
            if (quantity <= 0)
                return Json("Số lượng bán không hợp lệ");
            if (saleprice <= 0)
                return Json("Giá bán không hợp lệ");
            bool result = OrderDataService.SaveOrderDetail(orderId, productId, quantity, saleprice);
            if (!result)
                return Json("Không được phép thay đổi thông tin của đơn hàng này!");
            return Json("");
        }
        [HttpGet]
        public IActionResult Shipping(int id = 0)
        {
            ViewBag.OrderID = id;
            return View();
        }
        [HttpPost]
        public IActionResult Shipping(int id = 0, int ShipperID = 0)
        {
            if (ShipperID <= 0)
                return Json("Vui lòng chọn người giao hàng!");
            bool result = OrderDataService.ShipOrder(id, ShipperID);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển người giao hàng");
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Create()
        {
            var input = ApplicationContext.GetSessionData<ProductSearchInput>(Product_Search);
            if (input == null)
                input = new ProductSearchInput()
                {
                    Page = 1,
                    PageSize = Product_PAGE_SIZE,
                    SearchValue = ""
                };
            return View(input);
        }
        public IActionResult SearchProduct(OrderSearchInput input)
        {
            int rowCount = 0;
            var data = CommonProductService.ListProducts(out rowCount, input.Page, input.PageSize, input.SearchValue ?? "");
            var model = new ProductSearchResult()
            {
                Page = input.Page,
                PageSize = input.PageSize,
                SearchValue = input.SearchValue ?? "",
                RowCount = rowCount,
                Data = data
            };
            ApplicationContext.SetSessionData(Product_Search, input);
            return View(model);
        }
        private List<OrderDetail> GetShoppingCart()
        {
            var shoppingcart = ApplicationContext.GetSessionData<List<OrderDetail>>(Shopping_Cart);
            if (shoppingcart == null)
            {
                shoppingcart = new List<OrderDetail>();
                ApplicationContext.SetSessionData(Shopping_Cart, shoppingcart);
            }
            return shoppingcart;
        }
        public IActionResult ShowShoppingCart()
        {
            var model = GetShoppingCart();
            return View(model);
        }
        public IActionResult AddToCart(OrderDetail data)
        {
            if (data.SalePrice < 0 || data.Quantity < 0)
                return Json("giá bán và số lượng không hợp lệ");
            var shoppingcart = GetShoppingCart();
            var exitsproduct = shoppingcart.FirstOrDefault(x => x.ProductID == data.ProductID);
            if (exitsproduct == null)
            {
                shoppingcart.Add(data);
            }
            else
            {
                exitsproduct.Quantity += data.Quantity;
                
            }
            ApplicationContext.SetSessionData(Shopping_Cart, shoppingcart);
            return Json("");
        }
        public IActionResult RemoveFromCart(int id = 0)
        {
            var shoppingcart = GetShoppingCart();
            int index = shoppingcart.FindIndex(x => x.ProductID == id);
            if (index >= 0)
                shoppingcart.RemoveAt(index);
            ApplicationContext.SetSessionData(Shopping_Cart, shoppingcart);
            return Json("");
        }
        public IActionResult ClearCart(int id = 0)
        {
            var shoppingcart = GetShoppingCart();
            shoppingcart.Clear();
            ApplicationContext.SetSessionData(Shopping_Cart, shoppingcart);
            return Json("");
        }
        public IActionResult Init(int customerId=0, string deliveryProvince = "", string deliveryAddress = "")
        {
            var shoppingcart = GetShoppingCart();
            if (shoppingcart.Count == 0)
            {
                return Json("Giỏ hàng trống không thể lập đơn hàng!");
            }
            if (customerId == 0 || string.IsNullOrWhiteSpace(deliveryAddress) || string.IsNullOrWhiteSpace(deliveryProvince))
            {
                return Json("Vui lòng nhập đầy đủ thông tin!");
            }
            int emploeeId = Convert.ToInt32(User.GetUserData()?.UserId);
            int orderId = OrderDataService.InitOrder(emploeeId, customerId, deliveryProvince, deliveryAddress, shoppingcart);
            ClearCart();
            return Json(orderId);
        }

        public IActionResult Accept(int id = 0)
        {
            bool result = OrderDataService.AcceptOrder(id);
            if (!result)
                TempData["Message"] = "Không thể duyệt đơn hàng này";
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Finish(int id = 0)
        {
            bool result = OrderDataService.FinishOrder(id);
            if (!result)
                TempData["Message"] = "Không thể ghi nhận trạng thái kết thúc cho đơn hàng này";
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Cancel(int id = 0)
        {
            bool result = OrderDataService.CancelOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác huỷ đối với đơn hàng này";
            return RedirectToAction("Details", new { id = id });
        }
        public IActionResult Reject(int id = 0)
        {
            bool result = OrderDataService.RejectOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác từ chối đối với đơn hàng này";
            return RedirectToAction("Details", new { id =  id });
        }
        public IActionResult Delete(int id = 0)
        {
            bool result = OrderDataService.DeleteOrder(id);
            if (!result)
                TempData["Message"] = "Không thể thực hiện thao tác xoá đối với đơn hàng này";
            return RedirectToAction("Details", new { id = 0 });
        }
        [HttpGet]
        public IActionResult UpdateDelivery(int id)
        {
            ViewBag.OrderID = id;
            return View();
        }
        [HttpPost]
        public IActionResult UpdateDelivery(int id, string deliveryProvince, string deliveryAddress)
        {
            if(deliveryProvince.IsNullOrEmpty() || deliveryAddress.IsNullOrEmpty())
                return Json("Vui lòng nhập thông tin địa chỉ!");
            bool result = OrderDataService.UpdateAddress(id, deliveryProvince, deliveryAddress);
            if (!result)
                return Json("Đơn hàng không cho phép chuyển người giao hàng");
            return RedirectToAction("Details", new {id = id});
        }
    }
}
