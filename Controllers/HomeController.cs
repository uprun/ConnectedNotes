﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ConnectedNotes.Models;
using Newtonsoft.Json;

namespace ConnectedNotes.Controllers
{
    [RequestSizeLimit(100_000)]// explicit restriction to 100 kilobytes
    public class HomeController : Controller
    {
        

        public IActionResult ideas(string source)
        {
            lock(pageLocker)
            {
                homePageCounter++;
                Console.WriteLine($"#{homePageCounter} open of \"{nameof(ideas)}\" page");
            }
            
            return View();
        }

        

        private static Dictionary<string, string> synchronization = new Dictionary<string, string>();

        private static Dictionary<(string Receiver, string Sender), List<string> > messageBox = new Dictionary<(string Receiver, string Sender), List<string> > ();

        private static object pageLocker = new object();

        private static int homePageCounter = 0;

        private string GenerateToken()
        {
            char[] toSelect = new[] {'a', 'b', 'c', 'd', 'k', 'h', 'y', 'z', 'x', 's', 'f', 't'};
            var rnd = new Random();
            int firstPart = rnd.Next(100);
            int secondIndex = rnd.Next(toSelect.Length);
            int secondIndex_2 = rnd.Next(toSelect.Length);
            int thirdPart = rnd.Next(100);
            int fourthIndex = rnd.Next(toSelect.Length);
            int fourthIndex_2 = rnd.Next(toSelect.Length);
            return $"{firstPart}{toSelect[secondIndex]}{toSelect[secondIndex_2]}{thirdPart}{toSelect[fourthIndex]}{toSelect[fourthIndex_2]}";
        }

        [HttpPost]
        [Throttle(Name = nameof(GetOneTimeSynchronizationToken), Seconds = 10)]
        public JsonResult GetOneTimeSynchronizationToken(string publicKey)
        {
            string token;
            lock(synchronization)
            {
                token = GenerateToken();
                for(int k = 0; k < 10; k++)
                {
                    if(synchronization.ContainsKey(token))
                    {
                        token = GenerateToken();
                    }
                    else
                    {
                        break;
                    }
                }
                if(synchronization.ContainsKey(token))
                {
                    throw new Exception("Failed to obtain link token");
                }
                synchronization.Add(token, publicKey);
            }

            return new JsonResult(token);
            
        }
        
        [HttpPost]
        public JsonResult SendMessages(Message[] messages, string senderPublicKey)
        {
            if(messages.Length > 10)
            {
                Request.HttpContext.Response.StatusCode = (int) System.Net.HttpStatusCode.Conflict;
                return new JsonResult(false);
            }
            int sum = messages.Sum(x => x.Receiver.Length + x.Text.Length);
            if(sum > 4000)
            {
                Request.HttpContext.Response.StatusCode = (int) System.Net.HttpStatusCode.Conflict;
                return new JsonResult(false);
            }
            

            foreach(var m in messages)
            {
                lock(messageBox)
                {
                    var searchKey = ( Receiver: m.Receiver, Sender: senderPublicKey);
                    if(messageBox.ContainsKey( searchKey ))
                    {
                        var mailBoxOfReceiver = messageBox[searchKey];
                        
                        mailBoxOfReceiver.Add(m.Text);

                    }
                    else
                    {
                        messageBox.Add(searchKey, new List<string>() { m.Text });
                    }
                }
            }

            return new JsonResult(true);
            
        }

        [HttpPost]
        public JsonResult ReceiveMessages(string publicKey)
        { // potential place for abuse, because anyone who nows my public key can receive messages for me, but on the other hand they are encrypted
            var result = new List<string>();
        
            lock(messageBox)
            {
                messageBox.Keys.ToList()
                .Where(x => x.Receiver == publicKey)
                .ToList()
                .ForEach(key => {
                    var mailBoxOfReceiver = messageBox[key];
                    result.AddRange(mailBoxOfReceiver.Take(2));
                    messageBox[key] = mailBoxOfReceiver.Skip(2).ToList();
                });
            }
           

            return new JsonResult(result);
            
        }

        [HttpPost]
        public JsonResult StatisticsOnLoad(string publicKey)
        {

            return new JsonResult(true);
        }
        
        [Throttle(Name = nameof(GetSyncPublicKey), Seconds = 5)]
        public JsonResult GetSyncPublicKey(string token)
        {
            string publicKey;
            lock(synchronization)
            {
                if(synchronization.ContainsKey(token))
                {
                    publicKey = synchronization[token];
                    synchronization.Remove(token);
                }
                else
                {
                    throw new Exception("Syncronization token not found");
                }
            }
            return new JsonResult(publicKey);

        }

        

    
    }
}
