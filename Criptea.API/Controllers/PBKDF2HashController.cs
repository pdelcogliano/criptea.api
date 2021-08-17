using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Criptea.API.Requests;
using Criptea.API.Responses;
using Criptea.API.Services;
using System;
using System.Net.Mime;

namespace Criptea.API.Controllers
{
    [Route("api/v{version:apiVersion}/hashText")]
    [ApiVersion("1.0")]
    [Produces(MediaTypeNames.Application.Json)]
    [Consumes(MediaTypeNames.Application.Json)]
    [ApiController]
    public class PBKDF2HashController : ControllerBase
    {
        private readonly ILogger<PBKDF2HashController> _logger;
        private readonly IPBKDF2Service _pbkdf2Service;

        public PBKDF2HashController(ILogger<PBKDF2HashController> logger, IPBKDF2Service pbkdf2Service)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
            _pbkdf2Service = pbkdf2Service ??
                throw new ArgumentNullException(nameof(pbkdf2Service));
        }

        [HttpGet(Name = "HashText")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [MapToApiVersion("1.0")]
        public IActionResult HashText(string hashedText, string salt)
        {
            throw new NotImplementedException("Get");
        }

        /// <summary>
        /// hashes a string 
        /// </summary>
        /// <param name="request">an instance of HashToTextRequest</param>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST api/v1/hashText
        ///         {
        ///             "hashIterations": "100",
        ///             "textToHash": "This is a test",
        ///             "saltSize": "34"
        ///         }
        /// </remarks>
        /// <response code="201">Success - text hashed</response>
        /// <response code="400">Invalid request</response>
        /// <returns></returns>
        [HttpPost(Name = "HashText")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [MapToApiVersion("1.0")]
        public IActionResult HashText([FromBody] HashTextRequest request)
        {
            _logger.LogInformation($"hash text request for : {request.TextToHash}");
            if (request == null || !ModelState.IsValid)
            {
                return new BadRequestResult();
            }

            bool serverError = false;
            HashTextResponse response = new();

            try
            {
                Tuple<string, string> result = _pbkdf2Service.HashText(request.TextToHash, request.SaltSize, request.HashIterations);
                response.HashedText = result.Item1;
                response.Salt = result.Item2;
            }
            catch (Exception e)
            {
                serverError = true;
                _logger.LogError(e, "Error creating hashed text value");
            }

            if (serverError)
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);

            return new CreatedAtRouteResult(nameof(HashText), new { response.HashedText, response.Salt }, response);
        }
    }
}