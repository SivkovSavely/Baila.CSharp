using Baila.CSharp.Typing;

namespace Baila.CSharp.Runtime.Values.Abstractions;

public interface IValue
{
    long GetAsInteger();
    double GetAsFloat();
    bool GetAsBoolean();
    string GetAsString();

    BailaType GetBailaType();
}