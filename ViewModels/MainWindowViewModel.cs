using RequesterAvDesk.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace RequesterAvDesk.ViewModels
{
    public partial class MainWindowViewModel : ViewModelBase
    {
        public string DropText => "Или перенесите сюда файл";
        DataLoaderXLSX dataLoaderXLSX = new DataLoaderXLSX();
        DataloaderXML dataLoaderXML = new DataloaderXML();

        private Random random = new Random();
        private HashSet<string> existingLogins = new HashSet<string>();
        private HashSet<string> existingPasswords = new HashSet<string>();

        private ObservableCollection<RequestModel> _allrequests;
        private ObservableCollection<RequestModel> _openrequests;
        private ObservableCollection<RequestModel> _closedrequests;
        private ObservableCollection<RequestModel> _filteredRequests;
        private string _searchText;
        private string _selectedField;
        ///
        ///Список полей для поиска
        ///
        private string[] _searchFields =
        {
            "Выберите поле",
            "ФИО",
            "Тип запроса",
            "Дата получения",
            "Дата ответа",
            "ИНН",
            "КПП",
            "Учреждение",
            "Район",
            "Отдел",
            "Телефон",
            "Почта",
            "Логин",
            "Пароль",
            "Комментарий"
        };
        ///
        ///Словарь, используемый при реализации поиска
        ///Ключ - Из поля поиска
        ///Значение - Параметр класса
        ///
        private Dictionary<string, string> _searchFieldsDict = new Dictionary<string, string>
    {
        { "ФИО", "FullName" },
        { "Тип запроса", "RequestType" },
        { "Дата получения", "DateReceived" },
        { "Дата ответа", "DateResponded" },
        { "ИНН", "INN" },
        { "КПП", "KPP" },
        { "Учреждение", "Institution" },
        { "Район", "District" },
        { "Отдел", "Department" },
        { "Телефон", "Phone" },
        { "Почта", "Email" },
        { "Логин", "Login" },
        { "Пароль", "Password" },
        { "Комментарий", "Comment" }
    };
        ///
        ///Создание наблюдателей для каждой коллекции заявок
        ///
        public ObservableCollection<RequestModel> AllRequests
        {
            get { return _allrequests; }
            set
            {
                _allrequests = value;
                OnPropertyChanged(nameof(AllRequests));
            }
        }

        public ObservableCollection<RequestModel> OpenRequests
        {
            get { return _openrequests; }
            set
            {
                _openrequests = value;
                OnPropertyChanged(nameof(OpenRequests));
            }
        }

        public ObservableCollection<RequestModel> ClosedRequests
        {
            get { return _closedrequests; }
            set
            {
                _closedrequests = value;
                OnPropertyChanged(nameof(ClosedRequests));
            }
        }
        public ObservableCollection<RequestModel> FilteredRequests
        {
            get { return _filteredRequests; }
            set
            {
                _filteredRequests = value;
                OnPropertyChanged(nameof(FilteredRequests));
            }
        }

        ///
        ///Создание глобальных (в рамках класса) переменных для поиска и добавление им триггера
        ///
        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterRequests();
            }
        }
        public string SelectedField
        {
            get => _selectedField;
            set
            {
                _selectedField = value;
                OnPropertyChanged(nameof(SelectedField));
                FilterRequests();
            }
        }

        public string[] SearchFields => _searchFields;

        public MainWindowViewModel()
        {
            AllRequests = new ObservableCollection<RequestModel>();
            OpenRequests = new ObservableCollection<RequestModel>();
            ClosedRequests = new ObservableCollection<RequestModel>();
            FilteredRequests = new ObservableCollection<RequestModel>();
            _selectedField = _searchFields.First();

        }

        ///
        ///Метод для сохранения xml
        ///
        public void SaveXML(string path)
        {
            dataLoaderXML.SaveData(OpenRequests.ToList(), path + "/OpenedRequests.xml");
            dataLoaderXML.SaveData(ClosedRequests.ToList(), path + "/ClosedRequests.xml");
        }

        ///
        ///Метод для сохранения xlsx
        ///
        public void SaveXLSX(string path)
        {
            dataLoaderXLSX.SaveData(OpenRequests.ToList(), ClosedRequests.ToList(), path + "/Requests.xlsx");
        }

        ///
        ///Загрузчик данных в наблюдаемые колекции
        ///
        public void SelectPath(IEnumerable<Avalonia.Platform.Storage.IStorageItem> filePaths)
        {
            foreach(var filePath in filePaths)
            {
                System.Diagnostics.Debug.WriteLine(filePath.Path.LocalPath);
                if (filePath.Path.LocalPath.EndsWith(".xml"))
                {
                    System.Diagnostics.Debug.WriteLine("XML is loaded");
                    dataLoaderXML.path = filePath.Path.LocalPath;
                    var requests = dataLoaderXML.LoadData();

                    foreach(var request in requests)
                    {
                        AllRequests.Add(request);

                        if (!existingPasswords.Contains(request.Password)){
                            existingPasswords.Add(request.Password);
                        }

                        if (!existingLogins.Contains(request.Login))
                        {
                            existingLogins.Add(request.Login);
                        }

                        if (request.IsClosed)
                            ClosedRequests.Add(request);
                        else
                            OpenRequests.Add(request);
                            
                    }

                }

                if (filePath.Path.LocalPath.EndsWith(".xlsx"))
                {
                    System.Diagnostics.Debug.WriteLine("XLSX is loaded");
                    dataLoaderXLSX.path = filePath.Path.LocalPath;
                    var requests = dataLoaderXLSX.LoadData();

                    foreach (var request in requests)
                    {
                        AllRequests.Add(request);

                        if (!existingPasswords.Contains(request.Password))
                        {
                            existingPasswords.Add(request.Password);
                        }

                        if (!existingLogins.Contains(request.Login))
                        {
                            existingLogins.Add(request.Login);
                        }

                        if (request.IsClosed)
                            ClosedRequests.Add(request);
                        else
                            OpenRequests.Add(request);

                    }
                }
            }

        }

        ///
        ///Метод для фильтрации заявок для поиска
        ///
        private void FilterRequests()
        {
            _filteredRequests.Clear();
            if (_selectedField != "Выберите поле")
            {
                var filtered = _allrequests.Where(r =>
                {
                    if (string.IsNullOrEmpty(_searchText)) return true;
                    System.Diagnostics.Debug.WriteLine(_selectedField);
                    var trueField = _searchFieldsDict[_selectedField];
                    var property = typeof(RequestModel).GetProperty(trueField);
                    if (property == null) return false;

                    var value = property.GetValue(r)?.ToString();
                    //value = _searchFieldsDict[value];
                    return value != null && value.Contains(_searchText, StringComparison.OrdinalIgnoreCase);

                });

                foreach (var request in filtered)
                {
                    _filteredRequests.Add(request);
                }
            }
        }

        ///
        ///Метод для перемещения бъектов из одной коллекции в другую
        ///Используется при отметке обработанных заявок 
        ///
        public void MoveRequest(RequestModel req, bool isClosed)
        {
            if (isClosed)
            {
                OpenRequests.Remove(req);
                if (!ClosedRequests.Contains(req))
                {
                    req.IsClosed = true;
                    req.DateResponded = DateTime.Now.ToString("yyyy.MM.dd HH:mm:ss");
                    ClosedRequests.Add(req);
                    
                }
            }
            else
            {
                ClosedRequests.Remove(req);
                if (!OpenRequests.Contains(req))
                {
                    req.IsClosed = false;
                    req.DateResponded = "";
                    OpenRequests.Add(req);
                    
                }
            }
        }

        ///
        ///Метод для генерации логина и пароля
        ///
        public void GenerateCredentials(RequestModel req) {
            string GenerateUniqueLogin()
            {
                const string chars = "abcdefghijklmnopqrstuvwxyz1234567890";
                string newLogin;

                do
                {
                    newLogin = new string(Enumerable.Repeat(chars, 6)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                    newLogin = "user_" + newLogin;
                } while (existingLogins.Contains(newLogin));

                existingLogins.Add(newLogin);
                return newLogin;
            }

            string GenerateUniquePassword()
            {
                const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!@#$%^&*()";
                string newPassword;


                do
                {
                    newPassword = new string(Enumerable.Repeat(chars, 14)
                        .Select(s => s[random.Next(s.Length)]).ToArray());
                } while (existingPasswords.Contains(newPassword));

                existingPasswords.Add(newPassword);
                return newPassword;
            }


            string login = GenerateUniqueLogin();
            string password = GenerateUniquePassword();

            req.Password = password;
            req.Login = login;

            OnPropertyChanged(nameof(RequestModel));
        }
    }
}
