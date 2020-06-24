using System.Collections.Generic;
using System.Linq;

namespace GameBoard.Data
{
    public delegate void ObjectDelegate(Player sender, CanvasElement element);
    
    public class Canvas
    {
        public event ObjectDelegate OnObjectMoved;
        public event ObjectDelegate OnObjectAdded;
        public event ObjectDelegate OnObjectRemoved;
        public event ObjectDelegate OnObjectSelected;
        public event ObjectDelegate OnObjectChanged;
        public readonly List<CanvasElement> Elements = new List<CanvasElement>();
        
        private int _currentId;

        public CanvasElement GetObject(int id)
        {
            return Elements.Find(e => e.Id == id);
        }
        
        public void MoveObject(Player player, CanvasMoveEvent data)
        {
            var idx = Elements.FindIndex(e => e.Id == data.Id);

            if (Elements[idx].Owner != player)
            {
                return;
            }

            Elements[idx].Position = data.Position;
            Elements[idx].Angle = data.Angle;
            
            OnObjectMoved?.Invoke(player, Elements[idx]);
        }

        public void AddObject(Player player, CanvasPosition position)
        {
            var element = new Card(this, _currentId++, position);
            Elements.Add(element);
            OnObjectAdded?.Invoke(player, element);
        }

        public void RemoveObject(int id)
        {
            var element = GetObject(id);
            Elements.Remove(element);
            OnObjectRemoved?.Invoke(null, element);
        }

        public void SelectObjects(Player player, IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var element = GetObject(id);
                
                if (element.Owner != player)
                {
                    element.Owner = player;
                    OnObjectSelected?.Invoke(player, element);
                }
            }
        }

        public void UpdateObject(CanvasElement element)
        {
            OnObjectChanged?.Invoke(null, element);
        }

        public void DoubleClick(Player player, int id)
        {
            var element = GetObject(id);
            
            if (element?.Owner == player)
            {
                element?.OnDoubleClick(this);
            }
        }

        public void ClearSelection(Player player)
        {
            SelectObjects(null, Elements.Where(e => e.Owner == player).Select(e => e.Id));
        }
    }
}