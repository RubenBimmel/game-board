using System;
using System.Collections.Generic;

namespace GameBoard.Data
{
    public class Card : CanvasElement
    {
        public bool FaceUp { get; set; }
        public string Value { get; set; }
        
        public Card(Canvas canvas, int id, CanvasPosition position, string value = null)
        {
            Canvas = canvas;
            Id = id;
            Position = position;
            DisplayName = "Card";
            Image = DeckOfCards.BackUrl;
            FaceUp = false;
            Value = value ?? DeckOfCards.GetRandomCard();
        }

        public override void OnDoubleClick(Canvas canvas)
        {
            Flip();
        }

        public override Dictionary<string, Action> GetContextMenuOptions()
        {
            return new Dictionary<string, Action>
            {
                { "Flip", Flip },
                { "Remove", Remove }
            };
        }

        private void Flip()
        {
            SetFace(!FaceUp);
        }

        private void SetFace(bool up)
        {
            FaceUp = up;
            Image = FaceUp ? DeckOfCards.GetUrl(Value) : DeckOfCards.BackUrl;
            Canvas.UpdateObject(this);
        }

        private void Remove()
        {
            Canvas.RemoveObject(Id);
        }
    }
}