using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amazon;
using Amazon.Lambda.Core;
using Amazon.Runtime.Internal.Util;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AwsHelloWorldMail
{
    public class WordsWriter
    {
        [LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]
        public async Task<string> SayHello()
        {
            try
            {
                var awsAccessKeyId = "";
                var awsSecretAccessKey = "";
                var client = new AmazonSimpleEmailServiceClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1);

                var source = "o.shumeliuk@levi9.com";
                var destination = new Destination(new List<string> { "o.shumeliuk@levi9.com" });
                var message = new Message(new Content("Hello from AWS Lambda"), new Body(new Content("Body printed by AWS Lambda: Hello world")));
                var resp = await client.SendEmailAsync(new SendEmailRequest(source, destination, message));
                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ArgumentException($"Http code expected to be OK but found {resp.HttpStatusCode.ToString()}. Response MessageId: {resp.MessageId}");
                }
                return "Hello World!";
            }
            catch (Exception e)
            {
                LambdaLogger.Log("SayHello throws and error" + e.Message);
                //Logger.GetLogger(typeof(WordsWriter)).Error(e, "ERROR!");
                throw;
            }
        }
    }
}
