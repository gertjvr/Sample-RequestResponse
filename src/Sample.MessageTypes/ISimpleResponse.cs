namespace Sample.MessageTypes
{
    public interface ISimpleResponse
    {
        string CustomerName { get; }
    }
    
    public class SimpleResponse : ISimpleResponse
    {
        public SimpleResponse(string customerName)
        {
            CustomerName = customerName;
        }
        
        public string CustomerName { get; }
    }
}