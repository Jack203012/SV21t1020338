using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SV21t1020338.DataLayers
{
    public interface ICommonDAL<T> where T : class
    {
       
        //tìm kiếm và hiển thị dữ liệu dưới dạng phân trang
        IList<T> List(int page = 1, int pageSize = 0, string searchValue = "");
        //  đếm số dòng dữ liệu tìm được
        int Count(string searchValue = "");
        //  lấy 1 bảng ghi, dòng dữ liệu dựa trên mã
        T? Get(int id);
       // bổ xung dữ liệu vào bảng. hàm trả về id của dữ liệu được bổ xung
        int Add (T data);
        //cập nhật dữ liệu 
        bool Update(T data);
        //xoá dữ liệu dựa vào id
        bool Delete(int id);
        //kiểm tra xem 1 dòng dữ liệu có mã là id
        //hiện có liên quan đến các bảng dữ liệu hay không
        bool InUsed(int id);

    }
}
