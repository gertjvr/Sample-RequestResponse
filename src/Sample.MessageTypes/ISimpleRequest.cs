namespace Sample.MessageTypes
{
    using System;
    
    public interface ISimpleRequest
    {
        /// <summary>
        /// When the request was created
        /// </summary>
        DateTime Timestamp { get; }

        /// <summary>
        /// The customer id for the request (or whatever data you want here)
        /// </summary>
        string CustomerId { get; }
    }
    
    public class SimpleRequest : ISimpleRequest
    {
        public SimpleRequest(string customerId, DateTime timestamp)
        {
            CustomerId = customerId;
            Timestamp = timestamp;
        }

        /// <summary>
        /// When the request was created
        /// </summary>
        public DateTime Timestamp { get; }

        /// <summary>
        /// The customer id for the request (or whatever data you want here)
        /// </summary>
        public string CustomerId { get; }
    }
}