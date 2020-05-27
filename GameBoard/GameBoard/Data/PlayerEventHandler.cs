using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace GameBoard.Data
{
    public class PlayerEventHandler : IDisposable
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly GameService _gameService;
        
        private Player _player;
        private DotNetObjectReference<PlayerEventHandler> _objRef;
        
        
        public PlayerEventHandler (IJSRuntime runtime, GameService service)
        {
            _jsRuntime = runtime;
            _gameService = service;
        }

        public async Task Initialize() {
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

            foreach (var element in _gameService.Canvas.Elements)
            {
                AddObject(null, element);
            }
        }
        
        public void Dispose()
        {
            _jsRuntime.InvokeVoidAsync("dispose");
            _objRef?.Dispose();
            _gameService.Canvas.OnObjectMoved -= MoveObject;
            _gameService.Canvas.OnObjectAdded -= AddObject;
            _gameService.Canvas.OnObjectSelected -= SelectObject;
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
            Console.WriteLine(id);
        }

        private void MoveObject(Player player, CanvasElement element)
        {
            if (_player == player) return;
            _jsRuntime.InvokeVoidAsync("moveObject", element);
        }
        
        private void AddObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("addObject", element);
        }

        private void SelectObject(Player player, CanvasElement element)
        {
            _jsRuntime.InvokeVoidAsync("selectObject", element, _player == player);
        }
    }
}