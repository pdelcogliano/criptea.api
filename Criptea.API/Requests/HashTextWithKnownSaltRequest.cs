using System;
using System.ComponentModel.DataAnnotations;

namespace Criptea.API.Requests
{
    public class HashTextWithKnownSaltRequest
    {
        [Required(ErrorMessage = "TextToHash is req'd.")]
        public string TextToHash { get; set; }

        [Required(ErrorMessage = "SaltWithHashIteration is req'd.")]
        public string SaltWithHashIteration { get; set; }
    }
}
