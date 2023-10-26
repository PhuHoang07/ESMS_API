using Business.Services.SecretService;
using ESMS_Data.Entities;
using ESMS_Data.Repositories.UserRepository;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<ResultModel> Authenticate(UserModel user)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var currentUser = await _userRepository.GetUser(user.Email);

                if (currentUser != null && currentUser.IsActive.Value)
                {
                    user.Role = currentUser.Role.Name;

                    resultModel.IsSuccess = true;
                    resultModel.StatusCode = (int)HttpStatusCode.OK;
                    resultModel.Data = user;
                }
                else
                {
                    resultModel.IsSuccess = true;
                    resultModel.StatusCode = (int)HttpStatusCode.OK;
                    resultModel.Message = "User not allowed to access.";
                }

            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }

        public ResultModel GenerateToken(UserModel user)
        {
            ResultModel resultModel = new ResultModel();

            try
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecretService.SecretService.JwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role),
                    
                };

                var token = new JwtSecurityToken(
                                        claims: claims, 
                                        issuer: _configuration["JwtSettings:Issuer"],
                                        audience: _configuration["JwtSettings:Audience"],
                                        expires: DateTime.Now.AddHours(1), 
                                        signingCredentials: credentials
                                    );

                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.OK;
                resultModel.Data = new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception ex)
            {
                resultModel.IsSuccess = false;
                resultModel.StatusCode = (int)HttpStatusCode.BadRequest;
                resultModel.Message = ex.Message;
            }

            return resultModel;
        }
    }
}
