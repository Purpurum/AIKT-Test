using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;

namespace RequesterAvDesk.Models
{
    public class DataLoaderXLSX
    {
        public string path { get; set; } = string.Empty;

        public List<RequestModel> LoadData()
        {
            List<RequestModel> requestsList = new List<RequestModel>();

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

            FileInfo fileInfo = new FileInfo(path);
            using (ExcelPackage package = new ExcelPackage(fileInfo))
            {
                if (package.Workbook.Worksheets.Count == 1) {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string requestType = worksheet.Cells[row, 1].Text;
                        string dateReceived = worksheet.Cells[row, 2].Text;
                        string dateResponded = worksheet.Cells[row, 3].Text;
                        string FIO = worksheet.Cells[row, 4].Text;
                        string INN = worksheet.Cells[row, 5].Text;
                        string KPP = worksheet.Cells[row, 6].Text;
                        string institution = worksheet.Cells[row, 7].Text;
                        string district = worksheet.Cells[row, 8].Text;
                        string department = worksheet.Cells[row, 9].Text;
                        string phone = worksheet.Cells[row, 10].Text;
                        string email = worksheet.Cells[row, 11].Text;
                        string login = worksheet.Cells[row, 12].Text;
                        string password = worksheet.Cells[row, 13].Text;
                        string comment = worksheet.Cells[row, 14].Text;

                        RequestModel request = new RequestModel(
                            requestType, dateReceived, dateResponded, FIO, INN, KPP, institution,
                            district, department, phone, email, login, password, comment, CheckIsClosed(dateResponded));

                        requestsList.Add(request);
                    }
                }
                else
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

                    for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                    {
                        string requestType = worksheet.Cells[row, 1].Text;
                        string dateReceived = worksheet.Cells[row, 2].Text;
                        string dateResponded = worksheet.Cells[row, 3].Text;
                        string FIO = worksheet.Cells[row, 4].Text;
                        string INN = worksheet.Cells[row, 5].Text;
                        string KPP = worksheet.Cells[row, 6].Text;
                        string institution = worksheet.Cells[row, 7].Text;
                        string district = worksheet.Cells[row, 8].Text;
                        string department = worksheet.Cells[row, 9].Text;
                        string phone = worksheet.Cells[row, 10].Text;
                        string email = worksheet.Cells[row, 11].Text;
                        string login = worksheet.Cells[row, 12].Text;
                        string password = worksheet.Cells[row, 13].Text;
                        string comment = worksheet.Cells[row, 14].Text;

                        RequestModel request = new RequestModel(
                            requestType, dateReceived, dateResponded, FIO, INN, KPP, institution,
                            district, department, phone, email, login, password, comment, CheckIsClosed(dateResponded));

                        requestsList.Add(request);
                    }
                }
            }

            return requestsList;
        }
        public void SaveData(List<RequestModel> requestsListOpen, List<RequestModel> requestsListClosed, string filePath)
        {
            FileInfo fileInfo = new FileInfo(filePath);

            using (ExcelPackage package = new ExcelPackage())
            {
                ExcelWorksheet worksheet1 = package.Workbook.Worksheets.Add("Открытые заявки");
                ExcelWorksheet worksheet2 = package.Workbook.Worksheets.Add("Закрытые заявки");

                worksheet1.Cells[1, 1].Value = "Тип_завки";
                worksheet1.Cells[1, 2].Value = "Дата_получения";
                worksheet1.Cells[1, 3].Value = "Дата_ответа";
                worksheet1.Cells[1, 4].Value = "ФИО";
                worksheet1.Cells[1, 5].Value = "ИНН";
                worksheet1.Cells[1, 6].Value = "КПП";
                worksheet1.Cells[1, 7].Value = "Учреждение";
                worksheet1.Cells[1, 8].Value = "Район";
                worksheet1.Cells[1, 9].Value = "Отдел";
                worksheet1.Cells[1, 10].Value = "Телефон";
                worksheet1.Cells[1, 11].Value = "Почта";
                worksheet1.Cells[1, 12].Value = "Логин";
                worksheet1.Cells[1, 13].Value = "Пароль";
                worksheet1.Cells[1, 14].Value = "Комментарий";

                worksheet2.Cells[1, 1].Value = "Тип_завки";
                worksheet2.Cells[1, 2].Value = "Дата_получения";
                worksheet2.Cells[1, 3].Value = "Дата_ответа";
                worksheet2.Cells[1, 4].Value = "ФИО";
                worksheet2.Cells[1, 5].Value = "ИНН";
                worksheet2.Cells[1, 6].Value = "КПП";
                worksheet2.Cells[1, 7].Value = "Учреждение";
                worksheet2.Cells[1, 8].Value = "Район";
                worksheet2.Cells[1, 9].Value = "Отдел";
                worksheet2.Cells[1, 10].Value = "Телефон";
                worksheet2.Cells[1, 11].Value = "Почта";
                worksheet2.Cells[1, 12].Value = "Логин";
                worksheet2.Cells[1, 13].Value = "Пароль";
                worksheet2.Cells[1, 14].Value = "Комментарий";

                int row = 2;
                foreach (var request in requestsListOpen)
                {
                    worksheet1.Cells[row, 1].Value = request.RequestType;
                    worksheet1.Cells[row, 2].Value = request.DateReceived;
                    worksheet1.Cells[row, 3].Value = request.DateResponded;
                    worksheet1.Cells[row, 4].Value = request.FullName;
                    worksheet1.Cells[row, 5].Value = request.INN;
                    worksheet1.Cells[row, 6].Value = request.KPP;
                    worksheet1.Cells[row, 7].Value = request.Institution;
                    worksheet1.Cells[row, 8].Value = request.District;
                    worksheet1.Cells[row, 9].Value = request.Department;
                    worksheet1.Cells[row, 10].Value = request.Phone;
                    worksheet1.Cells[row, 11].Value = request.Email;
                    worksheet1.Cells[row, 12].Value = request.Login;
                    worksheet1.Cells[row, 13].Value = request.Password;
                    worksheet1.Cells[row, 14].Value = request.Comment;

                    row++;
                }

                row = 2;
                foreach (var request in requestsListClosed)
                {
                    worksheet2.Cells[row, 1].Value = request.RequestType;
                    worksheet2.Cells[row, 2].Value = request.DateReceived;
                    worksheet2.Cells[row, 3].Value = request.DateResponded;
                    worksheet2.Cells[row, 4].Value = request.FullName;
                    worksheet2.Cells[row, 5].Value = request.INN;
                    worksheet2.Cells[row, 6].Value = request.KPP;
                    worksheet2.Cells[row, 7].Value = request.Institution;
                    worksheet2.Cells[row, 8].Value = request.District;
                    worksheet2.Cells[row, 9].Value = request.Department;
                    worksheet2.Cells[row, 10].Value = request.Phone;
                    worksheet2.Cells[row, 11].Value = request.Email;
                    worksheet2.Cells[row, 12].Value = request.Login;
                    worksheet2.Cells[row, 13].Value = request.Password;
                    worksheet2.Cells[row, 14].Value = request.Comment;

                    row++;
                }

                package.SaveAs(fileInfo);
            }
        }
    }
}
