using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NodaTime;
using NodaTime.Text;

namespace WorkerRoleWithSBQueue1
{
    internal class CubeMessageFactory : ICubeMessageFactory
    {
        public string Build(string regardEvent)
        {
            var messageBusObject = JsonConvert.DeserializeAnonymousType(regardEvent, new
                                                                                     {
                                                                                         schema_version = 0x000,
                                                                                         organization = String.Empty,
                                                                                         product = String.Empty,
                                                                                         payload = String.Empty
                                                                                     });

            var messageBusEvent = JObject.Parse(messageBusObject.payload);
            var session = messageBusEvent["session-id"].Value<string>();
            var user = messageBusEvent["user-id"].Value<string>();
            var eventtype = messageBusEvent["event-type"].Value<string>();
            var time = messageBusEvent["time"].Value<long>();

            JObject result = new JObject();
            JToken existingHash;
            result["data"] = GetCubeDataHash(session, user, messageBusEvent.TryGetValue("data", out existingHash) ? existingHash.Value<JObject>(): new JObject());
            result["type"] = String.Format("{0}.{1}.{2}", messageBusObject.organization, messageBusObject.product, eventtype).ToLowerInvariant().Replace('.', '_');
            result["time"] = InstantPattern.ExtendedIsoPattern.Format(Instant.FromMillisecondsSinceUnixEpoch(time));

            return JsonConvert.SerializeObject(new [] { result });
        }

        private JObject GetCubeDataHash(string sessionId, string userId, JObject eventDataHash)
        {
            var cubeDataHash = new JObject
                               {
                                   {"session-id", sessionId},
                                   {"user-id", userId},
                               };

            foreach (KeyValuePair<string, JToken> keyValuePair in eventDataHash)
            {
                cubeDataHash[GetCubeSafeName(keyValuePair.Key)] = keyValuePair.Value;
            }

            return cubeDataHash;
        }

        private static string GetCubeSafeName(string key)
        {
            return key.Replace('.', '_');
        }
    }
}