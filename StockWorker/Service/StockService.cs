using CommonCore.EntityFramework.Common;
using CommonCore.Redis;
using HtmlAgilityPack;
using JQData;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Serilog;
using StockWorker.Db;
using StockWorker.Db.entity;
using StockWorker.Helper;
using StockWorker.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
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
            JQClientHelper jQClient = new JQClientHelper();
            var token = jQClient.GetToken("18826222483", "DDDfff123");
            while (true)
            {
                var res = jQClient.GetPrice("002025", 1, "1m");
            }
            await GetAllStock();
            await GetGroupInfo();
            return true;
        }

        private async Task<int> GetTotalPagesAsync(HttpClient client, string baseUrl)
        {
            // 请求第一页数据以获取总页数
            string url = $"{baseUrl}&pn=1";
            HttpResponseMessage response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                string responseData = await response.Content.ReadAsStringAsync();
                // 移除回调函数的部分，保留 JSON 数据
                responseData = responseData.Substring(responseData.IndexOf('(') + 1, responseData.LastIndexOf(')') - responseData.IndexOf('(') - 1);

                JObject json = JObject.Parse(responseData);
                int total = (int)json["data"]["total"];
                return total;
                int pageSize = (int)json["data"]["size"];
                return (int)Math.Ceiling((double)total / pageSize); // 计算总页数
            }
            return 0;
        }

        public async Task GetAllStock()
        {
            var stockList = _carrepository.GetRepository<StockData>().Query().ToList();
            // 定义基础URL（不包含pn参数）
            /*    string baseUrl = "https://push2.eastmoney.com/api/qt/clist/get?np=1&fltt=1&invt=2&cb=jQuery37102442809214882955_1739119008152&fs=m%3A0%2Bt%3A6%2Cm%3A0%2Bt%3A80%2Cm%3A1%2Bt%3A2%2Cm%3A1%2Bt%3A23%2Cm%3A0%2Bt%3A81%2Bs%3A2048&fields=f12%2Cf13%2Cf14%2Cf1%2Cf2%2Cf4%2Cf3%2Cf152%2Cf5%2Cf6%2Cf7%2Cf15%2Cf18%2Cf16%2Cf17%2Cf10%2Cf8%2Cf9%2Cf23&fid=f3&pz=100&po=1&dect=1&ut=fa5fd1943c7b386f172d6893dbfba10b&wbp2u=%7C0%7C0%7C0%7Cweb&_=1739119008205";
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
                            string apiUrl = $"{baseUrl}?pn={page}";

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
                }*/

            // 定义基础URL（不包含pn参数）
            string baseUrl = "https://push2.eastmoney.com/api/qt/clist/get?np=1&fltt=1&invt=2&cb=jQuery37102442809214882955_1739119008152&fs=m%3A0%2Bt%3A6%2Cm%3A0%2Bt%3A80%2Cm%3A1%2Bt%3A2%2Cm%3A1%2Bt%3A23%2Cm%3A0%2Bt%3A81%2Bs%3A2048&fields=f12%2Cf13%2Cf14%2Cf1%2Cf2%2Cf4%2Cf3%2Cf152%2Cf5%2Cf6%2Cf7%2Cf15%2Cf18%2Cf16%2Cf17%2Cf10%2Cf8%2Cf9%2Cf23&fid=f3&po=1&dect=1&ut=fa5fd1943c7b386f172d6893dbfba10b&wbp2u=%7C0%7C0%7C0%7Cweb&_=1739119008205";

            // 创建HttpClient实例
            using (HttpClient client = new HttpClient())
            {
                // 设置请求头
                client.DefaultRequestHeaders.Add("Accept", "*/*");
                client.DefaultRequestHeaders.Add("Accept-Language", "zh-CN,zh;q=0.9");
                client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                client.DefaultRequestHeaders.Add("Referer", "https://quote.eastmoney.com/center/gridlist.html");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "script");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "no-cors");
                client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site");
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36");
                client.DefaultRequestHeaders.Add("sec-ch-ua", "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
                client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
                client.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");

                // 获取总页数
                int totalStocks = await GetTotalPagesAsync(client, baseUrl);
                if (totalStocks == 0)
                {
                    Console.WriteLine("无法获取总页数。");
                    return;
                }
                var pageSize = 100;
                // 遍历所有页面
                for (int page = 1; page <= (totalStocks / pageSize) + 1; page++)
                {
                    // 构建当前页的URL
                    string url = $"{baseUrl}&pn={page}&pz={pageSize}";

                    // 发送GET请求
                    HttpResponseMessage response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        // 读取响应内容
                        string responseData = await response.Content.ReadAsStringAsync();
                        responseData = responseData.Substring(responseData.IndexOf('(') + 1, responseData.LastIndexOf(')') - responseData.IndexOf('(') - 1);

                        // 解析JSON数据
                        JObject json = JObject.Parse(responseData);

                        // 提取当前页的股票数据
                        JArray stocks = (JArray)json["data"]["diff"];
                        List<StockData> allStocks = new List<StockData>();
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

                        await _carrepository.GetRepository<StockData>().BatchInsertAsync(allStocks);
                    }
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