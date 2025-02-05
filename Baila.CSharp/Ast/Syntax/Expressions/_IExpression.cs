﻿using Baila.CSharp.Runtime.Types;
using Baila.CSharp.Runtime.Values;
using Baila.CSharp.Visitors;

namespace Baila.CSharp.Ast.Syntax.Expressions;

public interface IExpression : ISyntaxNode
{
    /// <summary>
    /// Returns the Baila code for this expression.
    /// The code returned by this method should be able to be parsed back into the identical expression.
    /// </summary>
    /// <returns>Stringified expression</returns>
    string Stringify();

    /// <summary>
    /// Returns the Baila type that is the result of the expression evaluation.
    /// </summary>
    BailaType? GetBailaType();

    IValue Evaluate();

    void AcceptVisitor(VisitorBase visitor);
}