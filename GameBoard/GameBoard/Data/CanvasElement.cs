namespace GameBoard.Data
{
    public class CanvasElement
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public CanvasPosition Position { get; set; }
        public float Angle { get; set; }
        public Player Owner { get; set; }

        public virtual void OnDoubleClick(Canvas canvas) { }
    }
}