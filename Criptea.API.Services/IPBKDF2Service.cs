using System;

namespace Criptea.API.Services
{
    public interface IPBKDF2Service
    {
        /// <summary>
        /// Compute the hash and will also generate a salt from parameters
        /// </summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="saltSize">The size of the salt to be generated</param>
        /// <param name="hashIterations"></param>
        /// <returns>the computed hash: HashedText and the generated salt</returns>
        Tuple<string, string> HashText(string textToHash, int saltSize, int hashIterations);

        /// <summary>
        /// Compute the hash using the provided salt
        /// </summary>
        /// <param name="textToHash">The text to be hashed</param>
        /// <param name="salt">The salt to be used</param>
        /// <returns>The computed hash: HashedText</returns>
        string HashTextWithKnownSalt(string textToHash, string saltWithHashIteration);

        /// <summary>
        /// Generates a salt
        /// </summary>
        /// <param name="hashIterations">the hash iterations to add to the salt</param>
        /// <param name="saltSize">the size of the salt</param>
        /// <returns>the generated salt</returns>
        string GenerateSalt(int hashIterations, int saltSize);

        /// <summary>
        /// Compare the passwords for equality
        /// </summary>
        /// <param name="passwordHash1">The first password hash to compare</param>
        /// <param name="passwordHash2">The second password hash to compare</param>
        /// <returns>true: indicating the password hashes are the same, false otherwise.</returns>
        bool Compare(string passwordHash1, string passwordHash2);
    }
}