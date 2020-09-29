using BlyadskiyBot;
using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot
{
    class Program
    {
        static Repository repository = new Repository();
        static string message;
        static TelegramBotClient client;
        static List<string> currencies = new List<string>() { "USD", "EUR" };
        public static object JavaScriptSerializer { get; private set; }

        static void Main(string[] args)
        {
            client = new TelegramBotClient("1366636939:AAHAHK8wb_E2fRfzvqbs2JkGvJ2rbsdy63g");
            client.OnMessage += getMsg;
            client.StartReceiving();
            Console.Read();
        }
        private static void getMoney(long TgID)
        {
            string cur = repository.MoneyUsers.Where(ti => ti.TgId == TgID).FirstOrDefault().Currencies;
            JObject curObj = JObject.Parse(cur);
            Console.WriteLine(curObj["USD"]);
            message = "";
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
                        Console.WriteLine(item["ccy"]);
                        if ((bool)curObj[item["ccy"].ToString()])
                            message += item["ccy"].ToString() + " BUY:" + Math.Round(Convert.ToDouble(item["buy"]), 2) + "\t\t\t SALE:" + Math.Round(Convert.ToDouble(item["sale"]), 2) + "\n";
                    }
                    Console.WriteLine(message);
                }
            }
        }
        private static void getMsg(object sender, MessageEventArgs e)
        {
            long tgid = -1;
            //repository.InsertUser(new MoneyUser() { Currencies = "test", TgId = e.Message.Chat.Id, Seed = 100000 });
            if(repository.MoneyUsers.Count>0)
            tgid = repository.MoneyUsers.Where(ti => ti.TgId == e.Message.Chat.Id).FirstOrDefault().TgId;
            string cur = repository.MoneyUsers.Where(ti => ti.TgId == e.Message.Chat.Id).FirstOrDefault().Currencies;
            JObject curObj = JObject.Parse(cur);
            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;
            if (e.Message.Text.Contains("/seed"))
            {
                repository.SaveSeed(Convert.ToInt32(e.Message.Text.Split(' ')[1]), e.Message.Chat.Id);
                client.SendTextMessageAsync(e.Message.Chat.Id, "Seed changed");
            }
            Console.WriteLine(e.Message.Text);
            switch (e.Message.Text.ToLower())
            {
                case "/reg":
                    
                    repository.InsertUser(new MoneyUser() { TgId = e.Message.Chat.Id, Seed = 100000 ,Currencies = "{'USD': true,'EUR': true,'RUR': false,'BTC': true}"});
                    client.SendTextMessageAsync(e.Message.Chat.Id, "Registered user");
                    break;
                case "money":
                    if(tgid != -1)
                    {
                        getMoney(e.Message.Chat.Id);
                        client.SendTextMessageAsync(e.Message.Chat.Id, message);

                    }
                    else
                    {
                        client.SendTextMessageAsync(e.Message.Chat.Id, "Not registered user,type /reg to register");

                    }
                    break;
                case "usd":
                    
                    if ((bool)curObj["USD"])
                    {
                        curObj["USD"] = false;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "USD Removed from your currency list");
                    }
                    else
                    {
                        curObj["USD"] = true;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "USD added from your currency list");
                    }
                    repository.SaveCurrency(curObj.ToString(), e.Message.Chat.Id);
                    break;
                case "eur":
                    if ((bool)curObj["EUR"])
                    {
                        curObj["EUR"] = false;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "EUR Removed from your currency list");
                    }
                    else
                    {
                        curObj["EUR"] = true;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "EUR added from your currency list");
                    }
                    repository.SaveCurrency(curObj.ToString(), e.Message.Chat.Id);
                    break;
                case "rur":
                    if ((bool)curObj["RUR"])
                    {
                        curObj["RUR"] = false;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "RUR Removed from your currency list");
                    }
                    else
                    {
                        curObj["RUR"] = true;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "RUR added from your currency list");
                    }
                    repository.SaveCurrency(curObj.ToString(), e.Message.Chat.Id);
                    break;
                case "btc":
                    if ((bool)curObj["BTC"])
                    {
                        curObj["BTC"] = false;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "BTC Removed from your currency list");
                    }
                    else
                    {
                        curObj["BTC"] = true;
                        client.SendTextMessageAsync(e.Message.Chat.Id, "BTC added from your currency list");
                    }
                    repository.SaveCurrency(curObj.ToString(), e.Message.Chat.Id);
                    break;
                case "options":
                    var markup = new ReplyKeyboardMarkup(new[]
                    {
                        new KeyboardButton("USD"),
                        new KeyboardButton("EUR"),
                        new KeyboardButton("RUR"),
                        new KeyboardButton("BTC")
                    });
                    markup.OneTimeKeyboard = true;
                    client.SendTextMessageAsync(e.Message.Chat.Id, "ON/OFF CURRENCY IN MSG", replyMarkup: markup);
                    break;
                default:
                    break;
            }

        }
    }
}