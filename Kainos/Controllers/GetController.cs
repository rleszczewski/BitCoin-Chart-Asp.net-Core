using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Kainos.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Kainos.Controllers
{
    public class GetController : Controller
    {
        public const string BITCOIN_TICKER = "BTCUSD";
        public const string LTC_TICKER = "LTCUSD";
        public const string ETH_TICKER = "ETHUSD";
        public const string GLOBAL_MARKET = "global";

        private readonly string _publicKey;
        private readonly string _secretKey;
        private readonly HMACSHA256 _sigHasher;
        private readonly DateTime _epochUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        private Secrets _secrets { get; }

        public GetController(IOptions<Secrets> secrets)
        {
            this._secrets = secrets.Value;
            _secretKey = _secrets.SecretKey;
            _publicKey = _secrets.PublicKey;
            _sigHasher = new HMACSHA256(Encoding.ASCII.GetBytes(_secretKey));
        }

        //public GetController()
        //{
        //    _secretKey = _secrets.SecretKey;
        //    _publicKey = _secrets.PublicKey;
        //    _sigHasher = new HMACSHA256(Encoding.ASCII.GetBytes(_secretKey));
        //}

        public string GetHeaderSignature()
        {
            var timestamp = (int)((DateTime.UtcNow - _epochUtc).TotalSeconds);
            var payload = timestamp + "." + _publicKey;
            var digestValueBytes = _sigHasher.ComputeHash(Encoding.ASCII.GetBytes(payload));
            var digestValueHex = BitConverter.ToString(digestValueBytes).Replace("-", "").ToLower();
            return payload + "." + digestValueHex;
        }

        public async Task<JToken> GetJsonAsync(string url)
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("X-signature", GetHeaderSignature());
                var user = JToken.Parse(await httpClient.GetStringAsync(url));
                //var stringuser = user.ToString();
                //var model = user.ToObject<BitcoinAllTimeHistoricalData[]>();
                //BitcoinAllTimeHistoricalData bitCoinOjb = JsonConvert.DeserializeObject<BitcoinAllTimeHistoricalData>(stringuser);
                return user;
            }

        }

        public Task<JToken> GetBitcoinAllTimeHistoricalDataAsync() => GetAllTimeHistoricalDataAsync(BITCOIN_TICKER);
        public Task<JToken> GetLTCcoinAllTimeHistoricalDataAsync() => GetAllTimeHistoricalDataAsync(LTC_TICKER);
        public Task<JToken> GetETHcoinAllTimeHistoricalDataAsync() => GetAllTimeHistoricalDataAsync(ETH_TICKER);

        public Task<JToken> GetAllTimeHistoricalDataAsync(string symbol, string market = GLOBAL_MARKET)
        {
            var url = "https://apiv2.bitcoinaverage.com/indices/" + market + "/history/" + symbol + "?"
                   + "&period=alltime"
                   + "&format=json";

            return GetJsonAsync(url);
        }

        //public static BitcoinAllTimeHistoricalData ReadToObject(JToken json)
        //{
        //    BitcoinAllTimeHistoricalData deserializedUser = new BitcoinAllTimeHistoricalData();
        //    MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        //    DataContractJsonSerializer ser = new DataContractJsonSerializer(deserializedUser.GetType());
        //    deserializedUser = ser.ReadObject(ms) as BitcoinAllTimeHistoricalData;
        //    ms.Close();
        //    return deserializedUser;
        //}
        public class ViewModel
        {
            public BitcoinAllTimeHistoricalData[] Btc { get; set; }
            public ETCAllTimeHistoricalData[] Etc { get; set; }
            public LTCAllTimeHistoricalData[] Ltc { get; set; }
        }
        public async Task<IActionResult> Index()
        {
            var model = new ViewModel();

            JToken BitCoin= await GetBitcoinAllTimeHistoricalDataAsync();

            JToken LTCCoin = await GetLTCcoinAllTimeHistoricalDataAsync();

            JToken ETHCoin = await GetETHcoinAllTimeHistoricalDataAsync();

            

            model.Btc = BitCoin.ToObject<BitcoinAllTimeHistoricalData[]>();
            model.Etc = ETHCoin.ToObject<ETCAllTimeHistoricalData[]>();
            model.Ltc = LTCCoin.ToObject<LTCAllTimeHistoricalData[]>();

            return View(model);
        }
    }
}