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






// Issuer Inbound 
// Web services that issuer should expose to complete provisionning request 

namespace MyDemoIssuer
{






    [ApiController]
    [Route("[controller]")]
    public class IssuerController : ControllerBase
    {



        private readonly ILogger<IssuerController> _logger;

        public IssuerController( ILogger<IssuerController> logger)
        {

            _logger = logger;

        }



        [HttpPost("api/v1/digitization/provisioningRequest")]
        //[Route("GetFundingAccounts")]
        public async Task<ActionResult<ProvisionningverificationResponse>> Verify(ProvisionningVerificaionRequest provisionningRequest)
        //public async Task<ActionResult<TestClass>> Verify(ProvisionningVerificaionRequest provisionningRequest)
        {

            _logger.LogInformation(" Get the ProvisionningVerificaionRequest  ");


            // Decrypt the JWE 

            // Check the data 

            if (provisionningRequest.requestId == "123456")     // Test 200 Valid Response 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }


            if (provisionningRequest.requestId == "123457")     // Test 200 Valid Response AUTH REQUIRE 
            {
                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                resp.decision = "AUTHENTICATE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";


                return Ok(resp);
            }


            if (provisionningRequest.requestId == "111111")     // Test 200 Valid Response but missed field decision
            {

                var resp = new ProvisionningverificationResponse();

                // var test = new TestClass(); /* For test */

                resp.SecurityCodeVerifiationResult = "MATCH";
                resp.phoneVerifiationResult = "MATCH";
                resp.emailVerifiationResult = "MATCH";
                resp.addressVerifiationResult = "MATCH";

                resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
                //resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

                resp.accountHolderInfo = new AccountHolderInfo();

                resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

                resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
                resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222";

                return Ok(resp);
            }

            if (provisionningRequest.requestId == "222222")     // Test 400 Error
            {


                // var test = new TestClass(); /* For test */

                return BadRequest();
            }

            if (provisionningRequest.requestId == "333333")     // Test 500 Error
            {


                // var test = new TestClass(); /* For test */

                return Problem();
            }


            return BadRequest(); /* For test */
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



    }
}
