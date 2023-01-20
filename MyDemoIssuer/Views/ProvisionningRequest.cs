namespace MyDemoIssuer.Schemas
{
    public class AccountHolderMobilePhoneNumber
    {
        public string? countryDialInCode { get; set; }
        public string? phoneNumber { get; set; }
    }

    public class AccountHolderAddress
    {
        public string Line1 { get; set; }
        public string? Line2 { get; set; }
        public string? City { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
    }

    public class CardData
    {
        public string accountNumber { get; set; }
        public string expiryDate { get; set; }
        public string securityCode { get; set; }
    }

    public class FundingAccountData
    {
        public CardData cardData { get; set; }

        public AccountHolderData accountHolderData { get; set; }
    }

    public class AccountHolderData
    {
        public string? accountHolderName { get; set; }
        public AccountHolderAddress accountHolderAddress { get; set; }

        public string? accountHolderEmailAddress { get; set; }
        public AccountHolderMobilePhoneNumber? accountHolderMobilePhoneNumber { get; set; }
    }

    public class AccountHolderInfo
    {

        public string? accountHolderEmailAddress { get; set; }
        public AccountHolderMobilePhoneNumber? accountHolderMobilePhoneNumber { get; set; }
    }

    public class ProvisionningVerificaionRequest {

            public string  issuerId { get; set; }
            public string  requestId { get; set; }

            public string   correlationId { get; set; }
        public FundingAccountData fundingAccountData { get; set; }


        public ProvisionningVerificaionRequest ()
        {

        }

        }

        public class ProvisionningverificationResponse
        {
            public string? SecurityCodeVerifiationResult    { get; set; }
            public string? addressVerifiationResult         { get; set; }
            public string? emailVerifiationResult           { get; set; }
            public string? phoneVerifiationResult           { get; set; }

            public string? accountStatus                    { get; set; }   //optionnal 

            public string? decision                         { get; set; }  //optionnal 

            public AccountHolderInfo? accountHolderInfo { get; set; }


    }



  

}




