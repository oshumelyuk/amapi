using System;
using System.Collections.Generic;
using Amazon;
using Amazon.Runtime.Internal.Util;
using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;

namespace AwsHelloWorldMail
{
    public static class WordsWriter
    {
        public static void SayHello()
        {
            try
            {
                var awsAccessKeyId = "AKIAIDGJMLHYWVGBT7OA";
                var awsSecretAccessKey = "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";
                var client = new AmazonSimpleEmailServiceClient(awsAccessKeyId, awsSecretAccessKey, RegionEndpoint.USEast1);

                var source = "o.shumeliuk@levi9.com";
                var destination = new Destination(new List<string> { "o.shumeliuk@levi9.com" });
                var message = new Message(new Content("Hello from AWS Lambda"), new Body(new Content("Body printed by AWS Lambda: Hello world")));
                var resp = client.SendEmailAsync(new SendEmailRequest(source, destination, message))
                    .GetAwaiter().GetResult();
                if (resp.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new ArgumentException($"Http code expected to be OK but found {resp.HttpStatusCode.ToString()}. Response MessageId: {resp.MessageId}");
                }
            }
            catch (Exception e)
            {
                Logger.GetLogger(typeof(WordsWriter)).Error(e, "ERROR!");
                throw;
            }

        }
    }
}
