using Baila.CSharp.Typing;

namespace Baila.CSharp.Ast.Values;

public interface IValue
{
    long GetAsInteger();
    double GetAsFloat();
    bool GetAsBoolean();
    string GetAsString();

    BailaType GetBailaType();
}