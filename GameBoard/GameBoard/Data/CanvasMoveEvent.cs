namespace GameBoard.Data
{
    public class CanvasMoveEvent
    {
        public int Id { get; set; }
        public CanvasPosition Position { get; set; }
        public float Angle { get; set; }
    }
}