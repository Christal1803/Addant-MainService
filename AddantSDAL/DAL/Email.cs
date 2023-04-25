using AddantSDAL.DTO;
using AddantService;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AddantSDAL.DAL
{
    public static class Email
    {
         //using SqlProviderServices = System.Data.Entity.SqlServer.SqlProviderServices;

        //#endregion
        #region email
        /// <summary>
        /// to send mail to hr
        /// </summary>
        /// <param name="candidateDTO"></param>
        public static void SendCandidateEmail(EmailField emailField)
        {

            string resumePath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");

            var message = new MailMessage();
            message.To.Add(new MailAddress(emailField?.UserName + " <" + emailField?.ToMail + ">"));
            message.From = new MailAddress("Career@Addant" + "<" + ConfigurationManager.AppSettings["PrMailAddress"] + ">");
            message.Subject = emailField?.Subject;
            message.Body = CreateEmailBodyFromTemplate(emailField);
            message.IsBodyHtml = true;
            if (!string.IsNullOrEmpty(resumePath) && !string.IsNullOrEmpty(emailField?.ResumeUrl))
                message.Attachments.Add(new Attachment(Path.Combine(resumePath, emailField?.ResumeUrl)));


            using (var smtp = new SmtpClient())
            {
                smtp.EnableSsl = true;
                smtp.Send(message);

            }
        }
        /// <summary>
        /// Send mail to person while submiting enquiry
        /// </summary>
        /// <param name="candidateDTO"></param>
        public static void SendEnquiryEmail(EmailField emailField)
        {
            //string resumePath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");
            try
            {
                var message = new MailMessage();
                message.To.Add(new MailAddress(emailField.UserName + " <" + (string.IsNullOrWhiteSpace(emailField?.ToMail) ? ConfigurationManager.AppSettings["PrMailAddress"] : emailField?.ToMail) + ">")); //Replace with HR mail addacomntenquiry@gmail.com 
                message.From = new MailAddress("Addant Systems" + "<" + ConfigurationManager.AppSettings["PrMailAddress"] + ">");
                message.Subject = emailField?.Subject;
                message.Body = CreateEmailBodyFromTemplate(emailField);
                message.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.EnableSsl = true;
                    smtp.Send(message);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog($"SendEnquiryEmail exception {JsonConvert.SerializeObject(ex)}");
            }          
        }
        public static void SendBlogEmail(EmailField emailField)
        {

            string resumePath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");
            string ccAddresses = ConfigurationManager.AppSettings["ccAddresses"].ToString();
            var message = new MailMessage();
            message.To.Add(new MailAddress("Addant Systems" + " <" + (string.IsNullOrWhiteSpace(emailField?.ToMail) ? ConfigurationManager.AppSettings["PrMailAddress"] : emailField?.ToMail) + ">"));
            for (int i = 0; i < ccAddresses.Split(',').Length; i++)
            {
                message.CC.Add(new MailAddress(ccAddresses.Split(',')[i]));
            }
            message.From = new MailAddress("Addant Systems" + "<" + ConfigurationManager.AppSettings["PrMailAddress"] + ">");
            message.Subject = emailField?.Subject;
            message.Body = CreateEmailBodyFromTemplate(emailField);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }
        /// <summary>
        /// to assign values to email template
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string createEmailBody(EmailField emailField)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath(emailField.TemplatePath)))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{UserName}", emailField?.UserName);
            body = body.Replace("{Position}", emailField?.Position);
            body = body.Replace("{Message}", emailField?.Message);
            body = body.Replace("{Title}", emailField?.Title);
            body = body.Replace("{Mail}", emailField.Mail);
            body = body.Replace("{Link}", emailField.Link);

            return body;
        }
        private static string CreateEmailBodyFromTemplate(EmailField emailField)
        {
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(HttpContext.Current.Server.MapPath("/DefaultEmailTemplate.html")))
            {
                body = reader.ReadToEnd();
            }
            var _emailfeildData = ExtractHeaderFromBody(emailField?.Body);
            body = body.Replace("{Body}", _emailfeildData?.Body);
            body = body.Replace("{Greetings}", _emailfeildData?.Greetings);
            body = body.Replace("{Position}", emailField?.Position);
            body = body.Replace("{Message}", emailField?.Message);
            body = body.Replace("{Title}", emailField?.Title);
            body = body.Replace("{Mail}", emailField?.Mail);
            body = body.Replace("{Link}", emailField?.Link);
            body = body.Replace("{Name}", emailField?.UserName);
            body = body.Replace("{urlblog}", emailField.Link);
            Logger.WriteLog($"HeaderrImageUrl -{emailField?.HeaderImageUrl}");
            body = body.Replace("{HeaderImage}", emailField?.HeaderImageUrl);
            body=body.Replace("{Password}", emailField?.Password);
            //  body = body.Replace("{BlogTitle}", emailField.Title);



            return body;
        }
        public class EmailField
        {
            public string UserName { get; set; }
            public string Password { get; set; } 
            public string Position { get; set; }
            public string Message { get; set; }
            public string Title { get; set; }
            public string Mail { get; set; }
            public string Link { get; set; }
            public string TemplatePath { get; set; }
            public string Subject { get; set; }
            public string ToMail { get; set; }
            public string ResumeUrl { get; set; }
            public string Body { get; set; }
            public string Greetings { get; set; }
            public string HeaderImageUrl { get; set; }

        }
        #endregion
        public static EmailField ExtractHeaderFromBody(string text)
        {
            var sb = new StringBuilder();
            var sr = new StringReader(text);
            var str = sr.ReadLine();
            var emailfeild = new EmailField();
            string strRes = "";

            while (str != null)
            {
                if (str.Contains("<h>"))
                {
                    //sb.AppendLine(str.Replace("<h>", "").Replace("</h>", "").Replace("</p>", "").Replace("<p>", ""));
                    emailfeild.Greetings = str.Replace("<h>", "").Replace("</h>", "").Replace("</p>", "").Replace("<p>", "");
                }
                else
                    sb.AppendLine(str);
                str = sr.ReadLine();
            }
            emailfeild.Body = "<p>" + sb.ToString();
            return emailfeild;
        }

        ///For sending email to user when creation time
        ///
        public static void SendUserEmail(EmailField emailField)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress("Addant Systems" + " <" + (string.IsNullOrWhiteSpace(emailField?.ToMail) ? ConfigurationManager.AppSettings["PrMailAddress"] : emailField?.ToMail) + ">"));
            message.To.Add(new MailAddress("New" + " <" + (string.IsNullOrWhiteSpace(emailField?.UserName) ? ConfigurationManager.AppSettings["PrMailAddress"] : emailField?.UserName) + ">"));

            message.From = new MailAddress("Addant Systems" + "<" + ConfigurationManager.AppSettings["PrMailAddress"] + ">");
            message.Subject = emailField?.Subject;
            message.Body = CreateEmailBodyFromTemplate(emailField);
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                smtp.EnableSsl = true;
                smtp.Send(message);
            }
        }



        /// <summary>
        /// otp mail 
        /// </summary>
        /// <param name="emailField"></param>
		public static void SendOTPEmail(EmailField emailField)
		{

            ///Generating OTP 
            ///
          
		//	string resumePath = HttpContext.Current.Server.MapPath("~/Uploads/Resume");

			var message = new MailMessage();
			message.To.Add(new MailAddress(emailField.UserName + " <" + (string.IsNullOrWhiteSpace(emailField?.ToMail) ? ConfigurationManager.AppSettings["PrMailAddress"] : emailField?.ToMail) + ">")); //Replace with HR mail addacomntenquiry@gmail.com 
			message.From = new MailAddress("Addant Systems" + "<" + ConfigurationManager.AppSettings["PrMailAddress"] + ">");
			message.Subject = emailField?.Subject;
            message.Body = "<P> Your OTP for reset Password is : </P>" + emailField?.Body;
			message.IsBodyHtml = true;
			using (var smtp = new SmtpClient())
			{
				smtp.EnableSsl = true;
				smtp.Send(message);
			}
		}

	}
}
