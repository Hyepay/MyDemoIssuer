namespace MyDemoIssuer.Schemas
{


    public class ErrorMessage
    {
        public string? responseId { get; set; }
        public string? errorCode { get; set; }
        public string? errorDescription { get; set; }
    }
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

        public AccountHolderData? accountHolderData { get; set; }
    }

    public class AccountHolderData
    {
        public string? accountHolderName { get; set; }
        public AccountHolderAddress? accountHolderAddress { get; set; }

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
            public string? securityCodeVerificationResult    { get; set; }
            public string? addressVerificationResult         { get; set; }
            public string? emailVerificationResult           { get; set; }
            public string? phoneVerificationResult           { get; set; }

            public string? expiryDateVerificationResult { get; set; }


            public string? accountStatus                    { get; set; }   //optionnal 

            public string? decision                         { get; set; }  //optionnal 

            public AccountHolderInfo? accountHolderInfo { get; set; }


    }

    /*
     *  {   "issuerId":"906b8d90-3060-4bf7-a503-dd63167950a1",
            "requestId":"6bce9f87-d7b0-4394-a19a-210b3cbda614",
            "correlationId":"4740924917750079604",
            "encryptedFundingPANInfo":"eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZHQ00iLCJraWQiOiIxMjM0NTY3OTEyMzY1NDc5In0.eW1lkj_wx7fPR1e32lDR07y8JD-gm5Nv6dMWfyJSE9pwWLdjOI6AXfm3WW1rO3ouwhGWjH1Mpt6J98sfvfga7FG-y5gpmMluJLfy6b5YNUlKWHwgJ_Qda25RoUtIuyhO-HL41K4qypt0knyak4tTPHU0BDIk_EeaB1iqXeiastiGDX04vjO9zv42Bl4loOJYk5ulesKoYGmoafZEmlle9dBVpcIM72ooxWz0O7afA7KVl0JOCBkgvYNnmpvBDUSPCNGAUx4_5CzpcPsZuboCe5lBo5UYbPTYMVTTwoznv1AC6obuSUGqgskoj5aUUBBxdM9pbddE37OquCvM50loOA.YiPEwr11GRhohDNq.QKla4I69APi8URB9RgIhid0JudqZ3DbtY5Hbnyh5P2NCvxKG6esAdRsLKAQp-Rf7U4_joWoT2M7BM9jWerIdoNmcjehXIHeVtQGLs5rwokBwKW_RVxTYAJTRphmTwJot8z0jIrbCJ9laX9wApTcICVqD7CzBHZ6TY0eXzZdh88M723w-zMt2KOTCoN-7dr9G0uZSPUlVehUtZ396plBCQ5heJ39aTtoMASx3xTucjziVXfsZnGylhYqD9S5E7lsNvSNsjpCScfveS6nBqPTHAS7XvT4DYx2v_nW_23EjYSjej-JRsRF72NXLZZHTpHbpJ3XW1-iaHX_wRR7cpenEKDm9RLEIDNV8LxyQuMDsyiKepRj0z0OBuEV122WkaNfYu5ixVxkbFPB2aO7oBIT_NcEMrx4jRNIaIgzg8PmwJB-gXd9-CJCjm01kqgg.YzC7OXF3wqd1huRkq6_ZuA"
            }  
And when you decrypt the encryptedFundingPANInfo, you will get this string that contains card and accountholder data:
{"cardData":{"accountNumber":"4012888888881881","expiryDate":"1128","securityCode":"321"},
"accountHolderData":{
"accountHolderName":"Salma",
"accountHolderAddress":{"Line1":"Victoria 1","Line2":null,"City":"Lagos","PostalCode":"12555","Country":"NG"},
"accountHolderEmailAddress":"salma@somemail.com",
"accountHolderMobilePhoneNumber":null}}  
 \"encryptedFundingPANInfo\":\"eyJhbGciOiJSU0EtT0FFUCIsImVuYyI6IkEyNTZHQ00iLCJraWQiOiIxMjM0NTY3OTEyMzY1NDc5In0.2-0fUJN29OFJR2c3OSYQINOe90tOyA0oZawer8oQJYu_CQ6fXaCXbZ1AQBLeI9i1hw2Xs6wGM2azW4BG_2rxcoGOkyh2zJPykJLH44ZAYv3J9RAIqqxKAmQlVD5ZUUZYDZghaK2Wb-4dJmyKD7-6keAak6312oxjpIRrT_g5qRq8CXU_jmSLe1dEqLm45vKyZaNyYLEXDitRV36xWprQpgpI-q3Uyq54r7kN_dqK2GNQkooS_C96ZXcV-4EdybcbrudIOZ8oLlVLspMFy6jrvUKZjhKvnod13-PzuixgUNQ6kFMrQiqWpBhQpq4RGcDWrL3SHoDzpsiZAz4C44wO1Q.jBgT0HngRMTZZZNo.6AEWqg3q3s97A238OrxAfXWoQE772ErtBa676wupL3Uxei_wz3VUsP0LjhZzpvgLh2BrUBBbdtr32N7m9atj3wQI74ojQxY5M8tuKZtK2K40Kltk2kTkxY1mlNObOSX6Q-F9QpsLuCerV_WUv_dFaxlY-f7uz2-LSVZeavYGYHoWyA8qK9ycuk8cb7UfPgpBzYsEQ9_L5kFxQJkbP0CYhSxuOoxUPOFYOq-0watrvu_UrYAsAWcKr-Jm2EgxI_ERdYmAw0jzzdXVxVpjVz-izOEYJws7z_dI4wiOVSc6s87w7s8GMG14oKLdPPN4SzL5sOLttYh0ARfD4FiR0EGDxUB_rxoRagVYHKOYTW4fm-24FoyG0ba0zbRSHkfDeQHb-1yRBib43KEp3gKIziQikC2nd89TauxUz95NMg.FzaqXaU0sFLuhXYjBWPP4A\"}"
     * */

    public class InboundIssuerGWProvisionningVerificaionRequest
    {

        public string issuerId { get; set; }
        public string requestId { get; set; }
        public string correlationId { get; set; }

        public string encryptedFundingPANInfo { get; set; }


        public InboundIssuerGWProvisionningVerificaionRequest()
        {

        }

    }


    public class GoogleWalletAddress
    {
            public string? name { get; set; }
            public string? address { get; set; }
            public string? locality { get; set; }
            public string? administrativeArea { get; set; }
        
            public string? countryCode { get; set; }

            public string? postalCode { get; set; }
            public string? phoneNumber { get; set; }
  

    }

    public class TestData
    {
        public string? opc { get; set; }

        public CardData? card { get; set; }

        public string? network { get; set; }

        public string? cardHolderName { get; set; }
        public GoogleWalletAddress? address { get; set; }
    }


  

}




