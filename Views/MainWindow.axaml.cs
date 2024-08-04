using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using RequesterAvDesk.Models;
using RequesterAvDesk.ViewModels;
using System;
using System.Text.RegularExpressions;
using DialogHostAvalonia;
using Avalonia.Platform.Storage;
using Avalonia.Media;

namespace RequesterAvDesk.Views
{
    public partial class MainWindow : Window
    {
        private string cellOriginalText;
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();

            //��������� ���������� ��� �����, ������ ������ ��� ��� �������� �� ����� ��������� ��������
            AddHandler(DragDrop.DropEvent, OnDrop);
        }

        //
        //����������� ���������
        //
        private void OnCheckboxChecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var viewModel = DataContext as MainWindowViewModel;
                var dataContext = GetDataContext(checkBox);
                RequestModel req = dataContext as RequestModel;

                if (viewModel != null && req != null)
                {
                    if (ValidateRow(req))
                    {
                        viewModel.MoveRequest(req, true);
                        PageRefresh();
                    }
                    else
                    {
                        errDial.IsOpen = true;
                    }
                }
            }
        }

        private void OnCheckboxUnchecked(object sender, RoutedEventArgs e)
        {
            var checkBox = sender as CheckBox;
            if (checkBox != null)
            {
                var viewModel = DataContext as MainWindowViewModel;
                var dataContext = GetDataContext(checkBox);
                RequestModel req = dataContext as RequestModel;

                if (viewModel != null && req != null)
                {
                    viewModel.MoveRequest(req, false);

                    TabControl tabControl = this.FindControl<TabControl>("tabControl");
                    int currentIndex = tabControl.SelectedIndex;

                    if (currentIndex != 3)
                    {
                        PageRefresh();
                    }
                }
            }
        }

        //
        //����������� ������
        //
        private void OnButtonClick(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button != null)
            {
                var viewModel = DataContext as MainWindowViewModel;
                var dataContext = GetDataContext(button);
                RequestModel req = dataContext as RequestModel;
                viewModel?.GenerateCredentials(req);
                PageRefresh();
            }
        }

        //
        //����������� ����� ���������
        //
        private void OnCellEditBeginning(object sender, DataGridBeginningEditEventArgs e)
        {
            var dataGrid = sender as DataGrid;
            if (dataGrid != null && e.Column.Header.ToString() == "���")
            {
                var dataContext = GetDataContext(e.Column.GetCellContent(e.Row));
                if (dataContext != null)
                {
                    var property = dataContext.GetType().GetProperty("KPP");
                    if (property != null)
                    {
                        cellOriginalText = property.GetValue(dataContext)?.ToString();
                    }
                }
            }
        }
     
        //
        //��������� � VM ��� �������� ���� ��� ����������
        //
        private async void SaveXML(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var file = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "����������"
            });

            var viewModel = DataContext as MainWindowViewModel;
            try
            {
                viewModel.SaveXML(file[0].Path.LocalPath.ToString());
            } catch { }
            
        }

        private async void SaveXLSX(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var file = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "����������"
            });

            var viewModel = DataContext as MainWindowViewModel;
            try
            {
                viewModel.SaveXLSX(file[0].Path.LocalPath.ToString());
            } catch { }
        }

        //
        //��������� � VM ��� �������� ���� � �����
        //
        private async void ChooseFile(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "�������� �����",
                AllowMultiple = true
            });
            var viewModel = DataContext as MainWindowViewModel;
            viewModel.SelectPath(files);
        }

        //
        //����������� �������
        //

        //���������� ���������� ���������, �.�. � �������� ��� ������������ ����������� ������
        private void PageRefresh()
        {
            TabControl tabControl = this.FindControl<TabControl>("tabControl");
            int currentIndex = tabControl.SelectedIndex;

            if (currentIndex > 2)
            {
                tabControl.SelectedIndex = currentIndex - 1;
                tabControl.SelectedIndex = currentIndex;
            }
            else
            {
                tabControl.SelectedIndex = currentIndex + 1;
                tabControl.SelectedIndex = currentIndex;
            }
        }

        //��������� ��������� ������(�������, �� �����)
        private object GetDataContext(Control control)
        {
            //������� �� ���� ������� �������, � ���� ����� - ������, �� ���������� �������� ������ 
            var parent = control.Parent;
            while (parent != null)
            {
                if (parent is DataGridRow row)
                {
                    return row.DataContext;
                }
                parent = parent.Parent;
            }
            return null;
        }

        //�������� ����������� ����
        private async void CloseDialog(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            var dialogSession = DialogHost.GetDialogSession("dialogError");
            if (dialogSession != null)
            {
                dialogSession.Close(true);
            }
        }

        //
        //���������� ����� � �����
        //
        private async void CheckEditValidity(object sender, DataGridCellEditEndingEventArgs e)
        {
            bool IsKPPValid(string kpp) => Regex.IsMatch(kpp, @"^\d{9}$");
            bool IsDateValid(string date) => DateTime.TryParse(date, out _);
            bool IsINNValid(string inn) => Regex.IsMatch(inn, @"^\d{10,12}$");
            bool IsPhoneValid(string phone) => Regex.IsMatch(phone, @"^\d{5} \d{2}-\d{2}-\d{2}$");
            bool IsEmailValid(string email) => Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$");

            var dataGrid = sender as DataGrid;
            if (dataGrid != null)
            {
                var columnHeader = e.Column.Header.ToString();
                var textBox = e.EditingElement as TextBox;

                if (textBox != null)
                {
                    string inputText = textBox.Text;
                    bool isValid = true;

                    switch (columnHeader)
                    {
                        case "���":
                            isValid = IsKPPValid(inputText);
                            break;
                        case "���� ���������":
                        case "���� ������":
                            isValid = IsDateValid(inputText);
                            break;
                        case "���":
                            isValid = IsINNValid(inputText);
                            break;
                        case "�������":
                            isValid = IsPhoneValid(inputText);
                            break;
                        case "����������� �����":
                            isValid = IsEmailValid(inputText);
                            break;
                    }

                    if (!isValid)
                    {
                        errDial.IsOpen = true;
                        System.Diagnostics.Debug.WriteLine("������������ ����!");
                        textBox.Text = cellOriginalText;

                        textBox.Foreground = Brushes.Red;
                    }
                    else
                    {
                        textBox.Foreground = Brushes.Black;
                    }
                }
            }
        }

        private bool ValidateRow(RequestModel req)
        {
            bool IsKPPValid(string kpp) => Regex.IsMatch(kpp, @"^\d{9}$");
            bool IsDateValid(string date) => DateTime.TryParse(date, out _);
            bool IsINNValid(string inn) => Regex.IsMatch(inn, @"^\d{10,12}$");
            bool IsPhoneValid(string phone) => Regex.IsMatch(phone, @"^\d{5} \d{2}-\d{2}-\d{2}$");
            bool IsEmailValid(string email) => Regex.IsMatch(email, @"^[^@]+@[^@]+\.[^@]+$");

            bool isValid = true;

            if (!IsKPPValid(req.KPP))
            {
                isValid = false;
            }
            if (!IsDateValid(req.DateReceived))
            {
                isValid = false;
            }
            if (!IsINNValid(req.INN))
            {
                isValid = false;
            }
            if (!IsPhoneValid(req.Phone))
            {
                isValid = false;
            }
            if (!IsEmailValid(req.Email))
            {
                isValid = false;
            }

            return isValid;
        }

        private void OnDrop(object sender, DragEventArgs e)
        {
            var files = e.Data.GetFiles();
            var viewModel = DataContext as MainWindowViewModel;
            viewModel.SelectPath(files);

            e.Handled = true;
        }
    }
}