using AddantSDal.DTO;
using AddantSDAL.DTO;
using AddantService.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDal.DAL
{
    public interface ILoginRepository
    {
        DALResult<LoginDTO>  CheckLogin(LoginDTO user);
        DALResult<List<DashBoardBlockData>> GetDashBoardBlockData(int category);
        DALResult<List<CandidateDTO>> GetShortListed();
		DALResult<UserDTO> SendOTP(string userName);
        DALResult<OTPDto>CreateOTP(OTPDto otp);
        DALResult<OTPDto> ValidateOTP(string UserName, string otp);
        DALResult<bool> ResetPassword(UserDTO userDTO);
    }
}
