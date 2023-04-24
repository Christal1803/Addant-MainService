using Microsoft.PowerBI.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AddantSDAL.DAL
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
