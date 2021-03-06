using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace GameBoard.Data
{
    public delegate void ContextMenuDelegate(CanvasPosition position, string title,  Dictionary<string, Action> options);
    
    public class PlayerEventHandler : IDisposable
    {
        public ContextMenuDelegate OnContextMenuOpen;
        public Action OnContextMenuClose;

        private readonly IJSRuntime _jsRuntime;
        private readonly GameService _gameService;
        
        private Player _player;
        private DotNetObjectReference<PlayerEventHandler> _objRef;
        
        
        public PlayerEventHandler (IJSRuntime runtime, GameService service)
        {
            _jsRuntime = runtime;
            _gameService = service;
        }

        public async Task<Guid> Initialize() {
            var id = await _jsRuntime.InvokeAsync<string>(
                "getPlayerId");

            if (!string.IsNullOrEmpty(id))
            {
                _player = _gameService.ConnectPlayer(this, id);
            }

            if (_player == null)
            {
                _player = _gameService.AddPlayer(this);
                await _jsRuntime.InvokeVoidAsync(
                    "setPlayerId",
                    _player.Id.ToString());
            }

            _objRef = DotNetObjectReference.Create(this);
            await _jsRuntime.InvokeVoidAsync(
                "initialize",
                _objRef);
            
            _gameService.Canvas.OnObjectMoved += MoveObject;
            _gameService.Canvas.OnObjectAdded += AddObject;
            _gameService.Canvas.OnObjectRemoved += RemoveObject;
            _gameService.Canvas.OnObjectSelected += SelectObject;
            _gameService.Canvas.OnObjectChanged += UpdateObject;

            foreach (var element in _gameService.Canvas.Elements)
            {
                AddObject(null, element);
            }

            return _player.Id;
        }
        
        public void Dispose()
        {
            _jsRuntime.InvokeVoidAsync("dispose");
            _objRef?.Dispose();
            _gameService.Canvas.OnObjectMoved -= MoveObject;
            _gameService.Canvas.OnObjectAdded -= AddObject;
            _gameService.Canvas.OnObjectRemoved -= RemoveObject;
            _gameService.Canvas.OnObjectSelected -= SelectObject;
            _gameService.Canvas.OnObjectChanged -= UpdateObject;
            _gameService.Canvas.ClearSelection(_player);
        }

        [JSInvokable]
        public void OnMove(CanvasMoveEvent data)
        {
            _gameService.Canvas.MoveObject(_player, data);
        }

        [JSInvokable]
        public void OnSelect(int[] ids)
        {
            if (ids.Length == 0) return;
            _gameService.Canvas.SelectObjects(_player, ids);
        }
        
        [JSInvokable]
        public void OnDeselect(int[] ids)
        {
            if (ids.Length == 0) return;
            _gameService.Canvas.SelectObjects(null, ids);
        }
        
        [JSInvokable]
        public void OnDoubleClick(int id)
        {
            _gameService.Canvas.DoubleClick(_player, id);
        }
        
        [JSInvokable]
        public void OnRightClick(CanvasPosition position)
        {
            OnContextMenuOpen?.Invoke(position, "Canvas", new Dictionary<string, Action>
            {
                { "Add card", () => _gameService.Canvas.AddObject(_player, position) }
            });
        }
        
        [JSInvokable]
        public void OnRightClickElement(int id, CanvasPosition position)
        {
            var element = _gameService.Canvas.GetObject(id);
            OnContextMenuOpen?.Invoke(position, element.DisplayName, element.GetContextMenuOptions());
        }
        
        [JSInvokable]
        public void OnRightClickMultiple(int[] ids, CanvasPosition position)
        {
            OnContextMenuOpen?.Invoke(position, "Multiple", new Dictionary<string, Action>());
        }
        
        [JSInvokable]
        public void CloseContextMenu()
        {
            OnContextMenuClose?.Invoke();
        }

        private void MoveObject(Player player, CanvasElement element)
        {
            if (_player == player) return;
            _jsRuntime.InvokeVoidAsync("moveObject", element);
        }
        
        private void AddObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("addObject", element, element.Owner == _player);
        }
        
        private void RemoveObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("removeObject", element.Id);
        }

        private void SelectObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("selectObject", element, player == _player);
        }

        private void UpdateObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("updateObject", element);
        }
    }
}