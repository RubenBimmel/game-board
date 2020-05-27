namespace GameBoard.Data
{
    public class CanvasElement
    {
        public int Id { get; set; }
        public CanvasPosition Position { get; set; }
        public float Angle { get; set; }
        public Player Owner { get; set; }
    }
}