using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TShop.ViewModels
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DisplayName("Mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool keepLoggedIn { get; set; }
    }
}
