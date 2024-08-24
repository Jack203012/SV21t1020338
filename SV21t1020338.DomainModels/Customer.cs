namespace SV21t1020338.DomainModels
/// <summary>
/// Thông tin khách hàng
/// </summary>

{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string Province { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsLocked{ get; set; }



    }
}
