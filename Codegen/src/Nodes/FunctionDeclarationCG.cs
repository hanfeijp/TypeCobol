using System;
using System.Linq;
using JetBrains.Annotations;
using TypeCobol.Codegen.Contribution;

namespace TypeCobol.Codegen.Nodes {
    using System.Collections.Generic;
    using TypeCobol.Compiler.CodeElements;
    using TypeCobol.Compiler.CodeElements.Expressions;
    using TypeCobol.Compiler.Nodes;
    using TypeCobol.Compiler.Text;

    internal class FunctionDeclarationCG : Compiler.Nodes.FunctionDeclaration, Generated {
        FunctionDeclaration OriginalNode = null;

        public FunctionDeclarationCG(Compiler.Nodes.FunctionDeclaration originalNode) : base(originalNode.CodeElement) {
            this.OriginalNode = originalNode;

            //Check if we need to generate something special for this Procedure
            bool needToGenerateParametersIntoLinkage = originalNode.CodeElement.Profile.InputParameters.Count + originalNode.CodeElement.Profile.InoutParameters.Count + originalNode.CodeElement.Profile.OutputParameters.Count +
                             (originalNode.CodeElement.Profile.ReturningParameter != null ? 1 : 0) > 0;
            //we'll generate things for public call
            var containsPublicCall = originalNode.ProcStyleCalls != null && originalNode.ProcStyleCalls.Count > 0;

            foreach (var child in originalNode.Children) {
                if (child is Compiler.Nodes.ProcedureDivision) {
                    Compiler.Nodes.LinkageSection linkageSection = null;
                    Compiler.Nodes.DataDivision dataDivision = null;

                    //Create DataDivision and LinkageSection if needed
                    //DataDivision must be created before ProcedureDivision because Program Node doesn't manage order of their children
                    //DataDivision manage order of their children so it's ok
                    if (needToGenerateParametersIntoLinkage || containsPublicCall) {
                        dataDivision = GetOrCreateNode<Compiler.Nodes.DataDivision>(originalNode, () => new DataDivision());
                        linkageSection = GetOrCreateNode<Compiler.Nodes.LinkageSection>(dataDivision, () => new LinkageSection(originalNode), dataDivision);


                        //declare procedure parameters into linkage
                        DeclareProceduresParametersIntoLinkage(originalNode, linkageSection, originalNode.Profile);
                    }
                    
                    if (originalNode.IsFlagSet(Node.Flag.UseGlobalStorage))
                    {
                        if (dataDivision == null)
                        {
                            dataDivision = GetOrCreateNode<Compiler.Nodes.DataDivision>(originalNode, () => new DataDivision());
                        }
                        (linkageSection ?? GetOrCreateNode<Compiler.Nodes.LinkageSection>(dataDivision, () => new LinkageSection(originalNode), dataDivision)).Add(new GlobalStorage.GlobalStorageNode());
                    }

                    //Replace ProcedureDivision node with a new one and keep all original children
                    var sentences = new List<Node>();
                    foreach (var sentence in child.Children)
                        sentences.Add(sentence);
                    var pdiv = new ProcedureDivision(originalNode, sentences);
                    children.Add(pdiv);


                    //Generate code if this procedure call a public procedure in another source
                    if (containsPublicCall) {
                        var workingStorageSection = GetOrCreateNode<Compiler.Nodes.WorkingStorageSection>(dataDivision, () => new WorkingStorageSection(originalNode), dataDivision);

                        ProgramImports imports = ProgramImportsAttribute.GetProgramImports(originalNode);
                        Node[] toAddRange =
                        {
                            new GeneratedNode2("01 TC-Call          PIC X     VALUE 'T'.", true),
                            new GeneratedNode2("    88 TC-FirstCall  VALUE 'T'.", true),
                            new GeneratedNode2("    88 TC-NthCall    VALUE 'F'", true),
                            new GeneratedNode2("                     X'00' thru 'S'", true),
                            new GeneratedNode2("                     'U' thru X'FF'.", true)
                        };
                        workingStorageSection.AddRange(toAddRange, 0);
                        GenerateCodeToCallPublicProc(originalNode, pdiv,  workingStorageSection, linkageSection);
                    }
                    else if (OriginalNode.IsFlagSet(Node.Flag.UseGlobalStorage))
                    {
                        pdiv.AddRange(GenerateCodeToCallGlobalStorage(4), 0);
                    }
                } else {
                    if (child.CodeElement is FunctionDeclarationEnd)
                    {
                        children.Add(new ProgramEnd(new URI(OriginalHash), child.CodeElement.Line));
                    } else {
                        // TCRFUN-CODEGEN-NO-ADDITIONAL-DATA-SECTION
                        // TCRFUN-CODEGEN-DATA-SECTION-AS-IS
                        children.Add(child);
                    }
                }
            }
        }

        /// <summary>
        /// Get or create the node and add it as a child of this Node
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="func"></param>
        /// <returns></returns>
        private T GetOrCreateNode<T>(Node parent, Func<T> func) where T : Node {
            return GetOrCreateNode(parent, func, this);
        }

        private T GetOrCreateNode<T>(Node parent, Func<T> func, Node newParent) where T : Node {
            T newNode;
            var retrievedChild = parent.GetChildren<T>();
            if (retrievedChild == null || !retrievedChild.Any()) {
                newNode = func.Invoke();
                //Do not add to parent but to "this"
                newParent.Add(newNode);
            } else {
                newNode = retrievedChild.First();
            }
            return newNode;
        }


        protected void GenerateCodeToCallPublicProc(FunctionDeclaration originalNode, ProcedureDivision procedureDivision, Compiler.Nodes.WorkingStorageSection workingStorageSection, Compiler.Nodes.LinkageSection linkageSection) {
            ProgramImports imports = ProgramImportsAttribute.GetProgramImports(originalNode);

            foreach (var pgm in imports.Programs.Values) {
                workingStorageSection.Add(
                    new GeneratedNode2("01 TC-" + pgm.Name + " pic X(08) value '" + pgm.Name.ToUpperInvariant() + "'.\n", true), 0);
            }

            List<Node> toAddRange = new List<Node>();
            toAddRange.Add(new GeneratedNode2("*Common to all librairies used by the program.", true));
            toAddRange.Add(new GeneratedNode2("01 TC-Library-PntTab.", false));
            toAddRange.Add(new GeneratedNode2("    05 TC-Library-PntNbr          PIC S9(04) COMP.", true));
            toAddRange.Add(new GeneratedNode2(
                "    05 TC-Library-Item OCCURS 1000\n                        DEPENDING ON TC-Library-PntNbr\n                        INDEXED   BY TC-Library-Idx.",
                false));
            toAddRange.Add(new GeneratedNode2("       10 TC-Library-Item-Idt      PIC X(08).", true));
            toAddRange.Add(new GeneratedNode2("       10 TC-Library-Item-Pnt      PROCEDURE-POINTER.", true));


            foreach (var pgm in imports.Programs.Values) {
                foreach (var proc in pgm.Procedures.Values) {
                    proc.IsNotByExternalPointer = true;
                    toAddRange.Add(new GeneratedNode2(" ", true));
                    toAddRange.Add(new GeneratedNode2("*To call program " + proc.Hash + " in module " + proc.ProcStyleCall.FunctionDeclaration.QualifiedName.Tail, false));
                    toAddRange.Add(new GeneratedNode2("*Which is generated code for " + proc.ProcStyleCall.FunctionDeclaration.QualifiedName, false));
                    toAddRange.Add(new GeneratedNode2("*Declared in source file " + proc.ProcStyleCall.FunctionDeclaration.CodeElement.TokenSource.SourceName, false));
                    toAddRange.Add(new GeneratedNode2("01 TC-" + pgm.Name + "-" + proc.Hash + "-Item.", false));
                    toAddRange.Add(new GeneratedNode2("   05 TC-" + pgm.Name + "-" + proc.Hash + "-Idt PIC X(08).", true));
                    toAddRange.Add(new GeneratedNode2("   05 TC-" + pgm.Name + "-" + proc.Hash + " PROCEDURE-POINTER.",
                        true));
                }
            }
            linkageSection.AddRange(toAddRange, 0);
        }

        private void DeclareProceduresParametersIntoLinkage(Compiler.Nodes.FunctionDeclaration node, Compiler.Nodes.LinkageSection linkage, ParametersProfileNode profile) {
            var data = linkage.Children();

            // TCRFUN-CODEGEN-PARAMETERS-ORDER
            var generated = new List<string>();
            foreach (var parameter in profile.InputParameters) {
                if (!generated.Contains(parameter.Name) && !Contains(data, parameter.Name)) {
                    linkage.Add(CreateParameterEntry(parameter, node));
                    generated.Add(parameter.Name);
                }
            }
            foreach (var parameter in profile.InoutParameters) {
                if (!generated.Contains(parameter.Name) && !Contains(data, parameter.Name)) {
                    linkage.Add(CreateParameterEntry(parameter, node));
                    generated.Add(parameter.Name);
                }
            }
            foreach (var parameter in profile.OutputParameters) {
                if (!generated.Contains(parameter.Name) && !Contains(data, parameter.Name)) {
                    linkage.Add(CreateParameterEntry(parameter, node));
                    generated.Add(parameter.Name);
                }
            }
            if (profile.ReturningParameter != null) {
                if (!generated.Contains(profile.ReturningParameter.Name) &&
                    !Contains(data, profile.ReturningParameter.Name)) {
                    linkage.Add(CreateParameterEntry(profile.ReturningParameter, node));
                    generated.Add(profile.ReturningParameter.Name);
                }
            }
            
        }

        private Node[] GenerateCodeToCallGlobalStorage(int columnOffset)
        {
            return new Node[]
            {
                new GeneratedNode2("* Get the data from the global storage section", false),
                new GeneratedNode2($"{new string(' ', columnOffset)}CALL '{OriginalNode.Root.MainProgram.Hash}' USING", true),
                new GeneratedNode2($"{new string(' ', columnOffset)}    by reference address of TC-GlobalData", true),
                new GeneratedNode2($"{new string(' ', columnOffset)}end-call", true),
            };
        }

        private ParameterEntry CreateParameterEntry(ParameterDescription parameter, FunctionDeclaration node)
        {
            var paramEntry = parameter.CodeElement as ParameterDescriptionEntry;
            var generated = new ParameterEntry(paramEntry, node.SymbolTable, parameter);
            if (paramEntry.DataConditions != null) {
                foreach (var child in paramEntry.DataConditions) generated.Add(new DataCondition(child));
            }

            var parameterNode = node.Profile.Parameters.FirstOrDefault(x => x.Name == paramEntry.Name);
            if (parameterNode != null)
                generated.CopyFlags(parameterNode.Flags);
            return generated;
        }

        private bool Contains([NotNull] IEnumerable<DataDefinition> data, string dataname) {
            foreach (var node in data)
                if (dataname.Equals(node.Name))
                    return true;
            return false;
        }

        private List<ITextLine> _cache = null;

        public override IEnumerable<ITextLine> Lines {
            get {
                if (_cache == null) {
                    _cache = new List<ITextLine>(); // TCRFUN-CODEGEN-AS-NESTED-PROGRAM
                    //_cache.Add(new TextLineSnapshot(-1, "*", null));
                    //TODO add Function signature as comment
                    _cache.Add(new TextLineSnapshot(-1, "*_________________________________________________________________",
                        null));
                    _cache.Add(new TextLineSnapshot(-1, "IDENTIFICATION DIVISION.", null));
                    if (OriginalNode.GenerateAsNested)
                    {
                        _cache.Add(new TextLineSnapshot(-1, "PROGRAM-ID. " + OriginalHash + " IS COMMON.", null));
                    }
                    else
                    {
                        _cache.Add(new TextLineSnapshot(-1, "PROGRAM-ID. " + OriginalHash + '.', null));
                    }

                    var envDiv = OriginalNode.GetProgramNode().GetChildren<EnvironmentDivision>();
                    if (!GenerateAsNested && envDiv != null && envDiv.Count == 1) {
                        _cache.Add(new TextLineSnapshot(-1, "ENVIRONMENT DIVISION. ", null));

                        var configSections = envDiv.First().GetChildren<ConfigurationSection>();

                        if (configSections != null && configSections.Count == 1) {
                            foreach (var line in configSections.First().SelfAndChildrenLines) {
                                _cache.Add(line);
                            }
                        }
                    } //else no config or semantic error, no my job  
                }
                return _cache;
            }
        }

        public new bool GenerateAsNested => OriginalNode.GenerateAsNested;

        public string OriginalHash => OriginalNode.Hash;

        public bool IsLeaf {
            get { return false; }
        }
    }
}