using Godot;

namespace GunRitualExorcistEdition.Scripts.Core;

public partial class SettingsHUD : Control
    {
        [Export] private Button _saveButton;
        [Export] private Button _leaveButton;
        [Export] private LineEdit _nameInput;
        [Export] private LineEdit _ipInput;
        [Export] private Label _errorLabel;

        public override void _Ready()
        {
            Visible = false;
            _saveButton.Pressed += OnSavePressed;
            _leaveButton.Pressed += OnLeavePressed;
            _errorLabel.Text = "";
        }

        public void ShowUI()
        {
            Visible = true;
            if (!string.IsNullOrEmpty(NetworkClient.Instance.UserName))
                _nameInput.Text = NetworkClient.Instance.UserName;

            _ipInput.Text = ""; // можно оставить пустым
            _errorLabel.Text = "";
        }

        private void OnSavePressed()
        {
            var name = _nameInput.Text.Trim();
            var ip = _ipInput.Text.Trim();

            if (string.IsNullOrEmpty(name))
            {
                _errorLabel.Text = "Введите имя игрока!";
                return;
            }

            if (string.IsNullOrEmpty(ip))
            {
                _errorLabel.Text = "Введите IP сервера!";
                return;
            }

            NetworkClient.Instance.UserName = name;

            // Простая проверка IPv4
            if (!IsValidIp(ip))
            {
                _errorLabel.Text = "Неверный формат IP!";
                return;
            }

            var url = $"ws://{ip}:8080/ws";
            bool connected = NetworkClient.Instance.ConnectToServer(url);

            if (connected)
            {
                GetTree().ChangeSceneToFile("res://Scenes/GameScene.tscn");
                AudioManager.Instance.StopMusic();
            }
            else
            {
                _errorLabel.Text = "Не удалось подключиться к серверу!";
            }
        }

        private bool IsValidIp(string ip)
        {
            var parts = ip.Split('.');
            if (parts.Length != 4) return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int n)) return false;
                if (n < 0 || n > 255) return false;
            }

            return true;
        }

        private void OnLeavePressed()
        {
            Visible = false;
            _errorLabel.Text = "";
        }
    }