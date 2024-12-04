using IdentityClass.Data;
using IdentityClass.Dto;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebAPIProject.Services
{
    public interface IAuthService
    {
        Task<ResponseDto> RegisterAsync(RegistrationDto model);
        Task<LoginResponseDto> LoginAsync(LoginDto model);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        public async Task<ResponseDto> RegisterAsync(RegistrationDto model)
        {
            var newUser = new ApplicationUser()
            {
                UserName = model.Email,
                FirstName = model.FisrtName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                PasswordHash = model.Password
            };

            var userExists = await _userManager.FindByNameAsync(model.Email);
            if (userExists != null)
                return new ResponseDto { Message = "User already exists", Success = false };


            var result = await _userManager.CreateAsync(newUser, model.Password);

            if (result.Succeeded)
            {
                //Assign Role: Default first registr Admin, rest User
                var checkAdmin = await _roleManager.FindByNameAsync("Admin");

                if (checkAdmin == null)
                {
                    await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
                    await _userManager.AddToRoleAsync(newUser, "Admin");

                    return new ResponseDto { Message = "User registered successfully", Success = true };
                }
                else
                {
                    var checkRole = await _roleManager.FindByIdAsync("User");

                    if (checkRole == null)
                        await _roleManager.CreateAsync(new IdentityRole { Name = "User" });

                    await _userManager.AddToRoleAsync(newUser, "User");

                    return new ResponseDto { Message = "User registered successfully", Success = true };
                }

            }

            return new ResponseDto { Message = "Registration failed", Success = false };
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto model)
        {
            var user = await _userManager.FindByNameAsync(model.Email);
            if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
            {
                return GenerateToken(user);
            }

            return null;
        }

        private LoginResponseDto GenerateToken(ApplicationUser user)
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new LoginResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = token.ValidTo
            };
        }

    }
}
