using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace HashExample
{
    class Program
    {
        static void Main(string[] args)
        {
             /** INSTRUCTIONS:
             * Implement a program in C# to sign a document. The program 
             * will be able to generate a public key pair, accept a document 
             * and compute the hash.  It then encrypts the has using the 
             * private key.  The program will be able to verify a signature 
             * by accepting an encrypted signature (hash) and a document.  
             * It computes the hash, decrypts the encrypted hash and compares 
             * the hashes. It reports if the document is valid or not.  Note, 
             * C# provides the necessary cryptographic primitives.  Use ONLY 
             * the hash and public key encryption functions, i.e. write your 
             * own digital signature functions. 
             **/

            try
            {
                // Create keys for Alice and Bob, then save them in a containers.
                GenKey_SaveInContainer("Alice");
                GenKey_SaveInContainer("Bob");

                // Retrieve the keys from the containers.
                GetKeyFromContainer("Alice");
                GetKeyFromContainer("Bob");

                // Delete the keys from the containers.  
                DeleteKeyFromContainer("Alice");
                DeleteKeyFromContainer("Bob");
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }


            static void GenKey_SaveInContainer(string ContainerName)
            {
                // Create container to hold key. 
                CspParameters cp = new CspParameters();
                cp.KeyContainerName = ContainerName;

                // Create instance of RSACryptoServiceProvider to access 
                // the key container.
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

                Console.WriteLine("Key added to container: \n  {0}", rsa.ToXmlString(true));
            }

            static void GetKeyFromContainer(string ContainerName)
            {
                // Similar to GenKey, BUT if container doesn't exist,
                // the key is generated and stored into the new container.
                // If the container exists, the key is loaded from the
                // container.
                CspParameters cp = new CspParameters();
                cp.KeyContainerName = ContainerName; 
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);
                Console.WriteLine("Key retrieved from container : \n {0}", rsa.ToXmlString(true));

                /* The key's Modulus is the public key */
            }

            static void DeleteKeyFromContainer(string ContainerName)
            {
                // Create the CspParameters object and set the key container   
                // name used to store the RSA key pair.  
                CspParameters cp = new CspParameters();
                cp.KeyContainerName = ContainerName;

                // Create a new instance of RSACryptoServiceProvider that accesses  
                // the key container.  
                RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(cp);

                // Delete the key entry in the container.  
                rsa.PersistKeyInCsp = false;

                // Call Clear to release resources and delete the key from the container.  
                rsa.Clear();

                Console.WriteLine("Key deleted.");
            }

        }
    }
}


            