
// PM> Install-Package MQTTnet

using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Protocol;

using MQTTnet.Extensions.ManagedClient;
using System.Text;

Console.WriteLine("Hello, World!");

string broker = "localhost";
int port = 1883;
string clientId = Guid.NewGuid().ToString();
string topic = "csharp/mqtt";
string username = "test";
string password = "******";

// Create a MQTT client factory
var factory = new MqttFactory();

// Create a MQTT client instance
var mqttClient = factory.CreateMqttClient();

// Create Testament
var msg = new MqttApplicationMessageBuilder()
       .WithTopic(topic)
       .WithPayload($"Bye bye, MQTT!")
       .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
       .WithRetainFlag(false)
       .Build();


// Create MQTT client options
var options = new MqttClientOptionsBuilder()
    .WithTcpServer(broker, port) // MQTT broker address and port
 //   .WithCredentials(username, password) // Set username and password
    .WithClientId(clientId)
         .WithWillTopic(msg.Topic)   
         .WithWillPayload(msg.PayloadSegment)
         .WithWillRetain(msg.Retain)
         .WithWillPayloadFormatIndicator(msg.PayloadFormatIndicator)
         .WithWillContentType(msg.ContentType)
    .WithCleanSession()
    .Build();

var connectResult = await mqttClient.ConnectAsync(options);

if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
{
    Console.WriteLine("Connected to MQTT broker successfully.");

    // Subscribe to a topic
    await mqttClient.SubscribeAsync(topic);

    // Callback function when a message is received
    mqttClient.ApplicationMessageReceivedAsync += e =>
    {
        Console.WriteLine($"Received message: {Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment)}");
        return Task.CompletedTask;
    };
}

for (int i = 0; i < 10; i++)
{
    var message = new MqttApplicationMessageBuilder()
        .WithTopic(topic)
        .WithPayload($"Hello, MQTT! Message number {i}")
        .WithQualityOfServiceLevel(MqttQualityOfServiceLevel.AtLeastOnce)
        .WithRetainFlag()
        .Build();

    await mqttClient.PublishAsync(message);
    await Task.Delay(1000); // Wait for 1 second
}

await mqttClient.UnsubscribeAsync(topic);

await mqttClient.DisconnectAsync();



