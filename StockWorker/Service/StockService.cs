using CommonCore.EntityFramework.Common;
using CommonCore.Redis;
using HtmlAgilityPack;
using JQData;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using StockWorker.Db;
using StockWorker.Db.entity;
using StockWorker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StockWorker.Service
{
    public class StockService : IStockService
    {
        private readonly IBaseRepository<CarDbContext> _carrepository;
        private static readonly HttpClient _client = new HttpClient();

        private DateTime searchTime = DateTime.Now.Date.AddDays(14).AddHours(12);

        public StockService(IBaseRepository<CarDbContext> carrepository)

        {
            _carrepository = carrepository;
        }

        public async Task<bool> RunHotStock()
        {
            JQClient jQClient = new JQClient();
            var token = jQClient.GetToken("18826222483", "DDDfff123");
            //await GetAllStock();
            await GetGroupInfo();
            return true;
        }

        public async Task GetAllStock()
        {
            var stockList = _carrepository.GetRepository<StockData>().Query().ToList();
            string baseUrl = "http://push2.eastmoney.com/api/qt/clist/get";
            int pageSize = 100;
            int totalStocks = 6000;
            List<StockData> allStocks = new List<StockData>();
            //await GetInfo("");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // 分页获取所有股票数据
                    for (int page = 1; page <= (totalStocks / pageSize) + 1; page++)
                    {
                        Log.Information($"执行{page}");
                        string apiUrl = $"{baseUrl}?pn={page}&pz={pageSize}&po=1&np=1&fltt=2&invt=2&fid=f3&fs=m:0+t:6,m:0+t:13,m:0+t:80&fields=f1,f2,f3,f4,f5,f6,f7,f8,f9,f10,f12,f13,f14,f15,f16,f17,f18,f20,f21,f23,f24,f25,f22,f11,f62,f128,f136,f115,f152";

                        HttpResponseMessage response = await client.GetAsync(apiUrl);
                        response.EnsureSuccessStatusCode(); // 确保请求成功
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // 解析JSON数据
                        JObject json = JObject.Parse(responseBody);
                        totalStocks = (int)json["data"]["total"];
                        var qq = (totalStocks / pageSize) + 1;
                        JArray stocks = (JArray)json["data"]["diff"]; // 返回的数据在data->diff中

                        foreach (var stock in stocks)
                        {
                            try
                            {
                                string stockCode = (string)stock["f12"];
                                var dbItem = stockList.FirstOrDefault(n => n.Code == stockCode);
                                if (dbItem != null)
                                {
                                    continue;
                                }

                                string stockName = (string)stock["f14"];
                                string stockPrice = (string)stock["f2"];
                                decimal.TryParse(stockPrice, out decimal price);
                                var type = 0;
                                if (stockCode.StartsWith("300"))
                                {
                                    type = 2;
                                }
                                else if (stockCode.StartsWith("688"))
                                {
                                    type = -1;
                                }
                                else if (stockCode.StartsWith("8"))
                                {
                                    type = -2;
                                }
                                else
                                {
                                    type = 1;
                                }

                                allStocks.Add(new StockData
                                {
                                    Code = stockCode,
                                    Name = stockName,
                                    Price = price,
                                    Type = type
                                });
                            }
                            catch (Exception ex)
                            {
                                var q = 23;
                            }
                        }
                    }
                    // 输出所有股票数据
                    Console.WriteLine($"\n共获取 {allStocks.Count} 数据");
                    await _carrepository.GetRepository<StockData>().BatchInsertAsync(allStocks);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"请求出错: {e.Message}");
                }
            }
        }

        public async Task GetGroupInfo()
        {
            var stockList = _carrepository.GetRepository<StockData>().Query().Where(n => n.groupCount <= 0 || !n.groupCount.HasValue).ToList();
            var groupList = _carrepository.GetRepository<StockGroup>().Query().ToList();

            foreach (var stock in stockList)
            {
                List<StockGroup> inserGroup = new List<StockGroup>();
                string url = $"https://basic.10jqka.com.cn/{stock.Code}/concept.html";

                try
                {
                    // 设置请求头
                    _client.DefaultRequestHeaders.Clear();
                    _client.DefaultRequestHeaders.Add("Upgrade-Insecure-Requests", "1");
                    _client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36");
                    _client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Not(A:Brand\";v=\"99\", \"Google Chrome\";v=\"133\", \"Chromium\";v=\"133\"");
                    _client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                    _client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");

                    // 发送请求并获取响应
                    var response = await _client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    var contentType = response.Content.Headers.ContentType.ToString();
                    Encoding encoding = Encoding.GetEncoding("GBK");

                    // 使用 StreamReader 和相应的编码读取网页内容
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new StreamReader(stream, encoding))
                    {
                        string htmlContent = await reader.ReadToEndAsync();
                        var htmlDoc = new HtmlDocument();
                        htmlDoc.LoadHtml(htmlContent);
                        var tdElement = htmlDoc.DocumentNode.SelectNodes("//td[@class='gnName']");
                        if (tdElement == null)
                        {
                            continue;
                        }
                        foreach (var td in tdElement)
                        {
                            StockGroup group = new StockGroup()
                            {
                                Code = stock.Code,
                                Name = stock.Name,
                                GroupName = td.InnerText.Trim(),
                                GroupId = td.GetAttributeValue("clid", string.Empty),
                                Type = stock.Type
                            };
                            var dbGroup = groupList.FirstOrDefault(n => n.Code == stock.Code && n.GroupId == group.GroupId);
                            if (dbGroup == null)
                            {
                                inserGroup.Add(group);
                            }
                        }
                        stock.groupCount = tdElement.Count;
                        Log.Information($"刷新数据【{stock.Code}】");
                        await _carrepository.GetRepository<StockData>().UpdateAsync(stock);
                    }
                    await _carrepository.GetRepository<StockGroup>().BatchInsertAsync(inserGroup);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请求出错: {ex.Message}");
                }
            }
        }

        public static async Task<string> GetStockLatestPriceAsync(string stockCode)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string url = $"http://hq.sinajs.cn/list={GetStockApiCode(stockCode)}";
                    var response = await client.GetStringAsync(url);
                    string[] data = response.Split(',');

                    if (data.Length >= 3)
                    {
                        return data[3];  // 获取第四个字段作为最新价格
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error getting stock price for {stockCode}: {ex.Message}");
                }
            }
            return "N/A";  // 如果出错，返回 N/A
        }

        public static string GetStockApiCode(string stockCode)
        {
            // 根据股票代码返回对应的 API 请求代码，沪市和深市不同
            if (stockCode.StartsWith("6"))
            {
                return $"sh{stockCode}";  // 上证股票代码以6开头
            }
            else
            {
                return $"sz{stockCode}";  // 深证股票代码以其他数字开头
            }
        }
    }
}