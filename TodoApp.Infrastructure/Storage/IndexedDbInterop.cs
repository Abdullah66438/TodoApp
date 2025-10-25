using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace TodoApp.Infrastructure.Storage
{
    /// <summary>Thin C# wrapper around the JS indexedDB helper in wwwroot/js/indexedDb.js</summary>
    public sealed class IndexedDbInterop
    {
        private readonly IJSRuntime _js;

        public IndexedDbInterop(IJSRuntime js) => _js = js;

        public ValueTask AddAsync(string store, object item)
            => _js.InvokeVoidAsync("todoIdb.add", store, item);

        public ValueTask<T?> GetAsync<T>(string store, Guid id)
            => _js.InvokeAsync<T?>("todoIdb.get", store, id);

        public ValueTask<T[]> ListAsync<T>(string store)
            => _js.InvokeAsync<T[]>("todoIdb.list", store);

        public ValueTask DeleteAsync(string store, Guid id)
            => _js.InvokeVoidAsync("todoIdb.delete", store, id);

        public ValueTask<T[]> ListTasksByAssigneeAsync<T>(Guid assigneeId)
            => _js.InvokeAsync<T[]>("todoIdb.listTasksByAssignee", assigneeId);
    }
}
