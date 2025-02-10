using Microsoft.Playwright;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;
using File = System.IO.File;

namespace StockWorker.Service
{
    public static class StockDemo
    {
        public static async Task Test()
        {
            decimal maxProfit = -9999999999999999999999M;

            decimal nowHappy = -9999999999999999999999M;

            string programDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string filePath = $"{programDirectory}/maxPush{DateTime.Now.ToString("yyyyMMdd")}.txt"; // 文件路径

            // 判断文件是否存在
            if (System.IO.File.Exists(filePath))
            {
                // 文件存在，读取内容
                string content = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(content))
                {
                    maxProfit = Convert.ToDecimal(content);
                }
            }

            Dictionary<string, decimal> containlist = new Dictionary<string, decimal>
            {
                //{"trs",5.30M },
                //{"els",6.66M },
                //{ "jxn",4.30M }, yes
                { "ty",5M},
                //{"hh",1.88M }, //yes
                //{"hw",25M },  //yes
                //{"ayjk",3.68M }, //高了
                {"nf",19M },
                { "my",30.88M }, //39
                //{"zsny",3.08M },
                //{"ldkg",2.25M },
                //{"zjrj",34.88M },
                {"zmcd",10.11M },
                {"jsdz",14.88M },

                //{"gdht",12.9M }
            };

            Dictionary<string, decimal> outlist = new Dictionary<string, decimal>
            {
                //{"trs",5.50M },
                {"zb",28.07M },
                {"tr",14.53M },
                {"gdht",14.17M },
                {"ayjk",4.17M },
                //{"trs",5.7M }
            };

            List<string> hasList = new List<string>
            {
                //"sz",
                "ayjk",
                "tdjs",
                "trs",
                "tr",
                "zbtx",
                "hh",
                "gdht",
                "hw",
                "jxn",
                "nf",
                "tjjt",
                "zmcd",
                "htgf"
            };

            List<DemoModel> list = new List<DemoModel>()
            {
               new DemoModel {Name = "sz",Url=" https://quote.eastmoney.com/ZS000001.html",origin=3200,count=0},
               //new DemoModel{Name="cyb", Url="https://quote.eastmoney.com/sz159915.html",origin =2.282,count=0 },
                new DemoModel{Name="jxn",Url = "https://quote.eastmoney.com/sz002548.html",origin=4.302M,count=2400},
               new DemoModel {Name="tdjs",Url="https://quote.eastmoney.com/SH600512.html",origin=2.321M,count=4400},
               new DemoModel{Name="gdht",Url="https://quote.eastmoney.com/sz002101.html",origin=12.906M,count=800},
               new DemoModel{Name="zjrj",Url="https://quote.eastmoney.com/sz301508.html",origin=35.38M,count=300},
               new DemoModel{Name="trs",Url ="https://quote.eastmoney.com/SZ002567.html",origin=5.597M,count=1700},
               new DemoModel {Name="wr",Url ="https://quote.eastmoney.com/sz002654.html",origin=2,count=0},
               new DemoModel{Name="zxcb",Url="https://quote.eastmoney.com/sz300788.html",origin=35.65M,count=0},
               new DemoModel {Name="mly",Url ="https://quote.eastmoney.com/SZ000815.html",origin=2,count=0},
               new DemoModel{Name="tr",Url="https://quote.eastmoney.com/sz002150.html",origin=12.141M,count=0},
               new DemoModel{Name="mrhd",Url="https://quote.eastmoney.com/sz300766.html",origin=1,count=1},
               new DemoModel {Name="ayjk",Url="https://quote.eastmoney.com/SZ002172.html",origin=3.792M,count=0}, //3500
               new DemoModel {Name="hh",Url="https://quote.eastmoney.com/sh600221.html",origin = 1.881M,count=0},
               new DemoModel {Name="jqkj",Url="https://quote.eastmoney.com/sh603083.html",origin=1,count=0},
                new DemoModel{Name="htgf",Url="https://quote.eastmoney.com/sz002840.html",origin=11.03M,count=0},
               // new DemoModel{Name="flgf",Url="https://quote.eastmoney.com/SZ001356.html",origin=2,count=0},
               new DemoModel{Name="qlgy",Url="https://quote.eastmoney.com/SZ002457.html",origin=11.95M,count=0},
                new DemoModel{Name="dksz",Url="https://quote.eastmoney.com/sh600850.html",origin=2,count=0},
               new DemoModel{Name="jsdz",Url="https://quote.eastmoney.com/sh600699.html",origin=15.75M,count=0},
               new DemoModel {Name = "nf",Url=" https://quote.eastmoney.com/SH603280.html",origin=20.55M,count=0},
               new DemoModel{Name="slw",Url="https://quote.eastmoney.com/sh600460.html",origin=1,count=0},
               new DemoModel{Name="tpy",Url="https://quote.eastmoney.com/SH601099.html",origin=1,count=0},
               new DemoModel{Name="tjjt",Url="https://quote.eastmoney.com/SH600129.html",origin=25.70M,count=0 },
               new DemoModel{Name = "hw",Url = "https://quote.eastmoney.com/sh603496.html",origin=25.013M,count=0},
               new DemoModel { Name = "zbtx", Url = "https://quote.eastmoney.com/SH603220.html", origin = 21.89M, count = 0 },
               new DemoModel { Name = "bykj", Url = "https://quote.eastmoney.com/sz002649.html", origin = 21.89M, count = 0 },
               new DemoModel{Name="zwzn",Url="https://quote.eastmoney.com/SZ001339.html",origin=1,count=0},
               new DemoModel { Name = "gxkj", Url = "https://quote.eastmoney.com/sz002281.html", origin = 21.89M, count = 0 },
               new DemoModel { Name = "zkcd", Url = "https://quote.eastmoney.com/SZ300496.html", origin = 21.89M, count = 0 },
               new DemoModel{Name="ntxx",Url="https://quote.eastmoney.com/sz000948.html",origin=16,count=0},

               new DemoModel{Name="scch",Url="https://quote.eastmoney.com/sh600839.html",origin=1,count=0},
               new DemoModel{Name="sagd",Url="https://quote.eastmoney.com/sh600703.html",origin=1,count=0},
               new DemoModel{Name ="zmcd",Url="https://quote.eastmoney.com/sh603767.html",origin=13.168M,count=0},
               new DemoModel{Name="rtdl",Url="https://quote.eastmoney.com/sz301236.html",origin=58,count=0},

             //  new DemoModel{Name="ldkg",Url="https://quote.eastmoney.com/sh600606.html",origin=2.25M,count=0},
               new DemoModel {Name="els",Url="https://quote.eastmoney.com/sz002467.html",origin=4,count=0},
               new DemoModel{Name="jl",Url="https://quote.eastmoney.com/SZ002495.html",origin=2.44M,count=0},

               new DemoModel{Name="ty", Url="https://quote.eastmoney.com/sh601500.html",origin =5.14M,count=0 },
               //new DemoModel{Name="xlc",Url = "https://quote.eastmoney.com/sz002219.html",origin=2.8M,count=0},

               new DemoModel{Name="cxyl",Url="https://quote.eastmoney.com/sz002173.html",origin=1,count=0},

               //new DemoModel{Name ="zsny",Url ="https://quote.eastmoney.com/sh601975.html",origin = 3.08M,count=0},
               new DemoModel{Name="rhzn",Url="https://quote.eastmoney.com/sz002313.html",origin=1,count=0},
               new DemoModel{Name="dfgx",Url="https://quote.eastmoney.com/sz300166.html",origin=1,count=0},
               new DemoModel{Name="rjrj",Url = "https://quote.eastmoney.com/sz002474.html",origin=1,count=0},
               //new DemoModel{Name="my",Url="https://quote.eastmoney.com/sz002714.html",origin=40,count=0},
               new DemoModel{Name="gakj",Url="https://quote.eastmoney.com/sz300551.html",origin=2,count=0},
               //new DemoModel {Name="712",Url="https://quote.eastmoney.com/sh603712.html",origin =1,count=0},
               new DemoModel{Name="jyd",Url="https://quote.eastmoney.com/sz003005.html",origin=1,count=0},
               new DemoModel{Name="het",Url="https://quote.eastmoney.com/sz002402.html",origin=1,count=0},
              // new DemoModel {Name = "xhl",Url="https://quote.eastmoney.com/sz000620.html",origin=2.36M,count=0},
              new DemoModel{Name="xxzb",Url="https://quote.eastmoney.com/sz002933.html",origin=2,count=0},
               new DemoModel{Name="hnd",Url="https://quote.eastmoney.com/sz002583.html",origin=1,count=0},
              new DemoModel {Name="bs",Url="https://quote.eastmoney.com/sh600973.html",origin=4,count=0},
              new DemoModel {Name="bx",Url="https://quote.eastmoney.com/sz002514.html",origin=4,count=0},

               //new DemoModel{Name ="jhw",Url="https://quote.eastmoney.com/sh603078.html",origin=3,count=0},
               //new DemoModel { Name = "yh", Url = "https://quote.eastmoney.com/sh601933.html", origin = 4, count = 0 },
               new DemoModel{Name="jm", Url="https://quote.eastmoney.com/sz002969.html",origin =2.52M,count=0 },
              new DemoModel{Name="jz", Url="https://quote.eastmoney.com/sz002090.html",origin =2.52M,count=0 },

              // new DemoModel {Name ="hf",Url="https://quote.eastmoney.com/sh603536.html",origin=11.34M,count = 0},
               new DemoModel{Name="lz", Url="https://quote.eastmoney.com/sz001227.html",origin =2.52M,count=0 },
              // new DemoModel {Name = "tysk",Url="https://quote.eastmoney.com/sz002354.html",origin=5.08M,count=0},
              new DemoModel{Name="sll",Url = "https://quote.eastmoney.com/sh601360.html",origin=9,count=0},
              // new DemoModel{Name = "zc",Url = "https://quote.eastmoney.com/sh600787.html",origin=7.07M,count=0},
              //new DemoModel {Name="zkjc",Url="https://quote.eastmoney.com/sz002657.html",origin=7,count=0},
              // new DemoModel{Name = "wt",Url = "https://quote.eastmoney.com/sz002886.html",origin=12M,count=0},
              new DemoModel{Name = "sj",Url = "https://quote.eastmoney.com/sz002796.html",origin=10.61M,count=0},
              new DemoModel{Name="lj",Url="https://quote.eastmoney.com/sz002651.html",origin=1,count=0},
              new DemoModel {Name="yfw",Url="https://quote.eastmoney.com/sz000670.html",origin=7,count=0},
              // new DemoModel {Name = "ho",Url="https://quote.eastmoney.com/sz002084.html",origin=2.78M,count=0},

              // new DemoModel{Name="ya", Url="https://quote.eastmoney.com/sz002277.html",origin =2.45M,count=0 },
              // new DemoModel{Name="zj", Url="https://quote.eastmoney.com/sz002535.html",origin =3.89M,count=0 },

               //new DemoModel{Name="zy", Url="https://quote.eastmoney.com/sh601375.html",origin =3.67M,count=0 },
               //new DemoModel{Name="yt", Url="https://quote.eastmoney.com/sh600157.html",origin =1.081M,count=0 },
               new DemoModel{Name="gs",Url="https://quote.eastmoney.com/sh601398.html",origin = 5.95M,count=0 },
               new DemoModel{Name="zs-sk", Url="https://quote.eastmoney.com/sz001979.html",origin =9.81M,count=0 },
               new DemoModel{Name="hy", Url="https://quote.eastmoney.com/sz001236.html",origin =15,count=0 },
               //new DemoModel {Name = "zgsy",Url="https://quote.eastmoney.com/sh601857.html",origin=8.90M,count=0},

               //new DemoModel {Name="jzd",Url="https://quote.eastmoney.com/sz002470.html",origin=1.61M,count=0},
            };
            // 初始化 Playwright
            using var playwright = await Playwright.CreateAsync();
            var exePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
            var args = new List<string>() { "--start-maximized", "--disable-blink-features=AutomationControlled", "--no-sandbox" };
            await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                ExecutablePath = exePath,
                Args = args.ToArray(),
                Timeout = 120000,
                Headless = true,
                ChromiumSandbox = false,
                IgnoreDefaultArgs = new[] { "--enable-automation" },
                SlowMo = 100,
            });

            // 创建一个页面列表
            var pages = new List<IPage>();

            // Create browser context with image blocking
            var context = await browser.NewContextAsync();

            // Set up request interception to block images
            await context.RouteAsync("**/*", route =>
            {
                // Check if the request URL starts with "api/"
                if (route.Request.Url.Contains("api/qt/stock/kline/get") || route.Request.Url.Contains("https://quote.eastmoney.com/")
                  || route.Request.Url.Contains("http://quote.eastmoney.com/"))
                {
                    route.ContinueAsync(); // Allow requests starting with "api/"
                }
                else
                {
                    route.AbortAsync(); // Block all other requests
                }
            });

            // Loop through your list and create new pages within the context
            foreach (var item in list)
            {
                var newpage = await context.NewPageAsync();
                pages.Add(newpage);
            }
            var hasAll = false;
            // 循环设置每个页面的请求监听器
            Stopwatch sw = Stopwatch.StartNew();
            sw.Start();
            for (var i = 0; i < list.Count; i++)
            {
                var page = pages[i];
                var item = list[i];
                page.Response += async (sender, response) =>
                {
                    try
                    {
                        var request = response.Request;
                        // 只监听特定的请求 URL
                        if (request.Url.Contains("api/qt/stock/kline/get"))
                        {
                            var json = await response.TextAsync();
                            var nowtime = DateTime.Now.ToString("yyyy-MM-dd");
                            var startIndex = json.IndexOf(nowtime);
                            if (startIndex >= 0)
                            {
                                var qq = json.Substring(startIndex, json.Length - 1 - startIndex);
                                var price = qq.Split(',')[2];
                                var maxPrice = qq.Split(',')[3].ToString();
                                var min = qq.Split(',')[4].ToString();
                                var lastChar = min.Substring(min.Length - 1);
                                var secondLastChar = min.Substring(min.Length - 2, 1);
                                var lowstr = "";
                                if (lastChar == secondLastChar)
                                {
                                    lowstr = $"lowPrice_{min}";
                                }
                                var maxlastChar = maxPrice.Substring(maxPrice.Length - 1);
                                var maxsecondLastChar = maxPrice.Substring(maxPrice.Length - 2, 1);
                                var maxstr = "";
                                if (maxlastChar == maxsecondLastChar)
                                {
                                    maxstr = $"maxPrice_{maxPrice}";
                                }
                                var change = qq.Split(",")[10];
                                change = change.Split("\"").FirstOrDefault();
                                var sum = (Convert.ToDecimal(price.ToString()) - item.origin) * item.count;

                                if (item.Name == "zbtx")
                                {
                                    sum = (Convert.ToDecimal("24.02".ToString()) - item.origin) * item.count;
                                }
                                if (item.Name == "hh")
                                {
                                    sum = (Convert.ToDecimal("1.56".ToString()) - item.origin) * item.count;
                                }
                                if (containlist.ContainsKey(item.Name))
                                {
                                    containlist.TryGetValue(item.Name, out decimal con);
                                    if (Convert.ToDecimal(min) <= con)
                                    {
                                        Log.Error($"userDo:{item.Name}insert{min}_{price}");
                                    }
                                }
                                if (outlist.ContainsKey(item.Name))
                                {
                                    outlist.TryGetValue(item.Name, out decimal con);
                                    if (Convert.ToDecimal(maxPrice) >= con)
                                    {
                                        Log.Error($"userDo:{item.Name}_out_{maxPrice}_{price}");
                                    }
                                }
                                /* if (containlist.Contains(item.Name) && Convert.ToDecimal(min) <= item.origin)
                                 {
                                     Log.Information($"{item.Name}insert{min}");
                                 }*/

                                item.sum = sum;
                                var happy = Math.Ceiling(list.Sum(n => n.sum));
                                if (hasList.Contains(item.Name))
                                {
                                    maxstr = maxPrice.ToString();
                                }
                                if (containlist.ContainsKey(item.Name))
                                {
                                    lowstr = min.ToString();
                                }
                                var str = $"pushdata:{happy}:{item.Name}_{price}_{Math.Round(sum, 0)}_{qq.Split(',')[8]}_{lowstr}_{maxstr}成功";

                                Log.Information(str);

                                nowHappy = happy;
                                if (sw.ElapsedMilliseconds > 1000 * 60 * 1)
                                {
                                    hasAll = true;
                                }
                                if (nowHappy > maxProfit && hasAll)
                                {
                                    // 文件不存在，创建文件并写入内容
                                    // 使用 File.WriteAllText 写入文件，文件会被覆盖
                                    File.WriteAllText(filePath, nowHappy.ToString());

                                    maxProfit = Convert.ToDecimal(nowHappy);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error listening to response: {ex.Message}");
                    }
                };
            }

            // 导航到 URL
            for (int i = 0; i < list.Count; i++)
            {
                try
                {
                    await pages[i].GotoAsync(list[i].Url);
                }
                catch (Exception ex)
                {
                    Log.Information(ex.ToString());
                }
            }
            while (true)
            {
                var now = DateTime.Now;
                DateTime startTime = new DateTime(now.Year, now.Month, now.Day, 9, 25, 0);
                DateTime endTime = new DateTime(now.Year, now.Month, now.Day, 11, 35, 0);
                // 获取当前星期几 (0-6，0是星期天，6是星期六)
                var workday = now.DayOfWeek >= DayOfWeek.Monday && now.DayOfWeek <= DayOfWeek.Friday;

                DateTime startTime2 = new DateTime(now.Year, now.Month, now.Day, 12, 59, 0);
                DateTime endTime2 = new DateTime(now.Year, now.Month, now.Day, 15, 30, 0);
                // 判断当前时间是否不在两个时间区间内，并且是工作日（星期一到星期五）
                if ((now < startTime || now > endTime) && (now < startTime2 || now > endTime2))
                {
                    Log.Information($"退出了{now.ToString("YYYY-MM-dd HHmmss")}");
                    break;
                }
                else
                {
                    await Task.Delay(3000);
                }
            }

            // 关闭浏览器
            await browser.CloseAsync();
        }
    }

    public class DemoModel
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public decimal origin { get; set; }
        public int count { get; set; }
        public decimal sum { get; set; }
    }
}