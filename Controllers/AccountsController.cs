using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Controllers
{
    public class AccountsController : Controller
    {

        private readonly ILogger<AccountsController> _logger;
        private readonly TshopContext _context;
        private readonly IPasswordHasher<Account> _passwordHasher;
        public INotyfService _notifyService { get; }

        public AccountsController(ILogger<AccountsController> logger, TshopContext context, IPasswordHasher<Account> passwordHasher, INotyfService notifyService)
        {
            _logger = logger;
            _context = context;
            _passwordHasher = passwordHasher;
            _notifyService = notifyService;
        }

        public IActionResult Register()
        {
            return View();
        } 

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Email == register.Email);

                    if(existingAccount != null)
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng");
                        return View(register);
                    }

                    var account = new Account
                    {
                        Email = register.Email,
                        Active = true,
                        RoleId = 2,
                    };

                    account.Password = _passwordHasher.HashPassword(account,register.Password);
                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Đăng kí thành công");
                    return RedirectToAction("Login","Accounts");

                }

                return View(register);
            }
            catch (Exception ex) 
            {
                _logger.LogError("Xảy ra lỗi " + ex.Message);
                return View(register);
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM login)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var account = await _context.Accounts.Include(c => c.Role).FirstOrDefaultAsync(c => c.Email == login.Email);

                    if (account != null)
                    {
                        var result = _passwordHasher.VerifyHashedPassword(account, account.Password, login.Password);

                        _logger.LogInformation("mat khau: " + result);

                        if (result == PasswordVerificationResult.Success)
                        {
                            if(account.Active == false)
                            {
                                _notifyService.Warning("Tài khoản bị khóa");
                                ModelState.AddModelError("", "Tài khoản này đã bị khóa");
                                return View(login);
                            }
                            _logger.LogInformation(account.Role?.RoleName);
                            var claims = new[] {
                            new Claim(ClaimTypes.Name, account.Email),
                            new Claim("AccountId", account.AccountId.ToString()),
                            new Claim(ClaimTypes.Role, account.Role?.RoleName ?? "customer")
                            };

                            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                            var principal = new ClaimsPrincipal(identity);

                            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
                            {
                                IsPersistent = login.keepLoggedIn,
                                ExpiresUtc = login.keepLoggedIn ? DateTime.UtcNow.AddDays(1) : (DateTime?)null
                            });
                            account.LastLogin = DateTime.Now;
                            _context.Accounts.Update(account);
                            _context.SaveChanges();
                            _notifyService.Success("Đăng nhập thành công");

                            return RedirectToAction("Index", "Home");
                        }
                        else ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                    }
                    else ModelState.AddModelError("", "Email hoặc mật khẩu không chính xác");
                }
                return View(login);
            }
            catch(Exception ex) 
            {
                _logger.LogError("Lỗi " + ex.Message);
                return View(login);
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public IActionResult Infor(int?id)
        {
            if(id != null)
            {
                var account = _context.Accounts.Where(a=> a.AccountId ==  id).FirstOrDefault();
                return View(account);
            }
            var accountIdClaim = User.FindFirst("AccountId")?.Value;

            if (!string.IsNullOrEmpty(accountIdClaim) && int.TryParse(accountIdClaim, out int accountId))
            {
                var account = _context.Accounts.Where(c => c.AccountId == accountId).FirstOrDefault();
                if (account == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                return View(account);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult UpdateInfor(Account account)
        {
            var accountUpdate = _context.Accounts.Where(a => a.AccountId == account.AccountId).FirstOrDefault();

            if (accountUpdate == null)
            {
                return RedirectToAction("Index", "Home");
            }
            accountUpdate.FullName = account.FullName;
            accountUpdate.Phone = account.Phone;
            accountUpdate.Birthday = account.Birthday;
        
            _context.Accounts.Update(accountUpdate);
            _context.SaveChanges();

            return RedirectToAction("Infor", "Accounts");
        }
        public IActionResult Order()
        {
            try
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;

                if (!string.IsNullOrEmpty(accountIdClaim) && int.TryParse(accountIdClaim, out int accountId))
                {
                    var account = _context.Accounts.Where(c => c.AccountId == accountId).FirstOrDefault();
                    if (account == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    var orders = _context.Orders.Where(o => o.AccountId == accountId).OrderByDescending(c=> c.OrderDate).ToList();
                    return View(orders);
                }
                else
                {
                    return RedirectToAction("Index", "Home");

                }
            }
            catch(Exception e)
            {
                _logger.LogInformation("lỗi", e.Message);
                return RedirectToAction("Index", "Home");
            }
        }
        [HttpGet("hoa-don/chi-tiet/{orderid}")]
        public IActionResult OrderDetail(int orderid)
        {
            try
            {
                var order = _context.Orders
                .Include(c => c.Account)
                .Include(o => o.Payment)
                .Include(o => o.TransactStatus)
                .FirstOrDefault(o => o.OrderId == orderid);
                var orderDetail = _context.OrderDetails.Where(d => d.OrderId == orderid)
                    .ToList();
                if (orderDetail == null)
                {
                    TempData["Message"] = $"Không có hóa đơn";
                    return Redirect("/404");
                }

                var variantIds = orderDetail.Select(o => o.VariantDetailId).ToList();

                List<CartItemVM> cartItemVMs = new List<CartItemVM>();
                foreach (var item in orderDetail)
                {
                    var variantDetail = _context.VariantDetails
                         .Where(vd => vd.VariantDetailId == item.VariantDetailId)
                         .Select(vd => new CartItemVM
                         {
                             Product = vd.Variant.Product,
                             Amount = item.Quantity.Value,
                             Color = vd.Variant.Color,
                             Thumb = vd.Variant.Thumb,
                             Size = vd.Size.SizeName,
                         }).FirstOrDefault();
                    cartItemVMs.Add(variantDetail);

                }
                var orderDetailVM = new OrderDetailVM()
                {
                    Order = order,
                    FullName = order.Account?.FullName,
                    PhoneNumber = order.Account?.Phone,
                    Payment = order.Payment?.PaymentType,
                    CartItems = cartItemVMs.ToList(),
                };

                return View(orderDetailVM);
            }
            catch (Exception e)
            {
                _logger.LogInformation("Lỗi " + e.Message);
                return Redirect("/404");
            }
        }

    }
}
