using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace SIGame_Clicker.Views
{
    /// <summary>
    /// Логика взаимодействия для TransparentWindow.xaml
    /// </summary>
    public partial class TransparentWindow : Window
    {
        public TransparentWindow()
        {
            InitializeComponent();
        }

        #region Скрытие окна из альттаба

        private const int WS_EX_TOOLWINDOW = 0x00000080;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            // Получаем хэндл окна
            var hWnd = new System.Windows.Interop.WindowInteropHelper(this).Handle;

            // Получаем текущий стиль окна
            var windowStyle = GetWindowLong(hWnd, -20);

            // Изменяем стиль окна, исключая его из переключателя задач
            SetWindowLong(hWnd, -20, windowStyle | WS_EX_TOOLWINDOW);
        }

        #endregion
    }
}
