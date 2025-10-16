namespace GameVaultWeb.Services
{
    public class UserState
    {
        private bool _isLoggedIn;
        private string? _username;
        private string _role = "Guest";

        public bool IsLoggedIn
        {
            get => _isLoggedIn;
            set
            {
                _isLoggedIn = value;
                StateChanged?.Invoke();
            }
        }

        public string? Username
        {
            get => _username;
            set
            {
                _username = value;
                StateChanged?.Invoke();
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value;
                StateChanged?.Invoke();
            }
        }

        public event Action? StateChanged;
    }
}