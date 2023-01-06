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

            var resp = new ProvisionningverificationResponse();

            // var test = new TestClass(); /* For test */

            resp.SecurityCodeVerifiationResult = "MATCH";
            resp.phoneVerifiationResult = "MATCH";
            resp.emailVerifiationResult = "MATCH";
            resp.addressVerifiationResult = "MATCH";

            resp.accountStatus = "ACTIVE"; // Possible value { "ACTIVE", "INACTIVE" }
            resp.decision = "APPROVE";  // Possible value { "APPROVE", "DECLINE", "AUTHENTICATE"  }

            resp.accountHolderInfo= new AccountHolderInfo();

            resp.accountHolderInfo.accountHolderEmailAddress = "yrtest@email.com";

            resp.accountHolderInfo.accountHolderMobilePhoneNumber = new AccountHolderMobilePhoneNumber();
            resp.accountHolderInfo.accountHolderMobilePhoneNumber.countryDialInCode = "+254";
            resp.accountHolderInfo.accountHolderMobilePhoneNumber.phoneNumber = "051110222"; 


            return Ok(resp);
            //return BadRequest(test); /* For test */
        }



        [HttpPost("IssuerDeliverActivationcode")]

        public async Task<ActionResult<ActivationCodeDeliveryResponse>> DeliverActivationCode(ActivationCodeDeliveryRequest activationCodeDeliveryRequest)
        {
            _logger.LogInformation(" Get the  PDeliverActivationcode Request  ");

            var resp = new ActivationCodeDeliveryResponse();
            resp.responseId = activationCodeDeliveryRequest.requestId;
            return Ok(resp);


        }



    }
}
