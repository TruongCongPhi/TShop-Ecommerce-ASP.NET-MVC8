using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using System.Drawing;
using System.Security.Policy;
using TShop.Models;
using TShop.ViewModels;

namespace TShop.Areas.Admin.Controllers
{
    [Area("admin")]
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
            _notifyService = notifyService;
            _passwordHasher = passwordHasher;

        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = _context.Accounts.Include(a => a.Location).Include(a => a.Role);

            return View(await accounts.ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");
            ViewData["LocationId"] = new SelectList(_context.Locations.Where(l => l.ParentCode == null), "LocationId", "Name");

            if (id == null)
            {
                var accountIdClaim = User.FindFirst("AccountId")?.Value;

                if (!string.IsNullOrEmpty(accountIdClaim) && int.TryParse(accountIdClaim, out int accountId))
                {
                    var account = _context.Accounts.Where(c => c.AccountId == accountId).Include(a => a.Location).Include(a => a.Role);
                    if (account == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    return View(account.FirstOrDefault());
                }
                return Redirect("404");
            }

            var accounts = _context.Accounts.Where(a => a.AccountId == id).Include(a => a.Location).Include(a => a.Role);

            return View(accounts.FirstOrDefault());
        }
        [HttpPost]
        public async Task<IActionResult> Edit(int id, Account account)
        {
            if (id != account.AccountId)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var accountUpdate = await _context.Accounts.Where(a => a.AccountId == account.AccountId).FirstOrDefaultAsync();

                accountUpdate.FullName = account.FullName;
                accountUpdate.Email = account.Email;
                accountUpdate.Phone = account.Phone;
                accountUpdate.Address = account.Address;
                accountUpdate.Ward = account.Ward;
                accountUpdate.District = account.District;
                accountUpdate.LocationId = account.LocationId;
                accountUpdate.RoleId = account.RoleId;
                accountUpdate.Birthday  = account.Birthday;
                accountUpdate.CreateAt = account.CreateAt;
                accountUpdate.LastLogin = account.LastLogin;
                accountUpdate.Active = account.Active;
                _context.Update(accountUpdate);
                await _context.SaveChangesAsync();
                _notifyService.Success("Cập nhật thành công");
                return RedirectToAction(nameof(Index));
            }
            return View(account);
        }

        [HttpPost("/admin/accounts/delete")]
        public async Task<IActionResult> DeleteAjax(int accountId)
        {

            var account = _context.Accounts.Where(o => o.AccountId == accountId).FirstOrDefault();
            _context.Accounts.Remove(account);
            _context.SaveChanges();
            return Json(new { success = true, message = "Đã xóa" });

        }
        [HttpPost("admin/accounts/search")]
        public IActionResult SearchAccounts(string query)
        {
            IQueryable<Account> accountQuery = _context.Accounts
                .Include(p => p.Location)
                .Include(p => p.Role);

            if (!string.IsNullOrEmpty(query))
            {
                var searchQuery = query.ToLower();
                accountQuery = accountQuery.Where(p => p.FullName.ToLower().Contains(searchQuery) ||
                                                         p.Phone.ToLower().Contains(searchQuery) ||
                                                         p.Role.RoleName.ToLower().Contains(searchQuery) ||
                                                         p.Email.ToLower().Contains(searchQuery));
            }

            var accounts = accountQuery.ToList();
            return PartialView("_AccountListPartial", accounts); // Create a partial view for product list rendering
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create(RegisterVM register)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingAccount = await _context.Accounts.FirstOrDefaultAsync(c => c.Email == register.Email);

                    if (existingAccount != null)
                    {
                        ModelState.AddModelError("Email", "Email đã được sử dụng");
                        return View(register);
                    }

                    var account = new Account
                    {
                        Email = register.Email,
                        Active = true,
                        RoleId = register.RoleId,
                    };

                    account.Password = _passwordHasher.HashPassword(account, register.Password);
                    _context.Accounts.Add(account);
                    await _context.SaveChangesAsync();
                    _notifyService.Success("Đăng kí thành công");
                    return RedirectToAction(nameof(Index));

                }
                _notifyService.Warning("Lỗi");
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");

                return View(register);
            }
            catch (Exception ex)
            {
                _logger.LogError("Xảy ra lỗi " + ex.Message);
                ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleName");

                return View(register);
            }
        }
    }
}
