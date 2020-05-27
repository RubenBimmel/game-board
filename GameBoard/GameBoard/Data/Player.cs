using System;

namespace GameBoard.Data
{
    public class Player
    {
        public Guid Id { get; } = Guid.NewGuid();
        public PlayerEventHandler EventHandler { get; set; }
        public string Color { get; set; }
    }
}