namespace Hyperledger.Aries.Max.Events
{
    public enum ApplicationEventType
    {
        ConnectionsUpdated,
        CredentialsUpdated,
        RefreshProofRequests
    }

    public class ApplicationEvent
    {
        public ApplicationEventType Type { get; set; }
    }
}
