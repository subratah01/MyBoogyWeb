using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OA.Model
{
    public class LoginRequest
    {
        public string Email { get; set; }
        public string Pwd { get; set; }
    }

    public class UserLoginData
    {
        public string Email { get; set; }
        public string DisplayName { get; set; }
        public string Token { get; set; }

    }




    //public class UserDataRequest
    //{        
    //    public string MobileNo { get; set; }
    //    public string OTP { get; set; }
    //    public string UserName { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string Email { get; set; }
    //    public string LocationId { get; set; }
    //    public string Pincode { get; set; }

    //}
    //public class AdminLoginRequest
    //{
    //    public string Email { get; set; }
    //    public string Pwd { get; set; }

    //}

    //public class OTPData
    //{
    //    public string MobileNo { get; set; }
    //    public string OTP { get; set; }
    //}

    //public class UserLoginInformation
    //{
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string DisplayName { get; set; }
    //    public int? LocationId { get; set; }
    //    public string Token { get; set; }
    //    public long UserId { get; set; }
    //    public string Mobile { get; set; }
    //    public string Email { get; set; }
    //    public long RoleId { get; set; }
    //    public string RoleName { get; set; }

    //    //public string LocationName { get; set; }
    //    //public string Pincode { get; set; }

    //}
}
