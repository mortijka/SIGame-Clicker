using CommunityToolkit.Mvvm.ComponentModel;
using SIGame_Clicker.Tools;
using System.Windows.Input;

namespace SIGame_Clicker.Models
{
    public class Model : ObservableObject
    {
        private Key _key;
        private string _delayStr;

        public Model()
        {
            Key = Key.F1;
            KeyStr = Key.ToString();
            DelayStr = "1";
            InterceptKeys.targetKey = Key;
        }

        public int CursorX { get; set; }
        public int CursorY { get; set; }

        public Key Key
        {
            get => _key;
            set
            {
                _key = value;
                KeyStr = Key.ToString();
                InterceptKeys.targetKey = value;
            }
        }

        public string KeyStr { get; set; }
        public string DelayStr
        {
            get => _delayStr;
            set
            {
                if (int.TryParse(value.ToString(), out int result) && result > 0)
                {
                    _delayStr = result.ToString();
                    Delay = result;
                }
            }
        }

        public int Delay { get; set; }
    }
}
