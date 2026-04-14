using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using NexaWork.Application.DTOs.Authentication;
using NexaWork.Domain.Constants;
using NexaWork.Domain.Entities;
using NexaWork.Domain.IdentityEntites;
using NexaWork.Infrastructure;

namespace NexaWork.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly UserManager<NexaWorkUser> _userManager;
        private readonly SignInManager<NexaWorkUser> _signInManager;
        private readonly NexaWorkDbContext _nexaWorkDbContext;
        private readonly IConfiguration _config;

        public AuthenticationController(
            NexaWorkDbContext nexaWorkDbContext,
        UserManager<NexaWorkUser> userManager,
        SignInManager<NexaWorkUser> signInManager,
        IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _nexaWorkDbContext = nexaWorkDbContext;
        }




        /// <summary>
        /// Endpoint đăng ký tài khoản mới. Hệ thống sẽ kiểm tra tính hợp lệ của dữ liệu, 
        /// tạo tài khoản mới bằng Identity và trả về kết quả cho frontend.
        /// </summary>
        /// <param name="registerDTO">Nhận vào Email và Password.</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // 1. Kiểm tra xem Email hoặc Username đã tồn tại chưa
            var userExists = await _userManager.FindByEmailAsync(registerDTO.Email);

            if (userExists != null)
            {
                return BadRequest(new ResponseDTO
                {
                    Success = false,
                    Message = "Email is not valid or already in use."
                });
            }

            // Khởi tạo đối tượng User mới
            var user = new NexaWorkUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Email,
                SecurityStamp = Guid.NewGuid().ToString() // Rất quan trọng cho Identity
            };

            // Thực hiện tạo User (Identity sẽ tự động Băm mật khẩu ở bước này)
            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");

                return Ok(new ResponseDTO
                {
                    Success = true,
                    Message = "Registration successful!"
                });
            }

            // 4. Nếu thất bại (ví dụ: mật khẩu quá yếu), gom các lỗi lại để báo cho Frontend
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ResponseDTO
            {
                Success = false,
                Message = "Registration failed.",
                Error = errors
            });
        }


        /// <summary>
        /// Hàm này dùng để tạo một Customer mới trong database sau khi người dùng đăng nhập thành công. 
        /// Mỗi Customer sẽ liên kết với một IdentityUser thông qua IdentityUserId
        /// </summary>
        /// <param name="userId">ID của người dùng Identity</param>
        /// <returns>True nếu tạo thành công, ngược lại là False</returns>
        private async Task<bool> CreateNewCustomer(string userId)
        {
            bool isOk = false;
            try
            {
                // Tạo một Customer mới với UserId vừa tạo
                var newCustomer = new Customer
                {
                    CustomerId = Guid.NewGuid(),
                    IdentityUserId = userId
                };
                await _nexaWorkDbContext.AddAsync(newCustomer);
                await _nexaWorkDbContext.SaveChangesAsync();
                isOk = true;
            }
            catch (System.Exception)
            {

            }

            return isOk;
        }




        /// <summary>
        /// Endpoint đăng nhập. Người dùng có thể đăng nhập bằng Email hoặc Username.
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns>Trả về token JWT nếu đăng nhập thành công</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Tìm người dùng bằng Email hoặc Username
            var user = await _userManager.FindByEmailAsync(loginDTO.UsernameOrEmail)
                       ?? await _userManager.FindByNameAsync(loginDTO.UsernameOrEmail);

            if (user == null)
            {
                // Trả về lỗi chung chung để bảo mật
                return Unauthorized(new ResponseDTO { Success = false, Message = "Tài khoản hoặc mật khẩu không chính xác." });
            }

            // 2. Sử dụng SignInManager để kiểm tra mật khẩu
            // LƯU Ý: Dùng CheckPasswordSignInAsync thay vì PasswordSignInAsync 
            // vì hàm này chỉ kiểm tra tính hợp lệ mà không tạo Cookie đăng nhập (rất phù hợp cho API dùng JWT)
            // Tham số lockoutOnFailure: true sẽ tự động đếm số lần sai và khóa tài khoản nếu vượt quá giới hạn
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, lockoutOnFailure: true);
            

            // Sau khi đăng nhập thành công, kiểm tra xem đã có Customer nào liên kết với IdentityUser này chưa
            var customer = await _nexaWorkDbContext.Customers.FirstOrDefaultAsync(c => c.IdentityUserId.Equals(user.Id));
            if (customer == null){
                // Nếu chưa tồn tại Customer nào liên kết với IdentityUser này, tạo mới một Customer
                var createCustomerResult = await CreateNewCustomer(user.Id);
                if (!createCustomerResult)
                {
                    return StatusCode(StatusCodes.Status500InternalServerError, new ResponseDTO
                    {
                        Success = false,
                        Message = "Đăng nhập thất bại do lỗi hệ thống. Vui lòng thử lại."
                    });
                }
            }
            


            if (result.Succeeded)
            {
                // 3. Nếu thành công, tạo JWT Token
                var token = await GenerateJwtTokenAsync(user);

                return Ok(new ResponseDTO
                {
                    Success = true,
                    AccessToken = token,
                    Message = "Đăng nhập thành công!"
                });
            }

            if (result.IsLockedOut)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseDTO
                {
                    Success = false,
                    Message = "Tài khoản đã bị khóa do đăng nhập sai quá nhiều lần. Vui lòng thử lại sau."
                });
            }

            if (result.IsNotAllowed)
            {
                return StatusCode(StatusCodes.Status403Forbidden, new ResponseDTO
                {
                    Success = false,
                    Message = "Tài khoản chưa được xác thực email hoặc không được phép đăng nhập."
                });
            }

            // Trường hợp sai mật khẩu
            return Unauthorized(new ResponseDTO { Success = false, Message = "Tài khoản hoặc mật khẩu không chính xác." });
        }


        /// <summary>
        /// Hàm này dùng để tạo JWT Token sau khi người dùng đăng nhập thành công. 
        /// Token sẽ chứa các claim cơ bản như UserId, Email, Username và Roles của người dùng.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private async Task<string> GenerateJwtTokenAsync(NexaWorkUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Lấy danh sách Roles của User và đưa vào Token
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // Thời gian hết hạn
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }







        /// <summary>
        /// Endpoint xử lý quy trình quên mật khẩu
        /// </summary>
        /// <param name="request">Nhận vào mail request của người dùng</param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            // BẢO MẬT: Luôn trả về thành công dù email có tồn tại hay không 
            // để tránh bị hacker dò quét email trong hệ thống
            // if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
            if (user == null)
            {
                return Ok(new ResponseDTO { Success = true, Message = "If mail exist, we'll send you the request. 111" });
            }

            // 1. Tạo Token reset password
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 2. Encode Token để truyền an toàn trên URL (tránh lỗi ký tự đặc biệt)
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // 3. Tạo Link frontend
            // Lưu ý: Thay đổi URL này thành URL frontend của bạn (VD: http://localhost:3000)
            // var resetLink = $"http://localhost:3000/reset-password?email={request.Email}&token={encodedToken}";
            var resetLink = $"{BaseURLConstants.REACT_APP_URL}/reset-password?email={request.Email}&token={encodedToken}";

            // 4. GỬI EMAIL THỰC TẾ Ở ĐÂY
            // Tạm thời để log ra Console để bạn test. Trong thực tế bạn sẽ dùng thư viện như MailKit hoặc SendGrid.
            Console.WriteLine($"\n--- EMAIL MÔ PHỎNG --- \nTo: {request.Email}\nLink Reset: {resetLink}\n----------------------\n");

            return Ok(new ResponseDTO { Success = true, Message = "If mail exist, we'll send you the request." });
        }



        /// <summary>
        /// Endpoint dùng để reset mật khẩu
        /// </summary>
        /// <param name="request">Nhận vào thông tin reset password của người dùng, bao gồm email, new password và token</param>
        /// <returns></returns>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return BadRequest(new ResponseDTO { Success = false, Message = "Request not valid" });
            }

            // Giải mã lại Token từ URL
            var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));

            // Thực hiện đổi mật khẩu bằng Identity
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

            if (result.Succeeded)
            {
                return Ok(new ResponseDTO { Success = true, Message = "Reset password successfully!" });
            }

            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            return BadRequest(new ResponseDTO { Success = false, Message = $"Lỗi: {errors}" });
        }
    }
}
