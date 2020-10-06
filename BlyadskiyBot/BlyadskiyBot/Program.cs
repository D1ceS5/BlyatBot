using BlyadskiyBot;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot
{
    class Program
    {
        static DateTime updateTime = DateTime.Now;
        static List<long> IDs = new List<long>();

        static TelegramBotClient client;
        static void Main(string[] args)
        {

            Thread.Sleep(1000);

            client = new TelegramBotClient("1366636939:AAHAHK8wb_E2fRfzvqbs2JkGvJ2rbsdy63g");
            client.OnMessage += getMsg;
            client.StartReceiving();



            Timer tt = new System.Threading.Timer(TickFunc, null, 0, 10000);

            Console.Read();
        }


        private static string getMoney(MoneyUser user)
        {
            StringBuilder sB = new StringBuilder();

            JObject currencies = JObject.Parse(user.Currencies);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(" https://api.privatbank.ua/p24api/pubinfo?exchange&json&coursid=11");
            using (var resp = (HttpWebResponse)request.GetResponse())
            {
                using (var reader = new StreamReader(resp.GetResponseStream()))
                {
                    JavaScriptSerializer js = new JavaScriptSerializer();
                    var objText = reader.ReadToEnd();
                    JArray jresp = JArray.Parse(objText);
                    foreach (var item in jresp)
                    {
                        if ((bool)currencies[item["ccy"]])
                        sB.Append(item["ccy"].ToString() + " BUY:" + Math.Round(Convert.ToDouble(item["buy"]), 2) + "\t\t\t SALE:" + Math.Round(Convert.ToDouble(item["sale"]), 2) + "\n");
                    }
                }
            }
            return sB.ToString();
        }
        private static void getMsg(object sender, MessageEventArgs e)
        {
            Repository rep = new Repository();
            if (e.Message.Type == Telegram.Bot.Types.Enums.MessageType.Text)
            {
                Console.WriteLine("Message from " + e.Message.Chat.FirstName + ":" + e.Message.Text);

                //___________________________________

                if (e.Message.Text.Contains("/seed"))
                {
                    int seed = -1;
                    int.TryParse(e.Message.Text.Split(' ')[1], out seed);
                    if (seed != -1)
                    {
                        rep.SaveSeed(seed, e.Message.Chat.Id);
                    }
                }

                switch (e.Message.Text)
                {
                    case "/start":
                        {
                            IDs.Add(e.Message.Chat.Id);

                            break;
                        }
                    case "/reg":
                        {
                            if (rep.MoneyUsers.Where(s => s.TgId == e.Message.Chat.Id).ToList().Count == 0)
                            {
                                rep.InsertUser(new MoneyUser() { TgId = e.Message.Chat.Id, Currencies = "[ 'USD': true, 'EUR': true:, 'RUR': false, 'BTC': true]", Seed = 1, LastUpdate = DateTime.Now });
                            }
                            break;
                        }
                    case "/options":
                        {
                            ReplyKeyboardMarkup markup = new ReplyKeyboardMarkup(new KeyboardButton[] {
                                new KeyboardButton("USD"),
                                new KeyboardButton("EUR"),
                                new KeyboardButton("RUR"),
                                new KeyboardButton("BTC"),

                            });
                            client.SendTextMessageAsync(e.Message.Chat.Id, "Choose currency", replyMarkup: markup);
                            break;
                        }
                    case "USD":
                        {
                            string objTxt = rep.MoneyUsers.Where(t => t.TgId == e.Message.Chat.Id).FirstOrDefault().Currencies;
                            JObject array = JObject.Parse(objTxt);
                            if ((bool)array["USD"])
                            {
                                array["USD"] = false;
                            }
                            else
                            {
                                array["USD"] = true;
                            }
                            rep.SaveCurrency(array.ToString(), e.Message.Chat.Id);
                            break;
                        }
                    case "EUR":
                        {
                            string objTxt = rep.MoneyUsers.Where(t => t.TgId == e.Message.Chat.Id).FirstOrDefault().Currencies;
                            JObject array = JObject.Parse(objTxt);
                            if ((bool)array["EUR"])
                            {
                                array["EUR"] = false;
                            }
                            else
                            {
                                array["EUR"] = true;
                            }
                            rep.SaveCurrency(array.ToString(), e.Message.Chat.Id);
                            break;
                        }
                    case "RUR":
                        {
                            string objTxt = rep.MoneyUsers.Where(t => t.TgId == e.Message.Chat.Id).FirstOrDefault().Currencies;
                            JObject array = JObject.Parse(objTxt);
                            if ((bool)array["RUR"])
                            {
                                array["RUR"] = false;
                            }
                            else
                            {
                                array["RUR"] = true;
                            }
                            rep.SaveCurrency(array.ToString(), e.Message.Chat.Id);
                            break;
                        }
                    case "BTC":
                        {
                            string objTxt = rep.MoneyUsers.Where(t => t.TgId == e.Message.Chat.Id).FirstOrDefault().Currencies;
                            JObject array = JObject.Parse(objTxt);
                            if ((bool)array["BTC"])
                            {
                                array["BTC"] = false;
                            }
                            else
                            {
                                array["BTC"] = true;
                            }
                            rep.SaveCurrency(array.ToString(), e.Message.Chat.Id);
                            break;
                        }
                }

            }
        }
        public async static void TickFunc(object state)
        {
            Repository rep = new Repository();
            
            List<MoneyUser> users = rep.MoneyUsers.OrderBy(x => x.LastUpdate).ToList();
            foreach (var user in users)
            {
                if (user.LastUpdate.Minute == DateTime.Now.Minute || user.LastUpdate < DateTime.Now)
                {
                    try
                    {
                        Console.WriteLine("Sended to " + user.TgId + ":" + getMoney(user));
                        await client.SendTextMessageAsync(user.TgId, getMoney(user));
                    }
                    catch (Exception ex)
                    {

                    }
                    rep.SaveDate(user.TgId, DateTime.Now.AddMinutes(user.Seed));
                }
            }
        }

    }
}