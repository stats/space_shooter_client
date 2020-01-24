public class QueuedMessage
{
    public string Message;
    public float Delay;

    public QueuedMessage(string message, float delay)
    {
        this.Message = message;
        this.Delay = delay;
    }
}