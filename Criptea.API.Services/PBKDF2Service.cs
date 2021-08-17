using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Criptea.API.Services
{
    public class PBKDF2Service : IPBKDF2Service
    {
        private readonly Serilog.ILogger _logger;

        public PBKDF2Service(Serilog.ILogger logger)
        {
            _logger = logger ??
                throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Compute the hash that will also generate a salt from parameters
        ///	</summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="saltSize">The size of the salt to be generated</param>
        /// <param name="hashIterations"></param>
        /// <returns>
        /// the computed hash: HashedText
        /// </returns>
        public Tuple<string, string> HashText(string textToHash, int saltSize, int hashIterations)
        {
            _logger.Information($"Compute: textToHash: '{textToHash}', saltSize: {saltSize}, hashIterations: {hashIterations}");

            if (string.IsNullOrEmpty(textToHash))
                throw new InvalidOperationException("text to hash cannot be empty");

            // generate salt
            string salt = GenerateSalt(hashIterations, saltSize);
            _logger.Information($"generated salt: {salt}");

            //compute the hash
            string hash = CalculateHash(textToHash, salt, hashIterations);
            _logger.Information($"hashed text: {hash}");

            return new Tuple<string, string>(hash, salt);
        }

        /// <summary>
        /// Compute the hash using the provided salt
        /// </summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="saltWithHashIteration">The salt to be used. must include the hash iterations value. String should be form of {Hash Iteration}.{Salt}</param>
        /// <returns>The computed hash: HashedText</returns>
        public string HashTextWithKnownSalt(string textToHash, string saltWithHashIteration)
        {
            _logger.Information($"ComputeWithKnownSalt: '{textToHash}' with salt {saltWithHashIteration}");

            if (string.IsNullOrEmpty(textToHash))
                throw new InvalidOperationException("text to hash cannot be empty");

            if (string.IsNullOrEmpty(saltWithHashIteration))
                throw new InvalidOperationException("saltWithHashIteration cannot be empty");

            // generate salt
            Tuple<int, string> hashAndSalt = ExpandSalt(saltWithHashIteration);

            _logger.Information($"expanded salt: {hashAndSalt.Item2}");

            //compute the hash
            string hash = CalculateHash(textToHash, hashAndSalt.Item2, hashAndSalt.Item1);
            _logger.Information($"hashed text: {hash}");

            return hash;
        }

        /// <summary>
        /// Generates a salt
        /// </summary>
        /// <param name="hashIterations">the hash iterations to add to the salt</param>
        /// <param name="saltSize">the size of the salt</param>
        /// <returns>
        /// the generated salt
        ///	</returns>
        public string GenerateSalt(int hashIterations, int saltSize)
        {
            if (saltSize < 1)
                throw new InvalidOperationException($"Cannot generate a salt of size {saltSize}, use a value greater than 1, recommended: 16");

            string randomValue = GenerateRandomKey(saltSize);

            //assign the generated salt in the format of {iterations}.{salt}
            return $"{hashIterations}.{randomValue}";
        }

        public string GenerateRandomKey(int saltSize)
        {
            var rand = RNGCryptoServiceProvider.Create();
            byte[] ret = new byte[saltSize];
            rand.GetBytes(ret);

            //return the generated value
            return Convert.ToBase64String(ret);
        }

        private string CalculateHash(string textToHash, string salt, int hashIterations)
        {
            //convert the salt into a byte array
            byte[] saltBytes = Encoding.UTF8.GetBytes(salt);

            var pbkdf2 = new Rfc2898DeriveBytes(textToHash, saltBytes, hashIterations);
            var key = pbkdf2.GetBytes(64);
            return Convert.ToBase64String(key);
        }

        private Tuple<int, string> ExpandSalt(string hashAndSalt)
        {
            try
            {
                //get the position of the . that splits the string
                int i = hashAndSalt.IndexOf(".");

                //Get the hash iteration from the first index
                int hashIterations = int.Parse(hashAndSalt.Substring(0, i), System.Globalization.NumberStyles.Number);
                i++;
                string salt = hashAndSalt[i..]; // this is same as substring(i)

                return new Tuple<int, string>(hashIterations, salt);
            }
            catch (Exception e)
            {
                _logger.Error($"ExpandSalt error: {e.Message}");
                throw new FormatException("The salt was not in an expected format of {integer}.{string}");
            }
        }

        /// <summary>
        /// Compare password hashes for equality. Uses a constant time comparison method.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="hashedPassword"></param>
        /// <returns></returns>
        public bool Compare(string password, string hashedPassword)
        {
            byte[] p = Convert.FromBase64String(password);
            byte[] hp = Convert.FromBase64String(hashedPassword);

            uint diff = System.Convert.ToUInt32(p.Length) ^ System.Convert.ToUInt32(hp.Length);
            int i = 0;

            while (i < p.Length && i < hp.Length)
            {
                diff |= System.Convert.ToUInt32(p[i] ^ hp[i]);
                i += 1;
            }

            return (diff == 0);
        }
    }
}