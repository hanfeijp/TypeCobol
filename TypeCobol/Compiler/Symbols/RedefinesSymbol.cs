﻿using System.IO;

namespace TypeCobol.Compiler.Symbols
{
    /// <summary>
    /// Represents a Redefines Symbol, for instance in the example Below it is the variable B.
    /// ex:
    /// 05  A PICTURE X(6).
    /// 05  B REDEFINES A.
    ///     10 B-1          PICTURE X(2).
    ///     10 B-2          PICTURE 9(4).
    /// 05  C PICTURE 99V99.
    /// </summary>
    public class RedefinesSymbol : VariableSymbol
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Symbol 's name</param>
        /// <param name="redefinedPath">The redefined Symbol path</param>
        public RedefinesSymbol(string name, string[] redefinedPath)
            : base(name)
        {
            System.Diagnostics.Debug.Assert(redefinedPath != null);
            System.Diagnostics.Debug.Assert(redefinedPath.Length != 0);
            base.SetFlag(Flags.Redefines, true);
            RedefinedPath = redefinedPath;
        }

        /// <summary>
        /// Reference to the redefined symbol.
        /// </summary>
        public string[] RedefinedPath { get; }

        public override void Dump(TextWriter output, int indentLevel)
        {
            base.Dump(output, indentLevel);
            string indent = new string(' ', 2 * indentLevel);

            output.Write(indent);
            output.WriteLine($"RedefinedPath: [{string.Join(", ", RedefinedPath)}]");
        }

        public override TResult Accept<TResult, TParameter>(IVisitor<TResult, TParameter> v, TParameter arg)
        {
            return v.VisitRedefinesSymbol(this, arg);
        }
    }
}
