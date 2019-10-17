namespace MergeO.Contracts
{
    public interface IContextRoot
    {
        object NewValue { get; set; }
        object OldValue { get; set; }
    }
}