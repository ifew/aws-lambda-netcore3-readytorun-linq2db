using System;
using System.Collections.Generic;
using Amazon.Lambda.Core;
using Amazon.Lambda.Serialization.Json;
using Amazon.Lambda.APIGatewayEvents;
using System.Net;
using LambdaNative;
using MySql.Data.MySqlClient;
using System.Data;
using LinqToDB;
using LinqToDB.Common;
using LinqToDB.Mapping;
using LinqToDB.Configuration;
using System.Linq;
using LinqToDB.Data;
using Newtonsoft.Json;

namespace aws_lambda_lambdanative
{
    public class Handler : IHandler<APIGatewayProxyRequest, APIGatewayProxyResponse>
    {
        public ILambdaSerializer Serializer => new Amazon.Lambda.Serialization.Json.JsonSerializer();

        public APIGatewayProxyResponse Handle(APIGatewayProxyRequest request, ILambdaContext context)
        {
            string unit_id = null;
            string lang = "THA";

            if (request.PathParameters != null && request.PathParameters.ContainsKey("unit_id")) {
                    unit_id = request.PathParameters["unit_id"];
            }
            if(request.QueryStringParameters != null && request.QueryStringParameters.ContainsKey("lang")) {
                lang = request.QueryStringParameters["lang"];
            }
            
            Console.WriteLine("Log: Start Connection");

            DataConnection.DefaultSettings = new MySettings();

            using (var db = new lab())
            {
                var query = from m in db.Member
                            orderby m.Id descending
                            select m;
                List<Member> members = query.ToList();

                APIGatewayProxyResponse respond = new APIGatewayProxyResponse
                {
                    StatusCode = (int)HttpStatusCode.OK,
                    Headers = new Dictionary<string, string>
                    {
                        { "Content-Type", "application/json" },
                        { "Access-Control-Allow-Origin", "*" },
                        { "X-Debug-UnitId", unit_id },
                        { "X-Debug-Lang", lang },
                    },
                    Body = JsonConvert.SerializeObject(members)
                };

                return respond;
            };
        }
    }


    [Table(Name = "test_member")]
    public class Member
    {
        [PrimaryKey, Identity]
        [Column(Name = "id"), NotNull]
        public int Id { get; set; }

        [Column(Name = "firstname"), NotNull]
        public string Firstname { get; set; }

        [Column(Name = "lastname"), NotNull]
        public string Lastname { get; set; }

    }

    public class ConnectionStringSettings : IConnectionStringSettings
    {
        public string ConnectionString { get; set; }
        public string Name { get; set; }
        public string ProviderName { get; set; }
        public bool IsGlobal => false;
    }

    public class MySettings : ILinqToDBSettings
    {
        public IEnumerable<IDataProviderSettings> DataProviders => Enumerable.Empty<IDataProviderSettings>();

        public string DefaultConfiguration => "MySQL";
        public string DefaultDataProvider => "MySQL";

        public IEnumerable<IConnectionStringSettings> ConnectionStrings
        {
            get
            {
                yield return
                    new ConnectionStringSettings
                    {
                        Name = "MySQL",
                        ProviderName = "MySQL",
                        ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION")
                    };
            }
        }
    }

    public class lab : LinqToDB.Data.DataConnection
    {
        public lab() : base("lab") { }

        public ITable<Member> Member => GetTable<Member>();
    }
}