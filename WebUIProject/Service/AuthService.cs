using IdentityClass.Dto;
using IdentityClass.Utility;

namespace WebUIProject.Service
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegistrationDto registrationRequestDto);
    }
    public class AuthService : IAuthService
    {
        private readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> LoginAsync(LoginDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto()
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/AuthAPI/login"
            }, withBearer: false);
        }

        public Task<ResponseDto?> RegisterAsync(RegistrationDto registrationRequestDto)
        {
            throw new NotImplementedException();
        }
    }
}
