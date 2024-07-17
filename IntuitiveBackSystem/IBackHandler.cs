namespace IntuitiveBackSystem
{
    public interface IBackHandler
    {
        public string ToolTip { get; }
        public void OnBack();
    }
}