namespace MyDemoIssuer.Schemas
{


    public class ActivationMethod
    {
        public string? type { get; set; }
        public string? value { get; set; }
    }

    public class ActivationCodeDeliveryRequest
    {
        public string? requestId { get; set; } // "123456",

        public string? tokenUniqueReference { get; set; } // "DWSPMC000000000132d72d4fcb2f4136a0532d3093ff1a45",

        public string? correlationId { get; set; } //: "D98765432104",

        public string? activationCode { get; set; } //: "A1B2C3D4",

        public string? expirationDateTime { get; set; } // : "2016-07-04T12:08:56.123-07:00",

        public ActivationMethod? activationMethod { get; set; }
        public List<string>? reasonCodes { get; set; }  //   "reasonCodes": [   "ADD_CARD" ]



    }

    public class ActivationCodeDeliveryResponse
    {
        public string? responseId { get; set; }


    }




}




