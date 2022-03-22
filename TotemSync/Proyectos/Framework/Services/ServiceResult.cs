using static Framework.Enums.Enumerations;

namespace Framework.Services
{
    public class ServiceResult
    {
        string _message = "";

        public ServiceMethodsStatusCode StatusCode { get; set; }
        public string Method { get; set; }
        public string Message
        {
            get
            {
                return _message.Replace("One or more errors occurred.", "").Trim();
            }
            set
            {
                _message = value;
            }
        }
        public string Notes { get; set; }
        public bool HasErrors { get; set; }
        public long IdObjeto { get; set; }

        public string Alert
        {
            get
            {
                if (Message == null)
                    return Notes;
                else
                    return Message.Trim() + ((Message.Trim().EndsWith(".")) ? " " : ". ") + Notes;
            }
        }
    }
}
