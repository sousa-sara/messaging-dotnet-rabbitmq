using System;

namespace Model
{
    public class UserMessage
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public DateTime RegistrationTimestamp { get; set; }
    }
}