using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using OA.Data;
using OA.Model;
using OA.Repo.Contracts;
using OA.Repo.Contracts.Common;
using OA.Utility;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace OA.Repo.Repositories
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IConfiguration _configuration;
        private string _dbConnString = string.Empty;
        private string _jwtKey = string.Empty;
        private string _issuer = string.Empty;
        private string _audience = string.Empty;
        private IRepositoryBase<Loginuser> _repositoryLoginUser;
        public AuthenticationRepository(IConfiguration configuration, IRepositoryBase<Loginuser> repositoryLoginUser)
        {
            _configuration = configuration;
            _repositoryLoginUser = repositoryLoginUser;

            //Get AppSettings Json Informations
            _dbConnString = _configuration["AppSettings:ConnectionStrings:IlsheguriDb"];
            _jwtKey = _configuration["AppSettings:Jwt:Key"];
            _issuer = _configuration["AppSettings:Jwt:Issuer"];
            _audience = _configuration["AppSettings:Jwt:Audiance"];
        }

        public APIResponse UserLogin(LoginRequest request)
        {
            var response = new APIResponse();
            string email = request.Email.Trim();
            string encryptPwd = CommonUtility.EncryptData(request.Pwd);
            var loginUser = _repositoryLoginUser.GetFirstOrDefault(a => a.UserName == email && a.Password == encryptPwd && a.IsActive == 1 && a.IsDeleted == 0);            
            if (loginUser != null)
            {
                string roleName = loginUser.Role.RoleName;
                string token = this.GenerateJwt(email, roleName, _jwtKey, _issuer, _audience);
                var userLoginData = new UserLoginData
                {
                    Email = email,
                    DisplayName = loginUser.DisplayName,
                    Token = token
                };

                response.Message = "Login Succeeded!";
                response.DataResult = userLoginData;
                response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                response.Message = "Unauthorized!";
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            return response;
        }        
        public string GenerateJwt(string Email, string Role, string JwtKey, string Issuer, string Audience)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            //claim is used to add identity to JWT token
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, Email),
                new Claim(ClaimTypes.Role,Role),
                new Claim(ClaimTypes.Version,"v3.1"),
                new Claim("Date", DateTime.Now.ToString()),
            };

            var token = new JwtSecurityToken(Issuer,
              Audience,
              claims,    //null original value
              expires: DateTime.Now.AddMinutes(120),

              //notBefore:
              signingCredentials: credentials);

            string Data = new JwtSecurityTokenHandler().WriteToken(token); //return access token 
            return Data;
        }

        //public APIResponse GetUserProfile(UserDataRequest request)
        //{
        //    var response = new APIResponse();
        //    var userLoginInfo = new UserLoginInformation();            
        //    DataTable dtResult;
        //    //using (SqlConnection sqlConn = new SqlConnection(_dbConnString))
        //    //{
        //    //    using (SqlCommand sqlCmd = new SqlCommand("[dbo].[uspGetUserProfile]", sqlConn))
        //    //    {
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Mobile", request.MobileNo));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@FirstName", request.FirstName));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@LastName", request.LastName));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Email", request.Email));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@LocationId", request.LocationId));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Pincode", request.Pincode));
        //    //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    //        using (SqlDataAdapter sqlAdpt = new SqlDataAdapter(sqlCmd))
        //    //        {
        //    //            dtResult = new DataTable();
        //    //            sqlAdpt.Fill(dtResult);
        //    //            if (dtResult != null && dtResult.Rows.Count > 0)
        //    //            {
        //    //                foreach (DataRow dr in dtResult.Rows)
        //    //                {
        //    //                    long userId = Convert.ToInt64(dr["UserId"]);
        //    //                    userLoginInfo.Mobile = dr["Mobile"].ToString();
        //    //                    userLoginInfo.DisplayName = dr["UserName"] != DBNull.Value ? dr["UserName"].ToString() : string.Empty;
        //    //                    string email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : string.Empty;
        //    //                    userLoginInfo.LocationId = dr["LocationId"] != DBNull.Value ? Convert.ToInt32(dr["LocationId"]) : 0;
        //    //                    string locationName = dr["LocationName"] != DBNull.Value ? dr["LocationName"].ToString() : string.Empty;
        //    //                    string pincode = dr["Pincode"] != DBNull.Value ? dr["Pincode"].ToString() : string.Empty;
        //    //                    //userLoginInfo.RoleId = Convert.ToInt64(dr["RoleId"]);
        //    //                    string roleName = dr["RoleName"].ToString();
        //    //                    userLoginInfo.Token = this.GenerateJwt(userId.ToString(),
        //    //                                                userLoginInfo.DisplayName,
        //    //                                                roleName,
        //    //                                                _jwtKey,
        //    //                                                _issuer,
        //    //                                                _audience
        //    //                                                );

        //    //                }

        //    //                response.Message = "Login Successful!";
        //    //                response.DataResult = userLoginInfo;
        //    //                response.StatusCode = (int)HttpStatusCode.OK;
        //    //            }
        //    //            else
        //    //            {                            
        //    //                response.Message = "Bad Request! Please Try Again.";
        //    //                response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    return response;
        //}
        //public APIResponse CreateOTP(OTPData request)
        //{
        //    var response = new APIResponse();
        //    bool createStatus = false;
        //    //using (SqlConnection sqlConn = new SqlConnection(_dbConnString))
        //    //{
        //    //    using (SqlCommand sqlCmd = new SqlCommand("[dbo].[uspCreateOTP]", sqlConn))
        //    //    {
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Mobile", request.MobileNo));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@OTP", request.OTP));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@CreateStatus", createStatus));
        //    //        sqlCmd.Parameters["@CreateStatus"].Direction = ParameterDirection.Output;
        //    //        sqlCmd.CommandType = CommandType.StoredProcedure;

        //    //        if (sqlConn.State == ConnectionState.Closed) sqlConn.Open();
        //    //        sqlCmd.ExecuteNonQuery();
        //    //        createStatus = (bool)sqlCmd.Parameters["@CreateStatus"].Value;
        //    //        if (sqlConn.State == ConnectionState.Open) sqlConn.Close();

        //    //        response.DataResult = createStatus;
        //    //        response.StatusCode = (int)HttpStatusCode.OK;

        //    //    }
        //    //}

        //    return response;
        //}
        //public APIResponse VerifyOTP(OTPData request)
        //{
        //    var response = new APIResponse();
        //    //using (SqlConnection sqlConn = new SqlConnection(_dbConnString))
        //    //{
        //    //    using (SqlCommand sqlCmd = new SqlCommand("[dbo].[uspVerifyOTP]", sqlConn))
        //    //    {
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Mobile", request.MobileNo));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@OTP", request.OTP));
        //    //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    //        if (sqlConn.State == ConnectionState.Closed) sqlConn.Open();
        //    //        int otpVerified = Convert.ToInt32(sqlCmd.ExecuteScalar());
        //    //        if (sqlConn.State == ConnectionState.Open) sqlConn.Close();

        //    //        if (otpVerified==1)
        //    //        {
        //    //            response.DataResult = otpVerified;
        //    //            response.Message = "OTP Verified!";
        //    //            response.StatusCode = (int)HttpStatusCode.OK;
        //    //        }
        //    //        else if (otpVerified == 2)
        //    //        {
        //    //            response.DataResult = otpVerified;
        //    //            response.Message = "OTP Verified!";
        //    //            response.StatusCode = (int)HttpStatusCode.OK;
        //    //        }
        //    //        else
        //    //        {
        //    //            response.DataResult = otpVerified;
        //    //            response.Message = "OTP Mismatched Or Expired! Please Try Again.";
        //    //            response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    //        }
        //    //    }
        //    //}

        //    return response;
        //}
        //public APIResponse AdminLogin(AdminLoginRequest request)
        //{
        //    var response = new APIResponse();
        //    var userLoginInfo = new UserLoginInformation();
        //    string encryptPwd = CommonUtility.EncryptData(request.Pwd);
        //    DataTable dtResult;
        //    //using (SqlConnection sqlConn = new SqlConnection(_dbConnString))
        //    //{
        //    //    using (SqlCommand sqlCmd = new SqlCommand("[dbo].[uspAuthenticateAdmin]", sqlConn))
        //    //    {
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Email", request.Email));
        //    //        sqlCmd.Parameters.Add(new SqlParameter("@Pwd", encryptPwd));
        //    //        sqlCmd.CommandType = System.Data.CommandType.StoredProcedure;

        //    //        using (SqlDataAdapter sqlAdpt = new SqlDataAdapter(sqlCmd))
        //    //        {
        //    //            dtResult = new DataTable();
        //    //            sqlAdpt.Fill(dtResult);
        //    //            if (dtResult != null && dtResult.Rows.Count > 0)
        //    //            {
        //    //                foreach (DataRow dr in dtResult.Rows)
        //    //                {
        //    //                    long userId = Convert.ToInt64(dr["UserId"]);
        //    //                    userLoginInfo.Mobile = dr["Mobile"].ToString();
        //    //                    userLoginInfo.FirstName = dr["FirstName"] != DBNull.Value ? dr["FirstName"].ToString() : string.Empty;
        //    //                    userLoginInfo.LastName = dr["LastName"] != DBNull.Value ? dr["LastName"].ToString() : string.Empty;
        //    //                    userLoginInfo.DisplayName = dr["UserName"] != DBNull.Value ? dr["UserName"].ToString() : string.Empty;
        //    //                    string email = dr["Email"] != DBNull.Value ? dr["Email"].ToString() : string.Empty;
        //    //                    //userLoginInfo.LocationId = dr["LocationId"] != DBNull.Value ? Convert.ToInt32(dr["LocationId"]) : 0;
        //    //                    //string locationName = dr["LocationName"] != DBNull.Value ? dr["LocationName"].ToString() : string.Empty;
        //    //                    //string pincode = dr["Pincode"] != DBNull.Value ? dr["Pincode"].ToString() : string.Empty;
        //    //                    userLoginInfo.RoleId = Convert.ToInt64(dr["RoleId"]);
        //    //                    string roleName = dr["RoleName"].ToString();
        //    //                    userLoginInfo.Token = this.GenerateJwt(userId.ToString(),
        //    //                                                userLoginInfo.DisplayName,
        //    //                                                roleName,
        //    //                                                _jwtKey,
        //    //                                                _issuer,
        //    //                                                _audience
        //    //                                                );

        //    //                }

        //    //                response.Message = "Login Successful!";
        //    //                response.DataResult = userLoginInfo;
        //    //                response.StatusCode = (int)HttpStatusCode.OK;
        //    //            }
        //    //            else
        //    //            {
        //    //                response.Message = "Bad Request! Please Try Again.";
        //    //                response.StatusCode = (int)HttpStatusCode.BadRequest;
        //    //            }
        //    //        }
        //    //    }
        //    //}
        //    return response;
        //}




    }
}
