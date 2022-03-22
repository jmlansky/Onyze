using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace EnviadorDeMail
{
    public interface IMailSender
    {
        Task<SendMailResponse> Send(string mailDestino, string asunto, string mensaje);
    }
}
