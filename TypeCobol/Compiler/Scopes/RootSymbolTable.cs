﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeCobol.Compiler.Domain;
using TypeCobol.Compiler.Symbols;

namespace TypeCobol.Compiler.Scopes
{
    /// <summary>
    /// The Root Symbol Table is a special Namespace
    /// </summary>
    public class RootSymbolTable : NamespaceSymbol
    {
        /// <summary>
        /// This is the first variable of the universe that can be assimilated to the 0 or null variable.
        /// </summary>
        public VariableSymbol BottomVariable { get; private set; }

        /// <summary>
        /// The count of all variable created in this RootSymbolTable
        /// </summary>
        private int _variableSymbolCounter = 0;
        /// <summary>
        /// A pool of free global index
        /// </summary>
        private Stack<int> GlobalIndexPool = new Stack<int>();

        /// <summary>
        /// Empty Constructor.
        /// </summary>
        public RootSymbolTable() : base(string.Intern("<<Root>>"))
        {
            base.Kind = Kinds.Root;
            Universe = new List<VariableSymbol>();
            ScopeDomain = new Domain<AbstractScope>();
            TypeDomain = new Domain<TypedefSymbol>();
            BottomVariable = new VariableSymbol("<<BottomVariable>>");
            AddToUniverse(BottomVariable);
            //Load Builtin symbol in it
            SymbolTableBuilder.AddBuiltinSymbol(this);
        }

        /// <summary>
        /// Copy Constructor
        /// </summary>
        /// <param name="from">RootSymbolTable instance from which to copy</param>
        public RootSymbolTable(RootSymbolTable from) : base(string.Intern("<<Root>>"), from)
        {
            base.Kind = Kinds.Root;
            if (from != null)
            {
                this.BottomVariable = from.BottomVariable;
                this.GlobalIndexPool = new Stack<int>(from.GlobalIndexPool);
                this._variableSymbolCounter = from._variableSymbolCounter;
                Universe = new List<VariableSymbol>(from.Universe);
                ScopeDomain = new Domain<AbstractScope>(from.ScopeDomain);
                TypeDomain = new Domain<TypedefSymbol>(from.TypeDomain);
            }
        }

        /// <summary>
        /// Full qualified name of this Symbol à la TypeCobol using "::"
        /// </summary>
        public override String FullName => "";

        /// <summary>
        /// Full qualified name of this Symbol à la COBOL85 using OF
        /// </summary>
        public override String FullOfName => "";

        /// <summary>
        /// Full dotted qualified name
        /// </summary>
        public override String FullDotName => "";

        /// <summary>
        /// Full typed dotted qualified name
        /// </summary>
        public override String FullTypedDotName => "";

        public override String TypedName => "";

        /// <summary>
        /// Program add event.
        /// </summary>
        public event EventHandler<SymbolEventArgs> ProgramAdded;

        /// <summary>
        /// The Count of Variable Symbol created
        /// </summary>
        public int VariableSymbolCount => _variableSymbolCounter;

        /// <summary>
        /// Get the Next VariableSymbol Context.
        /// </summary>
        /// <returns></returns>
        private int NextVariableSymbolIndex()
        {
            if (GlobalIndexPool.Count > 0)
            {
                return GlobalIndexPool.Pop();
            }
            return _variableSymbolCounter++;
        }

        /// <summary>
        /// Add the given VariableSymbol instance in this Root Symbol Table universe
        /// </summary>
        /// <param name="varSym">The Variable Symbol to be added</param>
        /// <returns>The given VariableSymbol instance.</returns>
        internal VariableSymbol AddToUniverse(VariableSymbol varSym)
        {
            System.Diagnostics.Debug.Assert(varSym != null);
            System.Diagnostics.Debug.Assert(varSym.GlobalIndex == 0);
            lock (Universe)
            {
                varSym.GlobalIndex = NextVariableSymbolIndex();
                Universe.Add(varSym);
            }

            return varSym;
        }

        /// <summary>
        /// Remove from the universe the given variable symbol.
        /// </summary>
        /// <param name="varSym">The variable symbol to be removed</param>
        internal void RemoveFromUniverse(VariableSymbol varSym)
        {
            System.Diagnostics.Debug.Assert(varSym != null);
            System.Diagnostics.Debug.Assert(varSym.GlobalIndex != 0);
            if (varSym.GlobalIndex != 0)
            {
                lock (Universe)
                {
                    ((IList < VariableSymbol >)Universe)[varSym.GlobalIndex] = null;
                    GlobalIndexPool.Push(varSym.GlobalIndex);
                    varSym.GlobalIndex = 0;
                }
            }
        }

        /// <summary>
        /// All Ordered Symbol that can be reached from this Root Symbol Table.
        /// This is in fact the entire domain of variable within this Root Symbol Table.
        /// </summary>
        public IList<VariableSymbol> Universe { get; internal set; }

        /// <summary>
        /// The Domain of all namespaces, programs and functions.
        /// </summary>
        internal Domain<AbstractScope> ScopeDomain { get; private set; }

        /// <summary>
        /// The Domain of all types.
        /// </summary>
        internal Domain<TypedefSymbol> TypeDomain { get; private set; }

        /// <summary>
        /// Add the given AbstractScope instance the domain
        /// </summary>
        /// <param name="absScope">Abstract Scope to be added</param>
        public override void AddToDomain(AbstractScope absScope)
        {
            System.Diagnostics.Debug.Assert(absScope != null);
            ScopeDomain.Add(absScope);
            if (ProgramAdded != null && absScope.Kind == Kinds.Program)
                ProgramAdded(this, new SymbolEventArgs(absScope));
        }

        /// <summary>
        /// Remove the given scope from the domain.
        /// </summary>
        /// <param name="absScope">The Scope to be removed</param>
        public override void RemoveFromDomain(AbstractScope absScope)
        {
            ScopeDomain.Remove(absScope);

            absScope.FreeDomain();

            //Remove all Types
            var types = absScope.Types;
            if (types != null)
            {
                foreach (var t in types)
                {
                    RemoveFromDomain(t);
                }
            }

            //Remove all programs
            var programs = absScope.Programs;
            if (programs != null)
            {
                foreach (var p in programs)
                {
                    RemoveFromDomain(p);
                }
            }

            //Remove all functions
            var functions = absScope.Functions;
            if (functions != null)
            {
                foreach (var p in functions)
                {
                    RemoveFromDomain(p);
                }
            }

            //Special case Namespace
            if (absScope.Kind == Kinds.Namespace)
            {
                NamespaceSymbol ns = (NamespaceSymbol)absScope;
                var nss = ns.Namespaces;
                if (nss != null)
                {
                    foreach (var n in nss)
                    {
                        RemoveFromDomain(n);
                    }
                }
            }
        }

        /// <summary>
        /// Add the given Type instance the domain
        /// </summary>
        /// <param name="type">The type to add to be added</param>
        public override void AddToDomain(TypedefSymbol type)
        {
            System.Diagnostics.Debug.Assert(type != null);
            TypeDomain.Add(type);
        }

        /// <summary>
        /// Remove the given type from the domain.
        /// </summary>
        /// <param name="type">The type to be removed</param>
        public override void RemoveFromDomain(TypedefSymbol type)
        {
            TypeDomain.Remove(type);
        }

        /// <summary>
        /// All Kinds of scope that.
        /// </summary>
        private static readonly Symbol.Kinds[] AllScopeKinds = new Kinds[]
            {Kinds.Namespace, Kinds.Program, Kinds.Function};

        /// <summary>
        /// Resolve any AbstractScope. Namespace, program and function are abstract scopes.
        /// </summary>
        /// <param name="path">The Abstract scope path</param>
        /// <param name="kinds">All kinds of scope to be resolved.</param>
        /// <returns>A Scope instance of matches</returns>
        public Scope<TS>.MultiSymbols ResolveScope<TS>(string[] path, params Symbol.Kinds[] kinds)
            where TS : AbstractScope
        {
            Scope<TS>.MultiSymbols results = new Scope<TS>.MultiSymbols();
            if (path == null || path.Length == 0)
                return results;

            string name = path[0];
            if (this.ScopeDomain.TryGetValue(name, out var candidates))
            {
                kinds = kinds == null || kinds.Length == 0 ? AllScopeKinds : kinds;
                foreach (var candidate in candidates)
                {
                    if (kinds.Contains(candidate.Kind) && candidate.IsMatchingPath(path))
                        results.Add((TS) candidate);
                }
            }

            return results;
        }

        /// <summary>
        /// Resolve a namespace path
        /// </summary>
        /// <param name="path">The Namespace's path'</param>
        /// <returns></returns>
        public Scope<NamespaceSymbol>.MultiSymbols ResolveNamespace(string[] path)
        {
            return ResolveScope<NamespaceSymbol>(path, Kinds.Namespace);
        }

        /// <summary>
        /// Resolve a Program path
        /// </summary>
        /// <param name="path">The Program's path'</param>
        /// <returns>The set of matching results</returns>
        public Scope<ProgramSymbol>.MultiSymbols ResolveProgram(string[] path)
        {
            return ResolveScope<ProgramSymbol>(path, Kinds.Program);
        }

        /// <summary>
        /// Resolve a Function path
        /// </summary>
        /// <param name="path">The function's path'</param>
        /// <returns></returns>
        public Scope<FunctionSymbol>.MultiSymbols ResolveFunction(string[] path)
        {
            return ResolveScope<FunctionSymbol>(path, Kinds.Function);
        }

        /// <summary>
        /// Resolve a Program or a Function path
        /// </summary>
        /// <param name="path">The program's' or function's path'</param>
        /// <returns>The set of matching results</returns>
        public Scope<ProgramSymbol>.MultiSymbols ResolveProgramOrFunction(string[] path)
        {
            return ResolveScope<ProgramSymbol>(path, Kinds.Program, Kinds.Function);
        }

        /// <summary>
        /// Resolve a Type
        /// </summary>
        /// <param name="path">Type's path'</param>
        /// <param name="bIncludeUndefined">True if undefined type must also be included, false otherwise</param>
        /// <returns>The set of matching results</returns>
        public Scope<TypedefSymbol>.MultiSymbols ResolveQualifiedType(string[] path, bool bIncludeUndefined = false)
        {
            Scope<TypedefSymbol>.MultiSymbols results = new Scope<TypedefSymbol>.MultiSymbols();
            if (path == null || path.Length == 0)
                return results;

            string name = path[0];
            if (this.TypeDomain.TryGetValue(name, out var candidates))
            {
                foreach (var candidate in candidates)
                {
                    //Only selected whose Type is defined.
                    if ((bIncludeUndefined || candidate.Type != null) && candidate.IsMatchingPath(path))
                        results.Add(candidate);
                }
            }

            return results;
        }
    }
}
