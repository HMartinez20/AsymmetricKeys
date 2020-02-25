using System;
using System.Collections.Generic;
using System.IO;
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

            /* REFERENCE: https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider.signhash?view=netframework-4.8 */

            /** STEPS
             * Sender:
             * 1. Generate keys
             * 2. Get document
             * 3. Encrypt document
             * 4. Get hash
             * 5. Encrypt hash for signature
             *    
             * Receiver:
             * 1. Verify signature (encrypted hash) & document
             *    a. Generate hash
             *    b. Verify document and signature
             *    c. Decrypt document
             **/

            /* STEP ONE -  Gen Keys */
            // May also use CspParameters.KeyContainerName to store keys in safe containers
            RSACryptoServiceProvider AliceKey = new RSACryptoServiceProvider();
            RSAParameters AlicePublic = AliceKey.ExportParameters(false);
            RSAParameters AlicePrivate = AliceKey.ExportParameters(true);

            RSACryptoServiceProvider BobKey = new RSACryptoServiceProvider();
            RSAParameters BobPublic = BobKey.ExportParameters(false);
            RSAParameters BobPrivate = BobKey.ExportParameters(true);


            /* STEP TWO - Get Doc*/
            string path = @"C:\Users\Jammer\source\repos\HashExample\privateDoc.txt";
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            FileStream readFS = File.Open(path, FileMode.Open);
            readFS.Read(fileBytes, 0, System.Convert.ToInt32(readFS.Length));
            readFS.Close();
            FileStream fs = File.Open(path, FileMode.Open);

            /* STEP THREE - Encrypt Doc */
            BobKey.ImportParameters(BobPublic);
            byte[] encDoc = BobKey.Encrypt(fileBytes, false);
            Console.WriteLine("Encrypted Doc using Bob's public key");
            for (int i = 0; i < encDoc.Length; i++) { Console.Write($"{encDoc[i]:X2}"); }
            Console.WriteLine("\n");
            
            
            /* STEP FOUR - Compute Hash With Doc */
            fs.Position = 0;
            SHA256 sha256Hash = SHA256.Create();
            byte[] hash = sha256Hash.ComputeHash(encDoc);
            Console.Write("Original Doc Hash: ");
            for (int i = 0; i < hash.Length; i++) { Console.Write($"{hash[i]:X2}"); }
            Console.WriteLine("\n");


            /* STEP FIVE - Encrypt Hash */
            AliceKey.ImportParameters(AlicePrivate);
            byte[] signature = AliceKey.SignHash(hash, CryptoConfig.MapNameToOID("SHA256"));
            Console.WriteLine("Encrypted Hash using Alice's private key");
            for (int i = 0; i < signature.Length; i++) { Console.Write($"{signature[i]:X2}"); }
            Console.WriteLine("\n");


            /* VERIFY SIGNATURE */
            /* STEP ONE, a - Generate New Hash */
            fs.Position = 0;
            SHA256 newSha256Hash = SHA256.Create();
            byte[] newHash = newSha256Hash.ComputeHash(encDoc);
            Console.Write("Re-Hash of Original: ");
            for (int i = 0; i < newHash.Length; i++) { Console.Write($"{newHash[i]:X2}"); }
            Console.WriteLine("\n");


            /* STEP ONE, b - Verification */
            AliceKey.ImportParameters(AlicePublic);
            bool dataVerified = AliceKey.VerifyData(encDoc, CryptoConfig.MapNameToOID("SHA256"), signature);
            Console.WriteLine("Verified document? {0}\n", dataVerified? "True" : "False");

            bool signatureVerified = AliceKey.VerifyHash(newHash, CryptoConfig.MapNameToOID("SHA256"), signature);
            Console.WriteLine("Verified signature? {0}\n", signatureVerified ? "True" : "False");

            /* STEP ONE, c - Decrypt Doc*/
            BobKey.ImportParameters(BobPrivate);
            byte[] decDoc = BobKey.Decrypt(encDoc, false);
            Console.WriteLine("Decrypted document: \n{0}\n", new ASCIIEncoding().GetString(decDoc));

            fs.Close();

            /* REMOVE KEYS */
            AliceKey.Clear();
            BobKey.Clear();
        }
    }
}


            