using System.Collections.Generic;
using System.Linq;

namespace GameBoard.Data
{
    public class GameService
    {
        public Canvas Canvas { get; } = new Canvas();
        public List<Player> Players { get; } = new List<Player>();

        private readonly string[] _colors = {"red", "blue", "green", "yellow", "orange", "purple"};
        
        public Player AddPlayer(PlayerEventHandler eventHandler)
        {
            var player = new Player
            {
                EventHandler = eventHandler,
                Color = GetUnusedColor()
            };
            Players.Add(player);
            return player;
        }

        private string GetUnusedColor()
        {
            foreach (var color in _colors)
            {
                if (Players.All(p => p.Color != color))
                {
                    return color;
                }
            }

            return "black";
        }
    }
}