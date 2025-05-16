using System.Numerics;
using HoudiniEngineCSharp;

namespace HoudiniEngine.NET;

public interface IHoudiniParameter
{
    string Name { get; }
    int Size { get; }
    
    HAPI_ParmInfo Info { get; }
}

public abstract record HoudiniParameter<T> : IHoudiniParameter
{
    public string Name { get; }
    public int Size => _buffer.Length;
    
    public HAPI_ParmInfo Info { get; }

    private readonly T[] _buffer;

    protected HoudiniParameter(HAPI_ParmInfo info, string name, params ReadOnlySpan<T> values)
    {
        Name = name;
        _buffer = values.ToArray();
        Info = info;
    }

    public T this[int index]
    {
        get => _buffer[index];
        set => _buffer[index] = value;
    }

    public override string ToString()
    {
        return $"[{typeof(T).Name}_{Size}]: {string.Join(',', _buffer)}";
    }
}

public record HoudiniParameterInt : HoudiniParameter<int>
{
    public HoudiniParameterInt(HAPI_ParmInfo info, string name, params ReadOnlySpan<int> values) : base(info, name, values) { }
    public override string ToString()
    {
        return base.ToString();
    }
}

public record HoudiniParameterBool :  HoudiniParameter<bool>
{
    public HoudiniParameterBool(HAPI_ParmInfo info, string name, params ReadOnlySpan<bool> values) : base(info, name, values) { }
    public override string ToString()
    {
        return base.ToString();
    }
}

public record HoudiniParameterFloat : HoudiniParameter<float>
{
    public HoudiniParameterFloat(HAPI_ParmInfo info, string name, params ReadOnlySpan<float> values) : base(info, name, values) { }
    public override string ToString()
    {
        return base.ToString();
    }
}

public record HoudiniParameterString : HoudiniParameter<string>
{
    public HoudiniParameterString(HAPI_ParmInfo info, string name, params ReadOnlySpan<string> values) : base(info, name, values) { }
    public override string ToString()
    {
        return base.ToString();
    }
}
