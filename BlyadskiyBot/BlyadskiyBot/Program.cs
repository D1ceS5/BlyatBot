using Nancy.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace TeleBot
{
    class Program
    {
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
        private static void getMoney()
        {
            //Math.Round
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
                        if (currencies.Contains(item["ccy"].ToString()))
                            message += item["ccy"].ToString() + " BUY:" + Math.Round(Convert.ToDouble(item["buy"]), 2) + "\t\t\t SALE:" + Math.Round(Convert.ToDouble(item["sale"]), 2) + "\n";
                    }
                    Console.WriteLine(message);
                }
            }
        }
        private static void getMsg(object sender, MessageEventArgs e)
        {

            if (e.Message.Type != Telegram.Bot.Types.Enums.MessageType.Text)
                return;

            Console.WriteLine($"Msg from {e.Message.Chat.Id}");
            switch (e.Message.Text.ToLower())
            {

                case "money":

                    getMoney();
                    client.SendTextMessageAsync(e.Message.Chat.Id, message);

                    break;
                case "usd":
                    if (currencies.Contains("USD"))
                    {
                        currencies.Remove("USD");
                    }
                    else
                    {
                        currencies.Add("USD");
                    }
                    break;
                case "eur":
                    if (currencies.Contains("EUR"))
                    {
                        currencies.Remove("EUR");
                    }
                    else
                    {
                        currencies.Add("EUR");
                    }
                    break;
                case "rur":
                    if (currencies.Contains("RUR"))
                    {
                        currencies.Remove("RUR");
                    }
                    else
                    {
                        currencies.Add("RUR");
                    }
                    break;
                case "btc":
                    if (currencies.Contains("BTC"))
                    {
                        currencies.Remove("BTC");
                    }
                    else
                    {
                        currencies.Add("BTC");
                    }
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