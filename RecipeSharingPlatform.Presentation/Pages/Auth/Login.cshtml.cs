using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RecipeSharingPlatform.Service.DTOs.RequestDTOs;
using RecipeSharingPlatform.Service.ServiceProviders.Interface;
using System.Security.Claims;

namespace RecipeSharingPlatform.Presentation.Pages.Auth
{
    public class LoginModel : PageModel
    {
        private readonly IServiceProviders _serviceProviders;

        public LoginModel(IServiceProviders serviceProviders)
        {
            _serviceProviders = serviceProviders;
        }

        [BindProperty]
        public LoginDTO LoginInfo { get; set; } = new LoginDTO();



        public void OnGet()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                Response.Redirect("/");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _serviceProviders.AuthService.Login(LoginInfo.Username, LoginInfo.Password);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
                return Page();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim("Avatar", user.ProfileImage)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            if (user.Role == "User")
            {
                return RedirectToPage("/Index");
            }
            else if (user.Role == "Admin")
            {
                return RedirectToPage("/Index");
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }
    }
}
