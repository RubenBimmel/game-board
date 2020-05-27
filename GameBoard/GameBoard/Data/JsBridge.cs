using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace GameBoard.Data
{
    public class JsBridge<T> : IDisposable where T : class
    {
        private readonly IJSRuntime jsRuntime;
        private DotNetObjectReference<T> objRef;
        
        public JsBridge(IJSRuntime jsRuntime)
        {
            this.jsRuntime = jsRuntime;
        }

        public ValueTask<T> Initialize(T reference)
        {
            objRef = DotNetObjectReference.Create(reference);

            return jsRuntime.InvokeAsync<T>(
                "initialize",
                objRef);
        }
        
        public void Dispose()
        {
            objRef?.Dispose();
        }
    }
}