namespace WFLib;

public abstract class Data  
{
    protected virtual void OnBaseConstruct() { }
    public virtual void OnInitialize() { }
    public virtual void OnLoad() { }
    public virtual void OnClear() { }
    public abstract void Clear();
    public abstract void Init();
    
    public Data()
    {
        OnBaseConstruct();
    }
}
