using System;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;

using MyDemoIssuer.Schemas;

using MyIssuerDemo.Data;


using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Routing;






// Issuer Inbound 
// Web services that issuer should expose to complete provisionning request 

namespace MyDemoIssuer
{






    [ApiController]
    [Route("[controller]")]
    public class IssuerController : ControllerBase
    {



        private readonly ILogger<IssuerController> _logger;

        private readonly List<FundingAccountData> accounts;  

        public IssuerController( ILogger<IssuerController> logger)
        {

            _logger = logger;

            string jsonString1 = @"
            {
                ""cardData"": {
                    ""accountNumber"": ""5555555555554444"", 
                    ""expiryDate"": ""12/28"",
                    ""securityCode"": ""123""
                },
                ""accountHolderData"": {
                    ""accountHolderName"": ""Jam One"", 
                    ""accountHolderAddress"": {
                        ""line1"": ""123 Main St"",
                        ""line2"": ""Apt 4B"",
                        ""city"": ""SETTAT"",
                        ""postalCode"": ""26000"",
                        ""country"": ""MAR""
                    },
                    ""accountHolderEmailAddress"": ""jam.one@example.com"",
                    ""accountHolderMobilePhoneNumber"": {
                        ""countryDialInCode"": ""+212"",
                        ""phoneNumber"": ""555-1234""
                    }
                }
            }";

            string jsonString2 = @"
            {
                ""cardData"": {
                    ""accountNumber"": ""4012888888881881"", 
                    ""expiryDate"": ""11/28"",
                    ""securityCode"": ""321""
                },
                ""accountHolderData"": {
                    ""accountHolderName"": "" Cool Jam"",
                    ""accountHolderAddress"": {
                        ""line1"": ""456 Elm St"",
                        ""line2"": ""Suite 5"",
                        ""city"": ""Othertown"",
                        ""postalCode"": ""17500"",
                        ""country"": ""FRA""
                    },
                    ""accountHolderEmailAddress"": ""Cool.Jam@example.com"",
                    ""accountHolderMobilePhoneNumber"": {
                        ""countryDialInCode"": ""+33"",
                        ""phoneNumber"": ""555-5678""
                    }
                }
            }";
            List<string> stringaccounts = new List<string> { jsonString1, jsonString2 };

            accounts = stringaccounts.Select(json => System.Text.Json.JsonSerializer.Deserialize<FundingAccountData>(json)).ToList();
        }



        [HttpPost("api/v1/digitization/provisioningRequest")]
        //[Route("GetFundingAccounts")]
        public async Task<ActionResult<ProvisionningverificationResponse>> Verify(ProvisionningVerificaionRequest provisionningRequest)
        //public async Task<ActionResult<TestClass>> Verify(ProvisionningVerificaionRequest provisionningRequest)
        {

            _logger.LogInformation(" Get the ProvisionningVerificaionRequest  ");


            // Decrypt the JWE 

            // Search if the account number exist 

            FundingAccountData foundAccount = accounts.FirstOrDefault(root => root.cardData.accountNumber == provisionningRequest.fundingAccountData.cardData.accountNumber);
            if (foundAccount != null)
            {
                if (foundAccount.cardData.expiryDate!= provisionningRequest.fundingAccountData.cardData.expiryDate)
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.expiryDateVerifiationResult = "INVALID";
                    resp.SecurityCodeVerifiationResult = "P";
                    resp.decision = "DECLINE";

                    return Ok(resp);
                }

                if (foundAccount.cardData.securityCode != provisionningRequest.fundingAccountData.cardData.securityCode)
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.expiryDateVerifiationResult = "MATCH";
                    resp.SecurityCodeVerifiationResult = "INVALID";
                    resp.decision = "DECLINE";

                    return Ok(resp);
                }
                else
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.SecurityCodeVerifiationResult = "MATCH";
                    resp.phoneVerifiationResult = "MATCH";
                    resp.emailVerifiationResult = "MATCH";
                    resp.addressVerifiationResult = "MATCH";
                    resp.expiryDateVerifiationResult = "MATCH";

                    resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                    resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                    resp.accountHolderInfo = new AccountHolderInfo();

                    resp.accountHolderInfo.accountHolderEmailAddress = foundAccount.accountHolderData.accountHolderEmailAddress;

                    resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                    resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = foundAccount.accountHolderData.accountHolderMobilePhoneNumber.countryDialInCode;
                    resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = foundAccount.accountHolderData.accountHolderMobilePhoneNumber.phoneNumber;


                    return Ok(resp);
                }
            }
            else
            {
                // the card doesn't exist

                var resp = new ProvisionningverificationResponse();
                resp.decision = "DECLINE";

                return Ok(resp);
            }
        

            if (provisionningRequest.correlationId == "123456")     // Test 200 Valid Response 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }

            if (provisionningRequest.correlationId == "100001")     // Test 200 Valid Response 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }


            if (provisionningRequest.correlationId == "123457")     // Test 200 Valid Response AUTH REQUIRE 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "AUTHENTICATE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }


            if (provisionningRequest.correlationId == "111111")     // Test 200 Valid Response but missed field decision 
                                                                    // if onbehalf setup for risk assesement and decisioning, perform decision
            {

                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                //resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";

                return Ok(resp);
            }


            if (provisionningRequest.correlationId == "999000")     // Test DECLINE  
            {

                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "DECLINE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";

                return Ok(resp);
            }


            if (provisionningRequest.correlationId == "222222")     // Test 400 Error
            {


                // var test = new TestClass(); /* For test */

                return BadRequest();
            }

            if (provisionningRequest.correlationId == "333333")     // Test 500 Error
            {


                // var test = new TestClass(); /* For test */

               

                               var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "XXX";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                //resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";

                return Ok(resp);
            }
            else
            {

                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "INVALID";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";
                resp.expiryDateVerifiationResult = "INVALID";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                //resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }
        }



        [HttpPost("api/v1/digitization/deliverActivationcode")]

        public async Task<ActionResult<ActivationCodeDeliveryResponse>> DeliverActivationCode(ActivationCodeDeliveryRequest activationCodeDeliveryRequest)
        {
            _logger.LogInformation(" Get the  PDeliverActivationcode Request  ");

            var resp = new ActivationCodeDeliveryResponse();
            resp.responseId = activationCodeDeliveryRequest.requestId;
            return Ok(resp);


        }

        
        [HttpPost("api/v1/digitization/notifyTokenUpdate")]

        public async Task<ActionResult<NotificationTokenUpdateResponse>> NotifyTokenUpdate(NotificationTokenUpdateRequest notificationTokenUpdateRequest)
        {
            _logger.LogInformation(" Get the NotifyTokenUpdate  Request  ");

            var resp = new NotificationTokenUpdateResponse();
            resp.responseId = notificationTokenUpdateRequest.requestId;
            return Ok(resp);


        }


        [HttpGet("api/v1/digitization/getPushProvisioningData")]

        public async Task<ActionResult<TestData>> GetPushData ()
        {
            _logger.LogInformation(" Start Push Provisioning Data  ");

            TestData data;

            //using (StreamReader r = new StreamReader("C:\\Users\\Jamal\\source\\repos\\MyDemoIssuer\\MyDemoIssuer\\files\\address.json"))
            using (StreamReader r = new StreamReader("../../Data/testdata.json"))
            {
                string json = r.ReadToEnd();
                 data = JsonConvert.DeserializeObject<TestData>(json);
            }

            return Ok(data);


        }

        [HttpGet("api/v1/digitization/testdata")]

        public async Task<ActionResult<TestData>> data(FundingAccountData accountData)
        {
            return Ok(new TestData());

        }

        }
}
