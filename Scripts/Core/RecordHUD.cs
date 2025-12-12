using Godot;
using System.Collections.Generic;

namespace GunRitualExorcistEdition.Scripts.Core
{
    public partial class RecordHUD : Control
    {
        [Export] private VBoxContainer _recordContainer;
        [Export] private PackedScene _recordScene;

        // Ключ = string, потому что PlayerManager.GetPlayers() возвращает string -> Player
        private readonly Dictionary<string, RecordPanel> _records = new();

        public override void _Ready()
        {
            Visible = false;
        }

        public override void _Process(double delta)
        {
            // Получаем текущих игроков
            var players = NetworkClient.Instance.PlayerManager.GetPlayers();

            // Добавление или обновление записей
            foreach (var kvp in players)
            {
                var id = kvp.Key;      // string
                var player = kvp.Value;

                if (!_records.ContainsKey(id))
                {
                    // Новый игрок – создаем панель
                    var record = _recordScene.Instantiate<RecordPanel>();
                    record.UpdateNickName(player.GetDisplayName());
                    _recordContainer.AddChild(record);

                    _records[id] = record;
                }
                else
                {
                    // Игрок уже есть – просто обновляем UI
                    _records[id].UpdateNickName(player.GetDisplayName());
                }
            }

            // Удаление игроков, которых больше нет
            var toRemove = new List<string>();

            foreach (var id in _records.Keys)
            {
                if (!players.ContainsKey(id))
                    toRemove.Add(id);
            }

            foreach (var id in toRemove)
            {
                _records[id].QueueFree();
                _records.Remove(id);
            }
        }

        public void HideUI()
        {
            Visible = false;
        }

        public void ShowUI()
        {
            Visible = true;
        }
    }
}
