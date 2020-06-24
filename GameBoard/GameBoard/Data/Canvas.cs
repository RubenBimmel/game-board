using System.Collections.Generic;
using System.Linq;

namespace GameBoard.Data
{
    public delegate void ObjectDelegate(Player sender, CanvasElement element);
    
    public class Canvas
    {
        public event ObjectDelegate OnObjectMoved;
        public event ObjectDelegate OnObjectAdded;
        public event ObjectDelegate OnObjectSelected;
        public event ObjectDelegate OnObjectChanged;
        public readonly List<CanvasElement> Elements = new List<CanvasElement>();
        
        private int _currentId;

        public void MoveObject(Player player, CanvasElement element)
        {
            var idx = Elements.FindIndex(e => e.Id == element.Id);

            if (Elements[idx].Owner != player)
            {
                return;
            }

            Elements[idx].Position = element.Position;
            Elements[idx].Angle = element.Angle;
            
            OnObjectMoved?.Invoke(player, Elements[idx]);
        }

        public void AddObject(Player player, CanvasPosition position)
        {
            var element = new Card(_currentId++, position);
            Elements.Add(element);
            OnObjectAdded?.Invoke(player, element);
        }

        public void SelectObjects(Player player, IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var idx = Elements.FindIndex(e => e.Id == id);
                
                if (Elements[idx].Owner != player)
                {
                    Elements[idx].Owner = player;
                    OnObjectSelected?.Invoke(player, Elements[idx]);
                }
            }
        }

        public void UpdateObject(CanvasElement element)
        {
            OnObjectChanged?.Invoke(null, element);
        }

        public void DoubleClick(Player player, int id)
        {
            var element = Elements.Find(e => e.Id == id);
            
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