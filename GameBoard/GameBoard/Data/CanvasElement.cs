using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace GameBoard.Data
{
    public abstract class CanvasElement
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public CanvasPosition Position { get; set; }
        public float Angle { get; set; }
        public Player Owner { get; set; }

        public abstract void OnDoubleClick(Canvas canvas);
        public abstract Dictionary<string, EventCallback<MouseEventArgs>> GetContextMenuOptions();
    }
}