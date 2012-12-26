using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using Z.Util;

namespace Z.OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthSignatureGenerator
    {
        /// <summary>
        /// 
        /// </summary>
        protected const string OAuthParameterPrefix = "oauth_";

        /// <summary>
        /// List of know and used oauth parameters' names
        /// </summary>
        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="oauth">The information that needs to be signed</param>
        /// <param name="extraParameters">The parameters</param>
        public SignatureResult GenerateSignature(OAuthInfomation oauth, QueryParameterList extraParameters)
        {
            SignatureResult result = new SignatureResult();

            switch (oauth.SignatureMethod)
            {
                case SignatureTypes.PLAINTEXT:
                    result.Signature = HttpUtility.UrlEncode(oauth.GetSignatureKey);
                    break;
                case SignatureTypes.HMACSHA1:
                    result = GenerateSignatureBase(oauth, extraParameters);
                    result.Signature = ComputeHash(new HMACSHA1(Encoding.ASCII.GetBytes(oauth.GetSignatureKey)), result.SignatureBase);
                    break;
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }

            return result;
        }

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        public string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            //return String.Concat(hashBytes.Select(b => Convert.ToString(b, 16)));
            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private QueryParameterList GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new QueryParameterList(BodyDataFormat.EncodedUrl);

            if (!String.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!String.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, String.Empty));
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        protected string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="oauth">The full url that needs to be signed </param>
        /// <param name="extraParameters">The parameters</param>
        private SignatureResult GenerateSignatureBase(OAuthInfomation oauth, QueryParameterList extraParameters)
        {
            if (oauth.Token == null)
            {
                oauth.Token = String.Empty;
            }

            if (oauth.TokenSecret == null)
            {
                oauth.TokenSecret = String.Empty;
            }

            var result = new SignatureResult();

            var parameters = GetQueryParameters(oauth.RequestUri.Query);
            parameters.AddRange(oauth.GetParametersForSignature());
            parameters.AddRange(extraParameters.Where(p => p.NeedSign));
            parameters.Sort(new QueryParameterComparer());

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", oauth.HttpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(oauth.RequestUri.Normalize()));
            signatureBase.AppendFormat("{0}", UrlEncode(parameters.ToString()));
            result.SignatureBase = signatureBase.ToString();

            return result;
        }
    }
}