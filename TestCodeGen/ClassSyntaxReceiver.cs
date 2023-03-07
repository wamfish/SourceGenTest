//  Copyright (c) 2023-present John Roscoe Hamilton
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace TestCodeGen;

/// <summary>
/// Creates a List<> of all ClassDeclarationSyntax nodes
/// </summary>
class ClassSyntaxReceiver : ISyntaxReceiver
{
    public readonly StringBuilder AllClasses = new StringBuilder();
    public readonly List<ClassDeclarationSyntax> Classes = new List<ClassDeclarationSyntax>();
    public readonly string BaseClass;
    public ClassSyntaxReceiver(string baseClass)
    {
        this.BaseClass = baseClass;
    }
    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (!(syntaxNode is ClassDeclarationSyntax)) return;
        var c = (ClassDeclarationSyntax)syntaxNode;
        AllClasses.AppendLine(c.Identifier.ToString());
        if (c.BaseList == null) return;
        var sbt = c.BaseList.ChildNodes().OfType<SimpleBaseTypeSyntax>().First();
        var insNodes = sbt.ChildNodes().OfType<IdentifierNameSyntax>();
        if (insNodes.Count() > 0)
        {
            var ins = insNodes.First();
            if (ins.Identifier.ValueText == BaseClass) Classes.Add(c);
        }
    }
}