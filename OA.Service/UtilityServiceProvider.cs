using Microsoft.Extensions.Configuration;
using OA.Service.Contracts;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace OA.Service
{
    public class UtilityServiceProvider : IUtilityServiceProvider
    {
        private readonly IConfiguration _configuration;
        public UtilityServiceProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMail(string mailSubject, string toAddress, string mailContent, byte[] bytes)
        {
            string strSMTPServer = _configuration["AppSettings:EmailSettings:SMTPServer"].ToString();
            int iSMTPPort = Convert.ToInt32(_configuration["AppSettings:EmailSettings:SMTPPort"].ToString());
            string strBCCAddress = _configuration["AppSettings:EmailSettings:BCCAddress"].ToString();
            string strSMTPUser = _configuration["AppSettings:EmailSettings:SMTPUserName"].ToString();
            string strSMTPDisplayName = _configuration["AppSettings:EmailSettings:SMTPDisplayName"].ToString();
            string strSMTPPassword = _configuration["AppSettings:EmailSettings:SMTPPassword"].ToString();
            var mailAddressFrom = new MailAddress(strSMTPUser, strSMTPDisplayName);
            var mailAddressTo = new MailAddress(toAddress);
            using (MailMessage mm = new MailMessage(mailAddressFrom, mailAddressTo))
            {
                mm.Bcc.Add(strBCCAddress);
                mm.Subject = mailSubject;
                mm.Body = mailContent;
                //if (bytes != null)  mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "MR.pdf"));
                mm.IsBodyHtml = true;
                mm.Priority = MailPriority.High;
                SmtpClient smtp = new SmtpClient();
                smtp.Host = strSMTPServer;
                smtp.EnableSsl = false;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                NetworkCredential NetworkCred = new NetworkCredential(strSMTPUser, strSMTPPassword);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = NetworkCred;
                smtp.Port = iSMTPPort;
                try
                {
                    smtp.Send(mm);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    smtp.Dispose();
                }
            }            
        }
        public async Task SendSMS(string toMobile, string txtMessage)
        {
            string user = _configuration["AppSettings:SMSSettings:SMSUserName"].ToString();
            string pass = _configuration["AppSettings:SMSSettings:SMSPass"].ToString();
            string apiKey = _configuration["AppSettings:SMSSettings:APIKey"].ToString();
            string adminMobileNo = _configuration["AppSettings:SMSSettings:AdminMobile"].ToString();
            short routeId = Convert.ToInt16(_configuration["AppSettings:SMSSettings:RouteId"].ToString());

            ////Sender ID,While using route4 sender id should be 6 characters long.
            string senderId = _configuration["AppSettings:SMSSettings:SMSSendersID"].ToString();
            string mobileNumbers = toMobile; //+ "," + adminMobileNo;
            //Your message to send, Add URL encoding here.
            string message = txtMessage; // HttpUtility.UrlEncode(txtMessage);

            //string strUrl = "http://182.18.162.128/api/mt/SendSMS?user=Thinktech&password=think123&senderid=thinkt&channel=Trans&DCS=0&flashsms=0&number=" + mobileNumbers + "&text=" + message + "&route=8";
            //string strUrl = String.Format("http://182.18.162.128/api/mt/SendSMS?user={0}&password={1}&senderid={2}&channel=Trans&DCS=0&flashsms=0&number={3}&text={4}&route={5}", user, pass, senderId, mobileNumbers, message, routeId);

            //--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

            //string strUrl = String.Format("http://182.18.162.128/api/mt/SendSMS?apikey=85I1g6L9hEeIntNZgQRrzA&senderid=ddrhow&channel=trans&DCS=0&flashsms=0&number=919831052038&text=test%20message&route=08");
            string strUrl = String.Format("http://182.18.162.128/api/mt/SendSMS?apikey={0}&senderid={1}&channel=trans&DCS=0&flashsms=0&number={2}&text={3}&route={4}", apiKey, senderId, mobileNumbers, message, routeId);
            WebRequest webRequest = HttpWebRequest.Create(strUrl);
            HttpWebResponse httpWebResp = (HttpWebResponse)webRequest.GetResponse();
            Stream objStream = (Stream)httpWebResp.GetResponseStream();
            StreamReader streamReader = new StreamReader(objStream);
            string dataString1 = streamReader.ReadToEnd();
            httpWebResp.Close();
            objStream.Close();
            streamReader.Close();
        }
    }
}
