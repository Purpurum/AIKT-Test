using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Numerics;
using System.Xml.Linq;

namespace RequesterAvDesk.Models
{
    public class DataloaderXML
    {
        public string path { get; set; } = string.Empty;

        public List<RequestModel> LoadData()
        {
            List<RequestModel> requestsList = new List<RequestModel>();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(path);

            XmlNodeList items = xmlDoc.GetElementsByTagName("record");
            

            foreach (XmlNode item in items)
            {
                string Check(XmlNode node, string fieldName)
                {
                    var field = node[fieldName];
                    //Если поле не найдётся, то вставлю пустышку, в экселе больше полей, чем в xml'e
                    return field != null ? field.InnerText : string.Empty;
                }

                bool CheckIsClosed(string date)
                {
                    if (date == null || date == "")
                    {
                        System.Diagnostics.Debug.WriteLine(date);
                        return false;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine(date);
                        return true;
                    }
                }

                var requestType = Check(item, "тип_завки");
                var dateReceived = Check(item, "дата_получения");
                var dateResponded = Check(item, "дата_ответа");
                var FIO = Check(item, "фио");
                var INN = Check(item, "инн");
                var KPP = Check(item, "кпп");
                var institution = Check(item, "учреждение");
                var district = Check(item, "район");
                var department = Check(item, "отдел");
                var phone = Check(item, "телефон");
                var email = Check(item, "почта");
                var login = Check(item, "логин");
                var password = Check(item, "пароль");
                var comment = Check(item, "комментарий");
                var isClosed = CheckIsClosed(dateResponded);
                RequestModel request = new RequestModel(requestType, dateReceived, dateResponded, FIO, INN, KPP, institution, district, department, phone, email, login, password, comment, isClosed);

                requestsList.Add(request);
            }

            return requestsList;
        }

        public void SaveData(List<RequestModel> requests, string filePath)
        {
            void AppendElement(XmlDocument doc, XmlElement parent, string name, string value)
            {
                XmlElement element = doc.CreateElement(name);
                element.InnerText = value;
                parent.AppendChild(element);
            }

            XmlDocument doc = new XmlDocument();

            XmlElement root = doc.CreateElement("dataset");
            doc.AppendChild(root);

            foreach (var request in requests)
            {
                XmlElement requestElement = doc.CreateElement("record");

                AppendElement(doc, requestElement, "тип_завки", request.RequestType);
                AppendElement(doc, requestElement, "дата_получения", request.DateReceived);
                AppendElement(doc, requestElement, "дата_ответа", request.DateResponded);
                AppendElement(doc, requestElement, "фио", request.FullName);
                AppendElement(doc, requestElement, "инн", request.INN);
                AppendElement(doc, requestElement, "кпп", request.KPP);
                AppendElement(doc, requestElement, "учреждение", request.Institution);
                AppendElement(doc, requestElement, "район", request.District);
                AppendElement(doc, requestElement, "отдел", request.Department);
                AppendElement(doc, requestElement, "телефон", request.Phone);
                AppendElement(doc, requestElement, "почта", request.Email);
                AppendElement(doc, requestElement, "логин", request.Login);
                AppendElement(doc, requestElement, "пароль", request.Password);
                AppendElement(doc, requestElement, "комментарий", request.Comment);

                root.AppendChild(requestElement);
            }

            doc.Save(filePath);
        }
    }
}
