using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

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

        public override Dictionary<string, EventCallback<MouseEventArgs>> GetContextMenuOptions()
        {
            return new Dictionary<string, EventCallback<MouseEventArgs>>
            {
                {
                    "log", EventCallback.Factory.Create<MouseEventArgs>(this, () => Console.WriteLine("log element " + Id))
                }
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
        }
    }
}