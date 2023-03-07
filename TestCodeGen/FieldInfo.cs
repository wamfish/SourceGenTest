//  Copyright (c) 2023-present John Roscoe Hamilton
namespace TestCodeGen;
class FieldInfo
{
    public string Type = string.Empty;
    public string Name = string.Empty;
    public string Initialization = string.Empty;
    public bool isPublic = false;
    public bool isPrivate => !isPublic;
    public bool isConst = false;
    public bool isStatic = false;
    public bool isReadonly = false;
    public bool isProperty = false;
    public bool isAutoProperty = false;
    public bool isKey = false;
    public bool isX = false;
}