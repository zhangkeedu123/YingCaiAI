using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace YingCaiAiWin.Models
{
    public class AppUser : INotifyPropertyChanged
    {
        private static AppUser _instance;
        public static AppUser Instance => _instance ??= new AppUser();

        private string _username;
        private string _token;
        private string _role;
        private int _coId;

        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        public string Token
        {
            get => _token;
            set { _token = value; OnPropertyChanged(); }
        }

        public string Role
        {
            get => _role;
            set { _role = value; OnPropertyChanged(); }
        }
        public int CoId
        {
            get => _coId;
            set { _coId = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public void Clear()
        {
            
            Username = string.Empty;
            Token = string.Empty;
            Role = string.Empty;
        }
    }

}
