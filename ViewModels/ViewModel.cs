using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Newtonsoft.Json;
using SIGame_Clicker.Models;
using SIGame_Clicker.Tools;
using SIGame_Clicker.Views;
using System.DirectoryServices;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using WindowsInput;

namespace SIGame_Clicker.ViewModels
{
    public class ViewModel : ObservableObject
    {
        public ViewModel(MainWindow win)
        {
            mainWin = win;
            SetPixelCommand = new RelayCommand(SetPixel);
            SetKeyCommand = new RelayCommand(SetKey);
            SaveSettingsCommand = new AsyncRelayCommand(SaveSettings);
            InterceptKeys.Start();
            InterceptKeys.KeyIntercepted += OnKeyIntercepted;
            LoadSettings();
        }

        // ************************* Команды *************************
        public RelayCommand SetPixelCommand { get; }
        public RelayCommand SetKeyCommand { get; }
        public AsyncRelayCommand SaveSettingsCommand { get; }

        // ************************* Приватные поля *************************

        private TransparentWindow? transpWin;
        private MainWindow mainWin;
        private InputSimulator inputSimulator = new InputSimulator();

        // ************************* Публичные свойства *************************

        public Model Model { get; set; } = new Model();
        public double BorderThickness { get; set; } = 0.1;
        public bool IsKeyChanging { get; set; } = false;
        public bool IsPixelChecking { get; set; }


        // ************************* Методы *************************

        //Сохранение настроек

        private async Task SaveSettings()
        {
            string modelStr = JsonConvert.SerializeObject(Model);
            await File.WriteAllTextAsync("settings.json", modelStr);
        }

        //Загрузка настроек

        private async Task LoadSettings()
        {
            if (File.Exists("settings.json"))
            {
                string modelStr = await File.ReadAllTextAsync("settings.json");
                JsonConvert.PopulateObject(modelStr, Model);
            }
        }

        // Проверка пикселя при нажатом хоткее
        private async void OnKeyIntercepted(object? sender, KeyInterceptedEventArgs e)
        {
            if (InterceptKeys.isKeyPressed && !IsPixelChecking)
            {
                IsPixelChecking = true;
                BorderThickness = 3;

                await Task.Run(async () =>
                {
                    while (InterceptKeys.isKeyPressed)
                    {
                        if (PixelChecker.IsPixelWhite(Model.CursorX, Model.CursorY))
                        {
                            await Task.Delay(Model.Delay);
                            inputSimulator.Keyboard.KeyPress(WindowsInput.Native.VirtualKeyCode.RCONTROL);
                        }
                        await Task.Delay(1);
                    }
                });

                IsPixelChecking = false;
                BorderThickness = 0.1;
            }
        }

        // Изменение клавиши для запуска скрипта
        private void SetKey()
        {
            InterceptKeys.Stop();
            mainWin.KeyDown += SetKeyOnKeyDown;
            IsKeyChanging = true;
            Model.KeyStr = "Нажмите клавишу";
        }

        // Сохранение клавиши для запуска скрипта
        private void SetKeyOnKeyDown(object sender, KeyEventArgs e)
        {
            InterceptKeys.Start();
            mainWin.KeyDown -= SetKeyOnKeyDown;
            IsKeyChanging = false;
            Model.Key = e.Key;
            Model.KeyStr = e.Key.ToString();
        }

        // Установка пикселя для отслеживания
        private void SetPixel()
        {
            transpWin = new TransparentWindow();
            transpWin.Width = SystemParameters.PrimaryScreenWidth;
            transpWin.Height = SystemParameters.PrimaryScreenHeight;

            transpWin.MouseMove += TranspWin_MouseMove;
            transpWin.MouseDown += TranspWin_MouseDown;

            transpWin.Show();
        }

        // Обработка отжания ЛКМ для сохранения координат пикселя для отслеживания
        private void TranspWin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            transpWin.MouseMove -= TranspWin_MouseMove;
            transpWin.MouseDown -= TranspWin_MouseDown;
            transpWin.Close();
        }

        // Обработка движения мыши для нахождения пикселя для отслеживания
        private void TranspWin_MouseMove(object sender, MouseEventArgs e)
        {
            var point = Mouse.GetPosition(transpWin);
            Model.CursorX = (int)point.X;
            Model.CursorY = (int)point.Y;

            // Очистка предыдущих линий на канвасе
            transpWin.canvas.Children.Clear();

            // Создание вертикальной линии
            Line verticalLine = new Line
            {
                X1 = point.X,
                Y1 = 0,
                X2 = point.X,
                Y2 = transpWin.Height,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                IsHitTestVisible = false
            };
            transpWin.canvas.Children.Add(verticalLine);

            // Создание горизонтальной линии
            Line horizontalLine = new Line
            {
                X1 = 0,
                Y1 = point.Y,
                X2 = transpWin.Width,
                Y2 = point.Y,
                Stroke = Brushes.Black,
                StrokeThickness = 1,
                IsHitTestVisible = false
            };
            transpWin.canvas.Children.Add(horizontalLine);

        }
    }
}
