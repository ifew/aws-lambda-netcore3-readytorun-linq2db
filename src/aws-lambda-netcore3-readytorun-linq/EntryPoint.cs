using Amazon.Lambda.APIGatewayEvents;
using aws_lambda_lambdanative;

namespace LambdaNative
{
    public static class EntryPoint
    {
        public static void Main()
        {
            LambdaNative.Run<Handler, APIGatewayProxyRequest, APIGatewayProxyResponse>();
        }
    }
}
