using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RequesterAvDesk.Models
{
    public class RequestModel(string requestType, string dateReceived, string dateResponded, string fullName, string inn, string kpp, string institution, string district, string department, string phone, string email, string login, string password, string comment, bool isClosed)
    {
        public string RequestType { get; set; } = requestType;
        public string DateReceived { get; set; } = dateReceived;
        public string DateResponded { get; set; } = dateResponded;
        public string FullName { get; set; } = fullName;
        public string INN { get; set; } = inn;
        public string KPP { get; set; } = kpp;
        public string Institution { get; set; } = institution;
        public string District { get; set; } = district;
        public string Department { get; set; } = department;
        public string Phone { get; set; } = phone;
        public string Email { get; set; } = email;
        public string Login { get; set; } = login;
        public string Password { get; set; } = password;
        public string Comment { get; set; } = comment;
        public bool IsClosed { get; set; } = isClosed;
    }
}
