using System;

namespace GameBoard.Data
{
    public class Card : CanvasElement
    {
        public bool FaceUp { get; set; }
        public string Value { get; set; }
        
        public Card(int id, CanvasPosition position, string value = null)
        {
            Id = id;
            Position = position;
            Image = DeckOfCards.BackUrl;
            FaceUp = false;
            Value = value ?? DeckOfCards.GetRandomCard();
        }

        public override void OnDoubleClick(Canvas canvas)
        {
            Flip();
            canvas.UpdateObject(this);
        }

        private void Flip()
        {
            SetFace(!FaceUp);
        }

        private void SetFace(bool up)
        {
            FaceUp = up;
            Image = FaceUp ? DeckOfCards.GetUrl(Value) : DeckOfCards.BackUrl;
        }
    }
}