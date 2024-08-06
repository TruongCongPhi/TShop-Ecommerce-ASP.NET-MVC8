using System.ComponentModel.DataAnnotations;
using TShop.Models;

namespace TShop.ViewModels
{
    public class CheckoutVM
    {
        public int CustomerId { get; set; }
        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Vui lòng Nhập địa chỉ")]
        public string Address { get; set; }
        public string? Note { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Tỉnh / Thành")]
        public int LocationIdCity { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Quận / Huyện")]
        public int LocationIdDistrict { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn Phường / Xã")] 
        public int LocationIdWard { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phương thức thanh toán")]
        public int PaymentId { get; set; }
        public List<Payment> Payments { get; set; } = new List<Payment>();

    }


}
