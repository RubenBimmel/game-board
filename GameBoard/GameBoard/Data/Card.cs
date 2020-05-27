namespace GameBoard.Data
{
    public static class Card
    {
        public static CanvasElement GetCard(int id, CanvasPosition position)
        {
            return new CanvasElement
            {
                Id = id,
                Image = DeckOfCards.GetRandomUrl(),
                Position = position
            };
        }
    }
}