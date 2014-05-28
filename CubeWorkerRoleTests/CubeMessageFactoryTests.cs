using System;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using WorkerRoleWithSBQueue1;

namespace CubeWorkerRoleTests
{
    [TestFixture]
    public class CubeMessageFactoryTests
    {
        [Test]
        public void DatesArePreserved()
        {
            var cubeEvent = new CubeMessageFactory().Build(ExampleEvent);

            var jArray = JArray.Parse(cubeEvent);

            jArray.Count.Should().Be(1);

            JsonConvert.DeserializeAnonymousType(JsonConvert.SerializeObject(jArray[0]), new
                                                        {
                                                            type = String.Empty,
                                                            time = String.Empty
                                                        }).time.Should().Be("2014-05-28T09:12:58.719Z");
        }

        private static string ExampleEvent
        {
            get
            {
                var regardEvent = new JObject
                                  {
                                      {"session-id", ""},
                                      {"user-id", Guid.NewGuid().ToString()},
                                      {"event-type", "page.visit"},
                                      {"time", 1401268378719}
                                  };

                return JsonConvert.SerializeObject(new
                                                   {
                                                       organization = "regard",
                                                       product = "website",
                                                       schema_version = 0x100,
                                                       payload = JsonConvert.SerializeObject(regardEvent)
                                                   });
            }
        }
    }
}
