using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;

namespace Genilog_WebApi.Repository
{
    public class WebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, WebSocket> ConnectedClients = new();
        private static readonly ConcurrentDictionary<string, List<string>> OrderGroups = new();


        public static async Task HandleConnection(WebSocket webSocket, HttpContext context)
        {
            string connectionId = Guid.NewGuid().ToString();
            ConnectedClients[connectionId] = webSocket;
            Console.WriteLine($"New connection: {connectionId}");

            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    ProcessMessage(connectionId, message);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Connection closed", CancellationToken.None);
                    ConnectedClients.TryRemove(connectionId, out _);
                    Console.WriteLine($"Connection {connectionId} closed");
                    // Remove socket if it is closed
                    RemoveSocket(connectionId);
                    // Reconnect logic if the socket is closed
                    await ReconnectAsync(connectionId);
                }
            }
        }

        private static void ProcessMessage(string connectionId, string message)
        {
            try
            {
                var msgObj = JsonConvert.DeserializeObject<dynamic>(message);
                string? action = msgObj?.action;

                switch (action)
                {
                    case "JoinOrderTracking":
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        JoinOrderTracking(connectionId, msgObj.orderId.ToString());
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        break;
                    case "SubscribeGasStation":
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                        BroadcastGasStation(msgObj.data);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                        break;
                    default:
                        Console.WriteLine("Unknown action received.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing message: {ex.Message}");
            }
        }

        public static async Task SendOrderToGroup(string orderId, object newOrder)
        {
            if (OrderGroups.TryGetValue(orderId, out var connections))
            {
                string json = JsonConvert.SerializeObject(new { eventType = "ORDER", data = newOrder });
                foreach (var connId in connections)
                {
                    if (ConnectedClients.TryGetValue(connId, out var webSocket))
                    {
                        await SendMessage(webSocket, json);
                    }
                }
            }
        }

        public static async Task SendOrderToLocation(string orderId, object newOrder)
        {
            if (OrderGroups.TryGetValue(orderId, out var connections))
            {
                string json = JsonConvert.SerializeObject(new { eventType = "LOCATION", dataPoint = newOrder });
                foreach (var connId in connections)
                {
                    if (ConnectedClients.TryGetValue(connId, out var webSocket))
                    {
                        await SendMessage(webSocket, json);
                    }
                }
            }
        }

        public static void JoinOrderTracking(string connectionId, string orderId)
        {
            if (!OrderGroups.ContainsKey(orderId))
                OrderGroups[orderId] = [];

            OrderGroups[orderId].Add(connectionId);
            Console.WriteLine($"Connection {connectionId} joined order {orderId}");
        }


        // THIS IS FOR GAS STATION
        // 🚀 Broadcasts  gas station to **ALL** connected clients
        public static async Task BroadcastGasStation(object gasStationData)
        {
            string json = JsonConvert.SerializeObject(new { eventType = "NEW_GAS_STATION", data = gasStationData });

            foreach (var client in ConnectedClients.Values)
            {
                await SendMessage(client, json);
            }
        }

        private static async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State == WebSocketState.Open || socket.State == WebSocketState.CloseReceived)
            {
                var data = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(new ArraySegment<byte>(data), WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                Console.WriteLine("WebSocket is not in a valid state to send a message.");
            }
        }

        // Method to handle disconnections
        public static void RemoveSocket(string connectionId)
        {
            if (ConnectedClients.TryGetValue(connectionId, out WebSocket? socket))
            {
                if (socket.State != WebSocketState.CloseReceived && socket.State != WebSocketState.Closed)
                {
                    socket.Abort();
                }
                ConnectedClients.TryRemove(connectionId, out _);
                Console.WriteLine($"Removed connection: {connectionId}");
            }
        }

        // Method to handle reconnections
        public static async Task ReconnectAsync(string connectionId)
        {
            // You can provide the actual WebSocket server URI here.
            string serverUri = "ws://api-data.ginilog.com/ws"; // Replace with your WebSocket server URI
            Console.WriteLine($"Attempting to reconnect for connection: {connectionId}");

            try
            {
                // Create a new WebSocket connection
                var webSocket = new ClientWebSocket();
                await webSocket.ConnectAsync(new Uri(serverUri), CancellationToken.None);

                // Add the new connection to the clients dictionary
                ConnectedClients[connectionId] = webSocket;
                Console.WriteLine($"Reconnected {connectionId}");

                // Optionally, you can send a message or join the order group after reconnecting
                // Example: JoinOrderTracking(connectionId, "your-order-id");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reconnecting: {ex.Message}");
            }
        }
    }
}
