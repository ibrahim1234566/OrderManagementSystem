using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OrderManagementSystem.Data;
using OrderManagementSystem.Models;
using OrderManagementSystem.Repositories.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OrderManagementSystem.Services
{
    public class AuthService
    {
        private readonly UserRepository _userRepo;
        private readonly IConfiguration _config;
        private readonly CustomerRepository _customerRepo;

        public AuthService(UserRepository userRepo, IConfiguration config , CustomerRepository customerRepo)
        {
            _userRepo = userRepo;
            _config = config;
            _customerRepo = customerRepo;
        }

        public async Task<string> RegisterAsync(string username, string password, string role, string? email = null)
        {
            if (role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
                role = "Admin";
            else if (role.Equals("Customer", StringComparison.OrdinalIgnoreCase))
                role = "Customer";
            else
                throw new Exception("Role must be either 'Admin' or 'Customer'");
            if (role == "Customer")
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new Exception("Customer email is required");
            }
            var existingUser = await _userRepo.FindAsync(u => u.Username == username);
            if (existingUser.Any())
                throw new Exception("Username already exists");

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                PasswordHash = hashedPassword,
                Role = role
            };

            await _userRepo.AddAsync(user);
            await _userRepo.SaveChangesAsync();

            if (role == "Customer")
            {
                var customer = new Customer
                {
                    Name = username,
                    Email = email!
                };

                await _customerRepo.AddAsync(customer);
                await _customerRepo.SaveChangesAsync();
            }

            return GenerateToken(user);
        }

        public async Task<string> LoginAsync(string username, string password)
        {
            var user = (await _userRepo.FindAsync(u => u.Username == username)).FirstOrDefault();
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                throw new Exception("Invalid username or password");

            return GenerateToken(user);
        }

        private string GenerateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
