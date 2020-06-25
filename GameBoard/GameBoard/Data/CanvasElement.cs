using System;
using System.Collections.Generic;

namespace GameBoard.Data
{
    public abstract class CanvasElement
    {
        public Canvas Canvas { get; set; }
        public int Id { get; set; }
        public string DisplayName { get; set; }
        public string Image { get; set; }
        public CanvasPosition Position { get; set; }
        public float Angle { get; set; }
        public Player Owner { get; set; }

        public abstract void OnDoubleClick(Canvas canvas);
        public abstract Dictionary<string, Action> GetContextMenuOptions();

        protected void AddToHand()
        {
            //Owner.Hand.Add(this);
            Console.WriteLine("Add to hand");
            Canvas.RemoveObject(Id);
        }
        
        protected void Remove()
        {
            Canvas.RemoveObject(Id);
        }
    }
}