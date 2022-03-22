using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace EnviadorDeMail
{
    public class Sender : IMailSender
    {
        private readonly string origin;
        private readonly string replyTo;
        public Sender(IConfiguration configuration)
        {
            origin = configuration.GetSection("MailOrigin").Value;
            replyTo = configuration.GetSection("MailDestination").Value;
        }

        public Task<SendMailResponse> Send(string destination, string subject, string message)
        {
            var password = "QeOKxI44PJ";

            try
            {
                var t = Task.Run(() => {
                    MailMessage msg = new MailMessage(origin, destination, subject, message);
                    
                    msg.ReplyToList.Add(new MailAddress(replyTo, "SimpliSale"));

                    SmtpClient client = new SmtpClient("c2010380.ferozo.com")
                    {
                        Credentials = new NetworkCredential(origin, password)
                    };                   

                    client.Send(msg);
                    client.Dispose();
                                  
                });
                t.Wait();

                return Task.FromResult(new SendMailResponse()
                {
                    From = origin,
                    To = destination,
                    Time = DateTime.Now,
                    Result = string.Empty,
                    Success = true
                });
            }
            catch (Exception ex)
            {
                return Task.FromResult(new SendMailResponse()
                {
                    From = origin,
                    To = destination,
                    Time = DateTime.Now,
                    Result = ex.InnerException.Message,
                    Success = false
                });
            }
        }
    }
}
