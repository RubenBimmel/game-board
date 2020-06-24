using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace GameBoard.Data
{
    public delegate void ContextMenuDelegate(CanvasPosition position, Dictionary<string, EventCallback<MouseEventArgs>> options);
    
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
            _gameService.Canvas.OnObjectSelected -= SelectObject;
            _gameService.Canvas.OnObjectChanged -= UpdateObject;
            _gameService.Canvas.ClearSelection(_player);
        }

        [JSInvokable]
        public void OnMove(CanvasElement element)
        {
            _gameService.Canvas.MoveObject(_player, element);
        }
        
        [JSInvokable]
        public void OnAdd(CanvasPosition position)
        {
            _gameService.Canvas.AddObject(_player, position);
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
            OnContextMenuOpen?.Invoke(position, new Dictionary<string, EventCallback<MouseEventArgs>>
            {
                { "log", EventCallback.Factory.Create<MouseEventArgs>(this, () =>  Console.WriteLine("log canvas"))}
            });
        }
        
        [JSInvokable]
        public void OnRightClickElement(int id, CanvasPosition position)
        {
            OnContextMenuOpen?.Invoke(position, _gameService.Canvas.GetObject(id).GetContextMenuOptions());
        }
        
        [JSInvokable]
        public void OnRightClickMultiple(int[] ids, CanvasPosition position)
        {
            OnContextMenuOpen?.Invoke(position, new Dictionary<string, EventCallback<MouseEventArgs>>
            {
                { "log", EventCallback.Factory.Create<MouseEventArgs>(this, () =>  Console.WriteLine("log multiple"))}
            });
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