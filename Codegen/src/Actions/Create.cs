﻿using System;
using System.Collections.Generic;
using TypeCobol.Codegen.Nodes;
using TypeCobol.Compiler.Nodes;

namespace TypeCobol.Codegen.Actions
{
    /// <summary>
    /// Action to create a new Generate Node.
    /// </summary>
    public class Create : EventArgs, Action
    {
        public string Group { get; private set; }
        public Node Parent
        {
            get;
            internal set;
        }
        private Node Child;
        private int? position;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="parent">The parent of the new Generate Node to Create</param>
        /// <param name="pattern">The pattern to apply</param>
        /// <param name="variables">The Substitution variables</param>
        /// <param name="group">The Group ID</param>
        /// <param name="position">The Insertion position (index) as child in the Parent node</param>
        public Create(Node parent, TypeCobol.Codegen.Skeletons.Pattern pattern, Dictionary<string, object> variables, string group, int? position)
        {
            this.Parent = parent;
            if (group != null) this.Group = new TypeCobol.Codegen.Skeletons.Templates.RazorEngine().Replace(group, variables);
            var solver = TypeCobol.Codegen.Skeletons.Templates.RazorEngine.Create(pattern.Template, variables);
            this.Child = new GeneratedNode((TypeCobol.Codegen.Skeletons.Templates.RazorEngine)solver);
            if (pattern.NewLine)
                this.Child.SetFlag(Node.Flag.FactoryGeneratedNodeWithFirstNewLine, true);
            //Mark this node as has been generated by a factory creation mechanism.
            this.Child.SetFlag(Node.Flag.FactoryGeneratedNode, true);
            this.position = position;

            //This is a special case for issue :
            //Codegen for procedure : remove usage of external  #519 
            if (pattern.Name != null && pattern.Name.Equals("ProcedureDivisionCalleeWithoutExternal"))
            {
                //look for the ProcedureDivision Node
                Node proc_parent = parent;
                while (!(proc_parent is TypeCobol.Compiler.Nodes.ProcedureDivision))
                    proc_parent = proc_parent.Parent;
                if (proc_parent != null)
                {
                    proc_parent.SetFlag(Node.Flag.ProcedureDivisionUsingPntTabPnt, true);
                    proc_parent.SetFlag(Node.Flag.ForceGetGeneratedLines, true);
                }
            }
        }
        /// <summary>
        /// Perform the creation action, the new GeneratedNode is added as child in the Parent node.
        /// </summary>
        public void Execute()
        {
            int index = (position ?? -1);
            Parent.Add(Child, index);
            if (index >= 0)
            {
                Child.SetFlag(Node.Flag.FactoryGeneratedNodeKeepInsertionIndex, true);
            }
        }
    }
}