using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace AuditLogManager.Sample.AspNetCore.Utilities;

public class AmbientContext
{
    private readonly ConcurrentDictionary<string, AsyncLocal<object?>> _contexts = new(StringComparer.Ordinal);

    public void Set<T>(string key, [MaybeNull] T val)
    {
        AsyncLocal<object?> keyctx = _contexts.AddOrUpdate(
            key,
            k => new AsyncLocal<object?>(),
            (k, al) => al);
        keyctx.Value = (object?)val;
    }

    [return: MaybeNull]
    public T Get<T>(string key)
    {
        return _contexts.TryGetValue(key, out AsyncLocal<object?>? keyctx)
            ? (T)(keyctx!.Value ?? default(T)!)
            : default(T);
    }
}