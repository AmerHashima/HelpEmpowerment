namespace StandardArticture.Enums
{
    public enum RequestStatus
    {
        Pending = 0,
        Sent = 1,
        Success = 2,
        Failed = 3,
        Timeout = 4
    }
    public enum NphiesRequestTypes
    {
        EligibilityRequest,
        PreAuthorizationRequest,
        ClaimRequest,
        ClaimStatusRequest
    }
}
