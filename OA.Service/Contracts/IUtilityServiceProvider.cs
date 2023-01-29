using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace OA.Service.Contracts
{
    public interface IUtilityServiceProvider
    {        
        Task SendSMS(string toMobile, string txtMessage);
        Task SendMail(string mailSubject, string toAddress, string mailContent, byte[] bytes);
    }
}
