using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Xamarin.Essentials;

using Microsoft.Azure.CognitiveServices.Vision.Face;
using FaceAuthApp.Helpers;

namespace FaceAuthApp.Services
{
    public class ServicioFace
    {
        private static FaceClient GetClient()
        {
            var credentials = new ApiKeyServiceClientCredentials(Constants.FaceApiKey);

            var client = new FaceClient(credentials);
            client.Endpoint = Constants.FaceApiEndpoint;

            return client;
        }

        public static async Task<string> GetGroupId(string groupName = "")
        {
            var groupId = Preferences.Get(Constants.GroupKey, string.Empty);

            if (string.IsNullOrWhiteSpace(groupId))
            {
                groupId = Guid.NewGuid().ToString();

                if (string.IsNullOrWhiteSpace(groupName))
                    groupName = "CursoXamarin";

                var client = GetClient();
                await client.PersonGroup.CreateAsync(groupId, groupName);

                Preferences.Set(Constants.GroupKey, groupId);
            }

            return groupId;
        }

        public static async Task<Guid> RegisterPerson(string personName)
        {
            var groupId = await GetGroupId();

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                var client = GetClient();

                //TODO: add Employee ID from database
                var userData = "1";

                var person = await client.PersonGroupPerson.CreateAsync(groupId, personName, userData);
                return person.PersonId;
            }

            return Guid.Empty;
        }

        public static async Task<Guid> RegisterFace(string personId, Stream stream)
        {
            stream.Position = 0;
            var groupId = await GetGroupId();

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                var personGuid = Guid.Parse(personId);
                var client = GetClient();

                var face = await client.PersonGroupPerson.AddFaceFromStreamAsync(groupId, personGuid, stream);
                return face.PersistedFaceId;
            }

            return Guid.Empty;
        }

        public static async Task<bool> TrainGroup()
        {
            var groupId = await GetGroupId();

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                var client = GetClient();
                await client.PersonGroup.TrainAsync(groupId);
                return true;
            }

            return false;
        }

        public static async Task<string> IdentifyPerson(Stream stream)
        {
            stream.Position = 0;

            var groupId = await GetGroupId();

            if (!string.IsNullOrWhiteSpace(groupId))
            {
                var client = GetClient();

                var faces = await client.Face.DetectWithStreamAsync(stream);

                if (faces.Count > 0)
                {
                    var firstFace = faces.First();
                    var faceIds = new List<Guid> { firstFace.FaceId.Value };

                    var identifyOp = await client.Face.IdentifyAsync(faceIds, groupId);

                    if (identifyOp.Count > 0)
                    {
                        var identifyResult = identifyOp.First();

                        if (identifyResult.Candidates.Count > 0)
                        {
                            var identifiedPerson = identifyResult.Candidates.First();
                            var identifiedPersonId = identifiedPerson.PersonId;

                            var person = await client.PersonGroupPerson.GetAsync(groupId, identifiedPersonId);

                            //TODO: return person.UserData;
                            return person.Name;
                        }
                    }
                }
            }

            return string.Empty;
        }
    }
}