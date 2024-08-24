using SV21t1020338.DomainModels;

namespace SV21t1020338.Web.Models
{
    public class OrderDetailModel
    {
        public Order Order { get; set; }
        public List<OrderDetail> Details { get; set; }
    }
}
