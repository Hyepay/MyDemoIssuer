using System;
using System.IO.Pipelines;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;


using MyDemoIssuer.Schemas;

using MyIssuerDemo.Data;
using Mastercard.Developer.ClientEncryption.Core.Utils;

using Microsoft.AspNetCore.Mvc;
using System.Xml.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net;
using Microsoft.AspNetCore.Routing;

using Newtonsoft.Json.Linq;




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
        private readonly IConfiguration _config;

        public IssuerController( ILogger<IssuerController> logger, IConfiguration config)
        {

            _logger = logger;

            string jsonString1 = @"
            {
                ""cardData"": {
                    ""accountNumber"": ""5555555555554444"", 
                    ""expiryDate"": ""1228"",
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
                    ""expiryDate"": ""1128"",
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
            _config = config;
        }



        [HttpPost("api/v1/digitization/provisioningRequest")]
        //[Route("GetFundingAccounts")]
        //public async Task<ActionResult<ProvisionningverificationResponse>> Verify(InboundIssuerGWProvisionningVerificaionRequest provisionningRequest)
        public async Task<ActionResult<ProvisionningverificationResponse>> Verify(InboundIssuerGWProvisionningVerificaionRequest provisionningRequest)
        //public async Task<ActionResult<TestClass>> Verify(ProvisionningVerificaionRequest provisionningRequest)
        {

            _logger.LogInformation(" Get the ProvisionningVerificaionRequest  ");

            // Get Peer Ip & Port Information 
            // Get the IP address and port of the client
            string clientIpAddress = HttpContext.Connection.RemoteIpAddress.ToString();
            int clientPort = HttpContext.Connection.RemotePort;

            _logger.LogInformation(" Peer IP Adress : {0} - Peer Port: {1}  ", clientIpAddress, clientPort);

            ErrorMessage errorObj = new ErrorMessage();


            // Decrypt the JWE 

            _logger.LogInformation(" Decrypt Payload :{0}  ", provisionningRequest.encryptedFundingPANInfo);

            // decrypt the encryptedAccountInfo using hypepay private key

            var environment = _config.GetValue<string>("Env:Environment");
            var deployment = _config.GetValue<string>("Env:Deployment");
            RSA privateKey;
            if (environment == "Development")
                privateKey = EncryptionUtils.LoadDecryptionKey("C:\\Users\\opent\\source\\repos\\Hyepay\\IssuerGW\\certificate\\t2p\\private.key");
            else
                privateKey = EncryptionUtils.LoadDecryptionKey("../certificate/t2p/private.key");


            string stringFundingAccountInfo;
            try
            {


                stringFundingAccountInfo = Jose.JWT.Decode(provisionningRequest.encryptedFundingPANInfo, privateKey);

                // Manage Error if JWE couldn't be decoded
            }
            catch (Exception ex)
            {
                _logger.LogInformation("Exception: {0} ", ex.StackTrace);

                errorObj.responseId = provisionningRequest.requestId;
                errorObj.errorCode = "INVALID_JWE_PAYLOAD";
                errorObj.errorDescription = "Unable to decrypt the JWE";

                return BadRequest(errorObj);
            }

            _logger.LogInformation(" JWE decrypted token :   {0}  ", stringFundingAccountInfo);


            FundingAccountData fundingAccountData = System.Text.Json.JsonSerializer.Deserialize<FundingAccountData>(stringFundingAccountInfo);

            /*
            // Temporary Performance Test
            // Wait 350 ms and provide reponse 

            // Replace Thread.Sleep with non-blocking delay
            await Task.Delay(350);
            var resp = new ProvisionningverificationResponse();
            resp.securityCodeVerifiationResult = "MATCH";
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

            */


            // Search if the account number exist 

            FundingAccountData foundAccount = accounts.FirstOrDefault(root => root.cardData.accountNumber == fundingAccountData.cardData.accountNumber);
            if (foundAccount != null)
            {
                if (foundAccount.cardData.expiryDate!= fundingAccountData.cardData.expiryDate)
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.expiryDateVerificationResult = "INVALID";
                    resp.securityCodeVerificationResult = "NOT_PROCESSED";
                    resp.addressVerificationResult = "NOT_PROCESSED";
                    resp.accountStatus = "NOT_PROCESSED";
                    resp.emailVerificationResult = "NOT_PROCESSED";
                    resp.phoneVerificationResult = "NOT_PROCESSED";
                    resp.decision = "DECLINE";

                    return Ok(resp);
                }

                if (foundAccount.cardData.securityCode != fundingAccountData.cardData.securityCode)
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.expiryDateVerificationResult = "MATCH";
                    resp.securityCodeVerificationResult = "INVALID";
                    resp.addressVerificationResult = "NOT_PROCESSED";
                    resp.accountStatus = "NOT_PROCESSED";
                    resp.emailVerificationResult = "NOT_PROCESSED";
                    resp.phoneVerificationResult = "NOT_PROCESSED";
                    resp.decision = "DECLINE";

                    return Ok(resp);
                }
                else
                {
                    var resp = new ProvisionningverificationResponse();
                    resp.securityCodeVerificationResult = "MATCH";
                    resp.phoneVerificationResult = "MATCH";
                    resp.emailVerificationResult = "MATCH";
                    resp.addressVerificationResult = "MATCH";
                    resp.expiryDateVerificationResult = "MATCH";

                    resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                    resp.emailVerificationResult = "NOT_PROCESSED";
                    resp.phoneVerificationResult = "NOT_PROCESSED";
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
                resp.expiryDateVerificationResult = "NOT_PROCESSED";
                resp.securityCodeVerificationResult = "NOT_PROCESSED";
                resp.addressVerificationResult = "NOT_PROCESSED";
                resp.accountStatus = "NOT_PROCESSED";
                resp.emailVerificationResult= "NOT_PROCESSED";
                resp.phoneVerificationResult= "NOT_PROCESSED"; 
                resp.decision = "DECLINE";

                return Ok(resp);
            }
        
            /*
            if (provisionningRequest.correlationId == "123456")     // Test 200 Valid Response 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); 

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

                // var test = new TestClass(); 

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

                // var test = new TestClass(); 

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

                // var test = new TestClass(); 

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

                // var test = new TestClass(); 

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


                // var test = new TestClass(); 

                return BadRequest();
            }

            if (provisionningRequest.correlationId == "333333")     // Test 500 Error
            {


                // var test = new TestClass(); 

               

                               var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); 

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

                // var test = new TestClass(); 

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
            */
            //}
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
