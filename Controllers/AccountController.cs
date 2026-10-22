
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAttendanceManagement.Data;
using StudentAttendanceManagement.Models;
using StudentAttendanceManagement.ViewModels;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace StudentAttendanceManagement.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly PasswordHasher<User> _passwordHasher;

    public AccountController(AppDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<User>();
    }

    // Display the signup page
    [HttpGet]
    public IActionResult Signup()
    {
        return View();
    }

    // Process the submitted signup form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Signup(SignupViewModel model)
    {
        // Validate the submitted fields
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Normalize user input
        string email = model.Email.Trim().ToLowerInvariant();
        string username = model.Username.Trim();

        // Check whether the email already exists
        if (await _context.Users.AnyAsync(u => u.Email == email))
        {
            ModelState.AddModelError(
                nameof(model.Email),
                "This email is already registered.");
        }

        // Check whether the username already exists
        if (await _context.Users.AnyAsync(u => u.Username == username))
        {
            ModelState.AddModelError(
                nameof(model.Username),
                "This username is already taken.");
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Create the database entity
        var user = new User
        {
            FullName = model.FullName.Trim(),
            Email = email,
            Username = username,
            CreatedAt = DateTime.UtcNow
        };

        // Hash the PIN before storing it
        user.PinHash = _passwordHasher.HashPassword(user, model.Pin);

        try
        {
            _context.Users.Add(user);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Account created successfully!";

            return RedirectToAction("Login");
        }

        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                string.Empty,
                "Unable to create your account. Please check your details and try again.");

            return View(model);
        }
    }
    

    //Display the Login page
    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    //Process the submitted Login Form
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        string email = model.UsernameOrEmail.Trim().ToLowerInvariant();
        string username = model.UsernameOrEmail.Trim();

        var user = await _context.Users
        .FirstOrDefaultAsync(u =>
        u.Username == username||
        u.Email == email);

        if(user==null)
        {
            ModelState.AddModelError(string.Empty,"Invalid username/email or PIN.");

            return View(model);
        }

        var result = _passwordHasher.VerifyHashedPassword(
            user,
            user.PinHash,
            model.Pin);
        
        if (result == PasswordVerificationResult.Failed)
        {
            ModelState.AddModelError(
                string.Empty,
                "Invalid username/email or PIN.");

            return View(model);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),

            new Claim(ClaimTypes.Name,user.Username),

            new Claim(ClaimTypes.Email,user.Email)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal);
        
       
        return RedirectToAction("Index","Home");
    }


    //Logout the user and clear the authentication cookie
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        TempData.Clear();

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        return RedirectToAction(nameof(Login));
    }

}
