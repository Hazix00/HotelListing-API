using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using HotelListingAPI.HotelListing.Models;

namespace HotelListingAPI.HotelListing.Services
{
    public interface IAuthManager
    {
        Task<bool> ValidateUser(LoginUserDTO loginUserDTO);
        Task<string> CreateToken();
    }
}