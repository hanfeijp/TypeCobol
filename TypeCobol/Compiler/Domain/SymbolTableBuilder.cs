﻿using System;
using TypeCobol.Compiler.CodeModel;
using TypeCobol.Compiler.CupParser.NodeBuilder;
using TypeCobol.Compiler.Nodes;
using TypeCobol.Compiler.Parser;
using TypeCobol.Compiler.Scopes;
using TypeCobol.Compiler.Symbols;
using TypeCobol.Tools.Options_Config;

namespace TypeCobol.Compiler.Domain
{
    /// <summary>
    /// Abstract base Class use to build the Symbol Table from a program perspective;
    /// </summary>
    public abstract class SymbolTableBuilder : ProgramClassBuilderNodeListener
    {
        /// <summary>
        /// The abstract scope of the SymbolTable built.
        /// </summary>
        public abstract AbstractScope Scope
        {
            get;
        }

        /// <summary>
        /// A Type Cobol configuration to be used.
        /// </summary>
        public static TypeCobolConfiguration Config
        {
            get;
            set;
        }

        /// <summary>
        /// Path of the Intrinsic file
        /// </summary>
        public static string IntrinsicPath
        {
            get;
            set;
        }

        /// <summary>
        /// Called when A node has been syntactically recognized by the TypeCobol Parser.
        /// </summary>
        /// <param name="node">The node being built</param>
        /// <param name="program">The Program that contains the node.</param>
        public abstract override void OnNode(Node node, Program program);

        /// <summary>
        /// The Root Symbol Table.
        /// </summary>
#if DOMAIN_CHECKER
        internal static RootSymbolTable _rootSymbolTable;
#else
        private static RootSymbolTable _rootSymbolTable;
#endif

        /// <summary>
        /// Lock variable on the Rootymboltable access
        /// </summary>
        private static object rootLock = new object();

        /// <summary>
        /// Check if this Symbol table Builder has a Root Symbol Table.
        /// </summary>
        public static bool HasRoot
        {
            get
            {
                lock (rootLock)
                {
                    return _rootSymbolTable != null;
                }
            }
        }
        /// <summary>
        /// The Root Symbol Table
        /// </summary>
        public static RootSymbolTable Root
        {
            get
            {
                lock (rootLock)
                {
                    if (_rootSymbolTable == null)
                    {
                        _rootSymbolTable = new RootSymbolTable();
                        //Store Builtin Symbols.
                        BuiltinSymbols.StoreSymbols(_rootSymbolTable.Types);
                        foreach (var t in _rootSymbolTable.Types)
                        {
                            //Enter each Builtin symbol in the domain also.
                            //And mark it as a builtin symbol.
                            t.SetFlag(Symbol.Flags.BuiltinSymbol, true);
                            _rootSymbolTable.AddToDomain(t);
                        }

                        LoadBaseTable();
                    }

                    return _rootSymbolTable;
                }
            }
            set
            {
                lock (rootLock)
                {
                    _rootSymbolTable = value;
                }
            }
        }

        /// <summary>
        /// Load intrinsic symbol in the root table.
        /// </summary>
        /// <param name="path"></param>
        public static void LoadIntrinsics(string path)
        {
            path = path ?? IntrinsicPath;
            if(path == null)
            {
                return; 
            }
            //Allocate a static Program Symbol Table Builder
            ProgramSymbolTableBuilder Builder = null;
            NodeListenerFactory BuilderNodeListenerFactory = () =>
            {
                Builder = new ProgramSymbolTableBuilder();
                return Builder;
            };
            Compiler.Parser.NodeDispatcher.RegisterStaticNodeListenerFactory(BuilderNodeListenerFactory);
            //Put all Types from programs into the Root Namespace.
            TransfertAllProgramsToIntrinsics();
            try
            {
                //There is no COPY in our intrinsic, so don't set AutoRemarks and copyFolders
                TypeCobol.Parser parser = TypeCobol.Parser.Parse(path, DocumentFormat.RDZReferenceFormat, /*bool autoRemarks =*/ false, /*IList<string> copies =*/ null);
            }
            finally
            {
                Compiler.Parser.NodeDispatcher.RemoveStaticNodeListenerFactory(BuilderNodeListenerFactory);
            }
        }

        /// <summary>
        /// Transfert the given program as an Intrinsics program.
        /// </summary>
        /// <param name="prg">The program to transfert as an intrinsics.</param>
        public static void TransfertIntrinsics(ProgramSymbol prg)
        {
            //We only transfert Types.
            foreach (var type in prg.Types)
            {
                Root.Types.Enter(type);
            }
        }


        /// <summary>
        /// Transfert all current programs has Intrinsics
        /// </summary>
        public static void TransfertAllProgramsToIntrinsics()
        {
            if (_rootSymbolTable != null)
            {
                foreach (var pgm in _rootSymbolTable.Programs)
                {
                    TransfertIntrinsics(pgm);
                }
            }
        }

        /// <summary>
        /// Load TypeCobol Intrinsics and dependencies into the root Symbol Table.
        /// </summary>
        /// <param name="config">The TypeCobol configuration</param>
        public static void LoadBaseTable(TypeCobolConfiguration config = null)
        {
            config = config ?? Config;
            if (config == null)
                return;
            SymbolTable baseSymbols = null;
#region Event Diags Handler
            bool diagDetected = false;
            EventHandler<Tools.APIHelpers.DiagnosticsErrorEvent> DiagnosticsErrorEvent = delegate (object sender, Tools.APIHelpers.DiagnosticsErrorEvent diagEvent)
            {
                //Delegate Event to handle diagnostics generated while loading dependencies/intrinsics
                diagDetected = true;
            };
            EventHandler<Tools.APIHelpers.DiagnosticsErrorEvent> DependencyErrorEvent = delegate (object sender, Tools.APIHelpers.DiagnosticsErrorEvent diagEvent)
            {
            };
#endregion

            //Allocate a static Program Symbol Table Builder for our representation.
            ProgramSymbolTableBuilder Builder = null;
            NodeListenerFactory BuilderNodeListenerFactory = () =>
            {
                Builder = new ProgramSymbolTableBuilder();
                return Builder;
            };
            Compiler.Parser.NodeDispatcher.RegisterStaticNodeListenerFactory(BuilderNodeListenerFactory);

            try
            {
                baseSymbols =
                    Tools.APIHelpers.Helpers.LoadIntrinsic(config.Copies, config.Format,
                        DiagnosticsErrorEvent); //Load intrinsic
                //Put all Intrinsic Types from programs into the Root Namespace.
                TransfertAllProgramsToIntrinsics();

                baseSymbols = Tools.APIHelpers.Helpers.LoadDependencies(config.Dependencies, config.Format, baseSymbols,
                    config.InputFiles, config.CopyFolders, DependencyErrorEvent); //Load dependencies
            }
            finally
            {
                Compiler.Parser.NodeDispatcher.RemoveStaticNodeListenerFactory(BuilderNodeListenerFactory);
            }
        }
    }
}
