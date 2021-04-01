using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;

using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace LambdaDynamoRetrievalExample
{

    [Serializable]
    public class Item
    {
        public decimal rating;
        public string type;
        public int count;
    }

    public class Function
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();
        private static string tableName = "RatingsByType";

        /// <summary>
        /// A simple function that takes a string and does a ToUpper
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task<string> FunctionHandler(APIGatewayProxyRequest input, ILambdaContext context)
        {

            string itemId = "";
            Dictionary<string, string> dict = (Dictionary<string, string>)input.QueryStringParameters;
            dict.TryGetValue("type", out itemId);
            GetItemResponse res = await client.GetItemAsync(tableName, new Dictionary<string, AttributeValue>
            {
                { "type", new AttributeValue { S = itemId } }
            });

            Document myDoc = Document.FromAttributeMap(res.Item);
            Item myItem = JsonConvert.DeserializeObject<Item>(myDoc.ToJson());

            //return myItem;

            //return "{ " +myItem.company + " " + myItem.description + " }"; 
            //return JsonConvert.SerializeObject(myItem.count.ToString() + myItem.rating.ToString());
            return "{" + $"\n count: {myItem.count},\n averageRating: {myItem.rating / myItem.count} "+ "}";
        }
    }
}
