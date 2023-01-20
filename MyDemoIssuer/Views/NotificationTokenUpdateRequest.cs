namespace MyDemoIssuer.Schemas
{

    public class Token
    {

        public string tokenUniqueReference { get; set; }
        public string status { get; set; }
        public string reasonCode { get; set; }

        public List<string>? suspendedBy { get; set; }
        public  DeviceInfo? deviceInfo { get; set; }

        public TokenRequestorInfo? tokenRequestorInfo { get; set; }

  
    }

public class NotificationTokenUpdateRequest
    {
        public string requestId { get; set; } // "123456",

        public string issuerId { get; set; }
    

        public string? correlationId { get; set; } //: "D98765432104",

        public List<Token> tokens { get; set; }  //   




    }

    public class NotificationTokenUpdateResponse
    {
        public string? responseId { get; set; }


    }




}




