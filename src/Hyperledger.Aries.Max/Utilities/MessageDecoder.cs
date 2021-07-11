using System;
using System.Net.Http;
using System.Threading.Tasks;
using Hyperledger.Aries.Agents;
using Hyperledger.Aries.Extensions;
using Hyperledger.Aries.Features.DidExchange;
using Hyperledger.Aries.Features.IssueCredential;
using Hyperledger.Aries.Features.PresentProof;
using Microsoft.AspNetCore.WebUtilities;

namespace Hyperledger.Aries.Max.Utilities
{
    public static class MessageDecoder
    {
        /// <inheritdoc />
        /// <inheritdoc />
        public static async Task<AgentMessage> ParseMessageAsync(string value)
        {
            string messageDecoded = null;
            if (value.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("https", StringComparison.OrdinalIgnoreCase)
                || value.StartsWith("didcomm", StringComparison.OrdinalIgnoreCase))
            {
                var url = new Uri(value);
                var query = QueryHelpers.ParseQuery(url.Query);
                if (query.TryGetValue("c_i", out var messageEncoded) ||
                    query.TryGetValue("d_m", out messageEncoded) ||
                    query.TryGetValue("m", out messageEncoded))
                {
                    messageDecoded = Uri.UnescapeDataString(messageEncoded);
                }
                else
                {
                    var client = new HttpClient(new HttpClientHandler { AllowAutoRedirect = false });
                    var response = await client.GetAsync(value);
                    var invitationUri = response.Headers.Location;
                    query = QueryHelpers.ParseNullableQuery(invitationUri.Query);

                    if (query.TryGetValue("c_i", out messageEncoded) ||
                        query.TryGetValue("d_m", out messageEncoded) ||
                        query.TryGetValue("m", out messageEncoded))
                    {
                        messageDecoded = Uri.UnescapeDataString(messageEncoded);
                    }
                }
            }
            else
            {
                messageDecoded = Uri.UnescapeDataString(value);
            }

            // Because the decoder above strips the +
            // https://github.com/aspnet/HttpAbstractions/blob/bc7092a32b1943c7f17439e419d3f66cd94ce9bd/src/Microsoft.AspNetCore.WebUtilities/QueryHelpers.cs#L165
            messageDecoded = messageDecoded.Replace(' ', '+');

            var json = messageDecoded.GetBytesFromBase64().GetUTF8String();
            var unpackedMessage = new UnpackedMessageContext(json, senderVerkey: null);

            switch (unpackedMessage.GetMessageType())
            {
                case MessageTypes.ConnectionInvitation:
                    return unpackedMessage.GetMessage<ConnectionInvitationMessage>();
                //case MessageTypes.EphemeralChallenge:
                //    return unpackedMessage.GetMessage<EphemeralChallengeMessage>();
                case MessageTypes.PresentProofNames.RequestPresentation:
                    return unpackedMessage.GetMessage<RequestPresentationMessage>();
                case MessageTypes.IssueCredentialNames.OfferCredential:
                    return unpackedMessage.GetMessage<CredentialOfferMessage>();
            }
            return null;
        }
    }
}
