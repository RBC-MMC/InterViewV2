namespace InterViewV2.Models
{
    public enum Interview_Application_Status
    {
        NewApplication = 1,
        ReviewedByManager_SuggestedForInitialInterview = 2,
        InvitedForInitialInterview = 3,
        PassedInitialInterview = 4,
        PassedSecondInterview = 5,
        FailedInitialInterview = 6,
        FailedSecondInterview = 7,
        JobOfferProvided = 8,
        Hired = 9,
        AddedToTalentPool = 10,
        ApplicationOnHold = 11,
        ApplicationRejected = 12
    }
}
