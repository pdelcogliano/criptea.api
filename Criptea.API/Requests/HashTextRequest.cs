using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Criptea.API.Requests
{
    public class HashTextRequest
    {
        [Required(ErrorMessage = "HashIterations is req'd. default value is 10,000")]
        public int HashIterations { get; set; }

        [Required(ErrorMessage = "TextToHash is req'd.")]
        public string TextToHash { get; set; }

        [Required(ErrorMessage = "SaltSize is req'd.")]
        public int SaltSize { get; set; }
    }
}
