using System.Linq;

namespace Hyperledger.Aries.Max.Extensions
{   
    public static class StringExtensions
    {
        /// <summary>
        /// Following "schemasubmitterDid | SchemaMarker | schemaName | schemaVersion" format 
        /// to get Credential Name as described in   https://github.com/hyperledger/indy-node/blob/master/design/anoncreds.md#state
        /// </summary>        
        public static string ToCredentialName(this string schemaId)
        {
            if (schemaId == null)
                return string.Empty;

            string[] schemaName = schemaId.Split(new char[1] { ':' }).Skip(2).Take(2).ToArray();
            if (schemaName == null || schemaName?.Length <= 0)
                return string.Empty;

            string nameWithVersion = string.Format($"{schemaName[0]} {schemaName[1]}");
            return nameWithVersion;
        }
    }
}
