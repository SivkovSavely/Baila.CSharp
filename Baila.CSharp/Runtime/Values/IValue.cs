using Baila.CSharp.Runtime.Types;

namespace Baila.CSharp.Runtime.Values;

public interface IValue
{
    long GetAsInteger();
    double GetAsFloat();
    bool GetAsBoolean();
    string GetAsString();

    BailaType GetBailaType();
}