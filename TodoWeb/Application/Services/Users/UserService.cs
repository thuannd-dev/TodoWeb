using AutoMapper;
using FluentValidation;
using TodoWeb.Application.Dtos.UserModel;
using TodoWeb.Application.Helpers;
using TodoWeb.Domains.Entities;
using TodoWeb.Infrastructures;

namespace TodoWeb.Application.Services.Users
{
    public class UserService : IUserService
    {
        private readonly IValidator<UserCreateViewModel> _validator;
        private readonly IApplicationDbContext _dbContext;
        private readonly IMapper _mapper;
        public UserService(IValidator<UserCreateViewModel> validator, IApplicationDbContext dbContext, IMapper mapper)
        {
            _validator = validator;
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public int Post(UserCreateViewModel user)
        {
            //var data = new Domains.Entities.User
            //{
            //    EmailAddress = user.EmailAddress,
            //    Password = user.Password,
            //    FullName = user.FullName,
            //    Role = user.Role
            //};
            var data = _mapper.Map<User>(user);
            var salting = HashHelper.GennerateRandomString(100);
            var password = user.Password + salting;
            data.Password = HashHelper.HashBcrypt(password);
            data.Salting = salting;
            _dbContext.Users.Add(data);
            _dbContext.SaveChanges();
            return data.Id;

        }

        public User UserLogin(UserLoginViewModel user)
        {
            //kiểm tra email, password có tồn tại trong db hay không
            var data = _dbContext.Users
                .FirstOrDefault(x => x.EmailAddress == user.EmailAddress);
            if (data == null)
            {
                return null;
            }

            var password = user.Password + data.Salting;
            if (!(data != null &&
                   HashHelper.VerifyBcrypt(password, data.Password)))
            {
                return null;
            }
            return data;
        }
    }
}
