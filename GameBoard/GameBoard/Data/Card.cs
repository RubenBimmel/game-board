namespace GameBoard.Data
{
    public class Card : CanvasElement
    {
        public Card(int id, CanvasPosition position)
        {
            Id = id;
            Image = DeckOfCards.GetRandomUrl();
            Position = position;
        }
    }
}