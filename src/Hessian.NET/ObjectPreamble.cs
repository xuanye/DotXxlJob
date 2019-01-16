namespace Hessian.Net
{
    public enum ObjectPreamble
    {
        None = -1,
        ClassDefinition,
        ObjectReference,
        InstanceReference,
        
        VarList,
        FixList,
        VarListUntyped,
        FixListUntyped,
        CompactFixList,
        CompactFixListUntyped
    }
}