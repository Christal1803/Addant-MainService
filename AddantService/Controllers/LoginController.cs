using AddantSDal;
using AddantSDal.DAL;
using AddantSDal.DTO;
using AddantSDAL.DAL;
using AddantSDAL.DTO;
using AddantService.Models;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Mvc;

namespace AddantService.Controllers
{
    [System.Web.Http.RoutePrefix("api/Login")]
    public class LoginController : BaseController
    {
        private ILoginRepository _loginRepository;
        #region api method
        public LoginController(ILoginRepository loginRepository)
        {
            _loginRepository = loginRepository;
        }
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("CheckUser")]
        public IHttpActionResult CheckLogin([FromBody] Models.LoginModel loginModel)
        {
            try
            {
                var obj = new AddantSDal.DTO.LoginDTO
                {
                    UserName = loginModel.UserName,
                    Password = loginModel.Password,
                    IsActive = loginModel.IsActive,
                    UserId = loginModel.UserId,
                    Deleted=loginModel.Deleted
                };
                var res = _loginRepository.CheckLogin(obj);
                if (res?.Object != null)
                {
                    res.Object.Token = GetToken(res.Object.UserId.ToString(), res.Object.UserName);
                }
                return WebResult(res);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

       // [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("DashBoard")]
        public IHttpActionResult GetDashBoardBlockData(int Category = 1)
        {
            var res = _loginRepository.GetDashBoardBlockData(Category);
            return WebResult(res);
        }

        [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ShortListed")]
        public IHttpActionResult GetShortListed()
        {
            var res = _loginRepository.GetShortListed();
            return WebResult(res);
        }

        #endregion
        #region functions
        public string GenerateToken()
        {
            string _token = string.Empty;
            try
            {
                byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
                byte[] ranKey = Guid.NewGuid().ToByteArray();
                _token = Convert.ToBase64String(time.Concat(ranKey).ToArray());
                return _token;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }
        public static bool IsTokenExpired(string token)
        {
            try
            {
                if (!string.IsNullOrEmpty(token))
                {
                    byte[] _data = Convert.FromBase64String(token);
                    DateTime _creationTime = DateTime.FromBinary(BitConverter.ToInt64(_data, 0));
                    if (_creationTime < DateTime.UtcNow.AddHours(-24))
                        return true;
                    else
                        return false;
                }
                return true;
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return true; }
        }
        #endregion

        public string GetToken(string _userid = "", string _name = "")
        {
            try
            {
                string key = "my_secret_key_12345"; //Secret key which will be used later during validation    
                var issuer = "";  //site url here   

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                //Create a List of Claims, Keep claims name short    
                var permClaims = new List<Claim>();
                permClaims.Add(new Claim("valid", "1"));
                //Create Security Token object by giving required parameters    
                var token = new JwtSecurityToken(
                                issuer, //Issure    
                                issuer,  //Audience    
                                permClaims,
                                expires: DateTime.Now.AddDays(1),
                                signingCredentials: credentials
                                );
                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return jwt_token.ToString();
            }
            catch (Exception ex) { Logger.WriteLog(ex.Message.ToString()); return null; }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("Visitors")]
        public IHttpActionResult GetVisitors(int Category = 1)
        {

            var res = AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisit1(Category, "2022-01-01", "today");
            if (res != null)
                return WebResult(res);
            else
                return null;
        }

        //[System.Web.Http.Authorize]
        //[System.Web.Http.HttpGet]
        //[System.Web.Http.Route("VisitorCount")]
        //public IHttpActionResult GetVisitorCount(int Category = 1) 
        //{

        //    var res = AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisitorCount(Category, "2022-01-01", "today");
        //    if (res != null)
        //        return WebResult(res);
        //    else
        //        return null;
        //}

        [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("CountrywiseVisitors")]

        public IHttpActionResult GetCountrywiseVisitors(int Category = 1)
        {
            var res = AddantSDAL.DAL.GoogleConsoleSearch.GetWebsiteVisitCountrywise(Category, "2022-01-01", "today");
            if (res != null)
                return WebResult(res);
            else
                return null;
        }

		////
		///For resetting Passwords 
		///
		//[System.Web.Http.Authorize]
		[System.Web.Http.HttpPost]
		[System.Web.Http.Route("SendOTP")]
        public IHttpActionResult SendOTP([FromBody] Models.LoginModel loginModel)
        {
			var obj = new AddantSDal.DTO.LoginDTO
            {
                UserName = loginModel.UserName,
				Password = loginModel.Password,
				IsActive = loginModel.IsActive,
				UserId = loginModel.UserId
			};
			var res = _loginRepository.CheckLogin(obj);
            if (res != null)
            {
                Random rand = new Random();
                string otp = (rand.Next(10000, 99999)).ToString();

                var emailField = new Email.EmailField
                {
                    UserName = res?.Object?.UserName,
                    Title = res?.Object?.UserName,
                    Body = otp,
                    ToMail = loginModel.UserName,
                    Subject = "[Addant Systems] Please reset Your Password "
				};
                OTPDto Otpdto = new OTPDto();
                Otpdto.OTP = otp;
                Otpdto.TimeStamp = DateTime.Now;
                Otpdto.Status = true;
				Otpdto.UserName = loginModel.UserName;

				var resOTP = _loginRepository.CreateOTP(Otpdto);
                Email.SendOTPEmail(emailField);
            }
            if (res != null)
                return WebResult(res);
            else
                return null;
        }


        ////
        ///ForValidate OTP
        ///
      //  [System.Web.Http.Authorize]
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("ValidateOTP")]
        public IHttpActionResult ValidateOTP(string UserName, string otp)
        {
            var res = _loginRepository.ValidateOTP(UserName, otp);
            return WebResult(res);
        }

        ///ResetPasword
        ///

       // [System.Web.Http.Authorize]
        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("ResetPassword")]
        public IHttpActionResult ResetPassword([FromBody] Models.UserResetModel userModel)
        {
            //Time being user password is assigned to lastname
            UserDTO userDTO = new UserDTO
            {
                Username = userModel.Username,
                Password = userModel.Password,
               
            };

            var res = _loginRepository.ResetPassword(userDTO);
            return WebResult(res);
        }
    }

}