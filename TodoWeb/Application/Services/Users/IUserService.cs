using TodoWeb.Application.Dtos.UserModel;
using TodoWeb.Domains.Entities;

namespace TodoWeb.Application.Services.Users
{
    public interface IUserService
    {
        public int Post(UserCreateViewModel user);
        
        public User? UserLogin(UserLoginViewModel user);

        string GenerateJwt(User user);
    }
}
