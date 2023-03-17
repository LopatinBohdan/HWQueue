using Azure;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System.Text.Json;

namespace HWQueue.Models
{
    public class MyQueue
    {
        private string connStr = "DefaultEndpointsProtocol=https;AccountName=lopatinqueue;AccountKey=kaZGDFsoYCw0FCIJiz63eesCAcz2jfcLsYXEOU1ugY46rjPsyW3sZUSjW8cKPsLxIJizPR/KZi1G+AStcDrVZg==;EndpointSuffix=core.windows.net";
        private string queueTitle = "lotsqueue";
        public QueueServiceClient queueService;
        public QueueClient queueClient;

        public MyQueue() { 
            queueService=new QueueServiceClient(connStr);
            try
            {
                queueClient = queueService.CreateQueue(queueTitle);
            }
            catch (Exception)
            {
                queueClient = queueService.GetQueueClient(queueTitle);
            }
            
        }

        //Add message
        public async Task AddMessageAsync(string message)
        {
            
            queueClient = queueService.GetQueueClient(queueTitle);
            await queueClient.SendMessageAsync(message, timeToLive:TimeSpan.FromHours(24));
        }
        //Show All Messages
        public async Task<List<object>> ShowAllAsync()
        {
            List<object> mess=new List<object>();
            queueClient = queueService.GetQueueClient(queueTitle);
            foreach (PeekedMessage item in (await queueClient.PeekMessagesAsync(maxMessages:10)).Value)
            {
                mess.Add(new { lot = JsonSerializer.Deserialize<Lot>(item.Body.ToString()), id = item.MessageId });
            }
            return mess;
        }
        //Show target Currency
        public async Task<List<object>> ShowTarget(string ID)
        {
            List<object> mess = new List<object>();
            queueClient = queueService.GetQueueClient(queueTitle);
            foreach (PeekedMessage item in (await queueClient.PeekMessagesAsync(maxMessages: 10)).Value)
            {
                var tempLot = new { lot=JsonSerializer.Deserialize<Lot>(item.Body.ToString()), id = item.MessageId };
                if (tempLot.id == ID)
                {
                    mess.Add(tempLot);
                }
            }
            return mess;
        }

        //Del Lot
        public async Task DeleteMessage(string? id)
        {
            // await queueClient.DeleteMessageAsync(item.MessageId, item.PopReceipt);
            if (id != null)
            {
                var arr = (await queueClient.ReceiveMessagesAsync(maxMessages: 10, visibilityTimeout:TimeSpan.FromSeconds(1))).Value;
                var message=arr.Where(a=>a.MessageId==id).First();
                await queueClient.DeleteMessageAsync(message.MessageId, message.PopReceipt);
            }
        }

    }
}
