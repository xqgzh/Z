using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Web;

namespace Z.OAuth
{
    /// <summary>
    /// 
    /// </summary>
    public class OAuthInfomation
    {
        private const string HMACSHA1SignatureType = "HMAC-SHA1";
        private const string PlainTextSignatureType = "PLAINTEXT";
        private const string RSASHA1SignatureType = "RSA-SHA1";
        private const string OAuthConsumerKeyKey = "oauth_consumer_key";
        private const string OAuthCallbackKey = "oauth_callback";
        private const string OAuthVersionKey = "oauth_version";
        private const string OAuthSignatureMethodKey = "oauth_signature_method";
        private const string OAuthSignatureKey = "oauth_signature";
        private const string OAuthTimestampKey = "oauth_timestamp";
        private const string OAuthNonceKey = "oauth_nonce";
        private const string OAuthTokenKey = "oauth_token";
        private const string OAuthTokenSecretKey = "oauth_token_secret";

        /// <summary>
        /// 
        /// </summary>
        public OAuthInfomation()
        {
            Nonce = GenerateNonce();
            TimeStamp = GenerateTimeStamp();
            OAuthVersion = "1.0";
            SignatureMethod = SignatureTypes.HMACSHA1;
            HttpMethod = WebRequestMethods.Http.Post;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="secret"></param>
        /// <param name="url"></param>
        public OAuthInfomation(string key, string secret, string url)
            : this()
        {
            ConsumerKey = key;
            ConsumerSecret = secret;
            RequestUri = new Uri(url);
        }

        /// <summary>
        /// 
        /// </summary>
        public string Nonce { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string TimeStamp { get; protected set; }

        /// <summary>
        /// 
        /// </summary>
        public string OAuthVersion { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SignatureTypes SignatureMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TokenSecret { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string HttpMethod { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public Uri RequestUri { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string GetSignatureKey
        {
            get { return String.Format("{0}&{1}", ConsumerSecret, TokenSecret); }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SignatureMethodKey
        {
            get
            {
                switch (SignatureMethod)
                {
                    case SignatureTypes.PLAINTEXT:
                        return PlainTextSignatureType;
                    case SignatureTypes.HMACSHA1:
                        return HMACSHA1SignatureType;
                    case SignatureTypes.RSASHA1:
                        return RSASHA1SignatureType;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<QueryParameter> GetParametersForSignature()
        {
            var parameters = new List<QueryParameter>();

            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, Nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, TimeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, SignatureMethodKey));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, ConsumerKey));

            if (!String.IsNullOrEmpty(Token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, Token));
            }

            return parameters;
        }

        /// <summary>
        /// 生成HTTP认证头。
        /// </summary>
        /// <param name="signature">签名</param>
        /// <returns></returns>
        public string GenerateAuthorizationHeader(string signature)
        {
            if (String.IsNullOrEmpty(Token))
            {
                return String.Format("OAuth oauth_consumer_key=\"{0}\",oauth_nonce=\"{1}\",oauth_signature=\"{2}\",oauth_signature_method=\"{3}\",oauth_timestamp=\"{4}\",oauth_version=\"{5}\"",
                    ConsumerKey, Nonce, HttpUtility.UrlEncode(signature), SignatureMethodKey, TimeStamp, OAuthVersion);
            }
            else
            {
                return String.Format("OAuth oauth_consumer_key=\"{0}\",oauth_nonce=\"{1}\",oauth_signature=\"{2}\",oauth_signature_method=\"{3}\",oauth_timestamp=\"{4}\",oauth_token=\"{5}\",oauth_version=\"{6}\"",
                    ConsumerKey, Nonce, HttpUtility.UrlEncode(signature), SignatureMethodKey, TimeStamp, Token, OAuthVersion);
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        protected virtual string GenerateNonce()
        {
            return Z.Enc.MD5.ToString(Encoding.ASCII.GetBytes(new Random().Next().ToString()));
        }
    }
}
