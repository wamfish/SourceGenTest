//  Copyright (c) 2023-present John Roscoe Hamilton
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace TestCodeGen; 
/// <summary>
/// Gather information for a class based on it's ClassDeclarationSyntax.
/// </summary>
class ClassInfo
{
    public static INamedTypeSymbol KeyAttribute = null; //disabled for test
    public static INamedTypeSymbol XAttribute = null;
    public static INamedTypeSymbol RangeAttribute = null;
    public string Name { get; set; } = string.Empty;
    public string BaseClass { get; set; } = string.Empty;
    public string Namespace { get; set; } = string.Empty;
    public string Initializer { get; set; } = string.Empty;
    public List<string> Comments { get; set; } = new List<string>();
    public bool isPublic = false;
    public bool isPrivate => !isPublic;
    public bool isSealed = false;
    public bool isStatic = false;
    public bool isPartial = false;
    public List<FieldInfo> Fields = new List<FieldInfo>();
    public ClassInfo(Compilation comp, ClassDeclarationSyntax cds)
    {
        List<IFieldSymbol> fields = new List<IFieldSymbol>();
        List<IPropertySymbol> properties = new List<IPropertySymbol>();
        Name = cds.Identifier.ToString();
        var cta = cds.ChildTokens();
        foreach(var ct in cta)
        {
            if (ct.ValueText == "partial") isPartial= true;
        }

        var model = comp.GetSemanticModel(cds.SyntaxTree);
        var symbol = model.GetDeclaredSymbol(cds);
        Namespace = symbol.ContainingNamespace.ToString();

        if (!(symbol is ITypeSymbol)) return;
        var type = symbol as ITypeSymbol;

        var bt = type.BaseType;
        if (bt != null)
        {
            BaseClass = bt.Name;
            fields.AddRange(bt.GetMembers().OfType<IFieldSymbol>());
            properties.AddRange(bt.GetMembers().OfType<IPropertySymbol>());
        }
        fields.AddRange(type.GetMembers().OfType<IFieldSymbol>());
        properties.AddRange(type.GetMembers().OfType<IPropertySymbol>());

        if (symbol.DeclaredAccessibility == Accessibility.Public) isPublic = true;
        if (symbol.IsSealed) isSealed = true;
        if (symbol.IsStatic) isStatic = true;
        //var bc = classes.Where(x => x.Identifier.ValueText == BaseClass);
        //if (bc.Count() > 0) 
        //    LoadFields(comp,bc.First());
        //LoadFields(comp,cds); 
        LoadFields(comp, cds, fields);
        LoadProperties(comp,properties);
    }
    public string FixType(string type, string nameSpace)
    {
        if (nameSpace.Length > 0)
        {
            type = type.Replace(nameSpace + ".", "");
        }
        type = type.Replace("Godot.", "");
        type = type.Replace("System.", "");
        return type;
    }
    private void LoadFields(Compilation comp, ClassDeclarationSyntax cds, List<IFieldSymbol> fields)
    {
        //var model = comp.GetSemanticModel(cds.SyntaxTree);
        foreach (var field in fields)
        {
            var fi = new FieldInfo();
            fi.Name = field.Name;
            fi.Type = FixType(field.Type.ToString(), field.Type.ContainingNamespace.ToString());
            if (field.DeclaredAccessibility == Accessibility.Public) fi.isPublic = true;
            if (field.IsConst) fi.isConst = true;
            if (field.IsStatic) fi.isStatic = true;
            if (field.IsReadOnly) fi.isReadonly = true;
            if (!field.DeclaringSyntaxReferences.IsEmpty)
            {
                var vsr = field.DeclaringSyntaxReferences.First();
                var vs = vsr.GetSyntax();
                if (vs is VariableDeclaratorSyntax vds)
                {
                    if (vds.Initializer != null)
                        fi.Initialization = vds.Initializer.ToFullString();
                    var model = comp.GetSemanticModel(vds.SyntaxTree);
                    var fsymbol = model.GetDeclaredSymbol(vds);
                    if (fsymbol != null)
                    {
                        var attribs = fsymbol.GetAttributes();
                        for(int i=0;i<attribs.Length; i++)
                        {
                            var ad = attribs[i];
                            if (ad.AttributeClass.Equals(KeyAttribute, SymbolEqualityComparer.Default))
                            {
                                fi.isKey = true;
                            }
                            if (ad.AttributeClass.Equals(XAttribute, SymbolEqualityComparer.Default))
                            {
                                fi.isX = true;
                            }
                            //if (ad.AttributeClass.Equals(DataGenerator.RangeAttribute, SymbolEqualityComparer.Default))
                            //{
                            //    ImmutableArray<TypedConstant> args = ad.ConstructorArguments;
                            //    fi.Min = (float) args[0].Value;
                            //    fi.Max = (float) args[1].Value;
                            //    //foreach (KeyValuePair<string, TypedConstant> namedArgument in ad.NamedArguments)
                            //    //{
                            //    //    if (namedArgument.Key == "Min")
                            //    //    {
                            //    //        var min = namedArgument.Value.Value;
                            //    //        fi.Min = (float) min;
                            //    //    }
                            //    //    if (namedArgument.Key == "Max")
                            //    //    {
                            //    //        var min = namedArgument.Value.Value;
                            //    //        fi.Min = (float)min;
                            //    //    }
                            //    //}
                            //}
                        }
                    }
                }
            }
            Fields.Add(fi);
        }
    }
    private void LoadProperties(Compilation comp, List<IPropertySymbol> properties)
    {
        foreach (var field in properties)
        {
            var fi = new FieldInfo();
            fi.isProperty = true;
            fi.Name = field.Name;
            if (field.DeclaredAccessibility == Accessibility.Public) fi.isPublic = true;
            if (field.IsStatic) fi.isStatic = true;
            if (field.IsReadOnly) fi.isReadonly = true;
            if (field.DeclaringSyntaxReferences.Count() > 0)
            {
                foreach (var s in field.DeclaringSyntaxReferences)
                {
                    var ds = s.GetSyntax();
                    if (ds is PropertyDeclarationSyntax pds)
                    {
                        if (pds.Initializer != null) fi.Initialization = pds.Initializer.ToFullString();
                        var model = comp.GetSemanticModel(pds.SyntaxTree);
                        var fsymbol = model.GetDeclaredSymbol(pds);
                        fi.Type = FixType(pds.Type.ToString(), fsymbol.ContainingNamespace.ToString());
                        if (fsymbol != null)
                        {
                            fi.isAutoProperty = IsAutoProperty(fsymbol);
                            var attribs = fsymbol.GetAttributes();
                            for (int i = 0; i < attribs.Length; i++)
                            {
                                var ad = attribs[i];
                                if (ad.AttributeClass.Equals(KeyAttribute, SymbolEqualityComparer.Default))
                                {
                                    fi.isKey = true;
                                }
                                if (ad.AttributeClass.Equals(XAttribute, SymbolEqualityComparer.Default))
                                {
                                    fi.isX = true;
                                }
                                //if (ad.AttributeClass.Equals(DataGenerator.RangeAttribute, SymbolEqualityComparer.Default))
                                //{
                                //    ImmutableArray<TypedConstant> args = ad.ConstructorArguments;
                                //    fi.Min = (float)args[0].Value;
                                //    fi.Max = (float)args[1].Value;
                                //    //foreach (KeyValuePair<string, TypedConstant> namedArgument in ad.NamedArguments)
                                //    //{
                                //    //    if (namedArgument.Key == "Min")
                                //    //    {
                                //    //        var min = namedArgument.Value.Value;
                                //    //        fi.Min = (float)min;
                                //    //    }
                                //    //    if (namedArgument.Key == "Max")
                                //    //    {
                                //    //        var min = namedArgument.Value.Value;
                                //    //        fi.Min = (float)min;
                                //    //    }
                                //    //}
                                //}
                            }
                        }
                    }
                }
            }
            Fields.Add(fi);
        }
    }
    static bool IsAutoProperty(IPropertySymbol propertySymbol)
    {
        // Get fields declared in the same type as the property
        var fields = propertySymbol.ContainingType.GetMembers().OfType<IFieldSymbol>();

        // Check if one field is associated to
        return fields.Any(field => SymbolEqualityComparer.Default.Equals(field.AssociatedSymbol, propertySymbol));
    }
}