using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.Commands;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.NetworkInformation;

namespace RuneterraArticlesBot.Modules
{
    public class Commands : ModuleBase<SocketCommandContext>
    {
        string result = null;
        //string url = "https://masteringruneterra.com/articles-2/";
        string url = "https://masteringruneterra.com/guides/";
        string[] textpull;
        string[] links;

        [Command("run")]
        public async Task run()
        {
            while (true)
            {
                await Task.Delay(3600); // Should be delaying command to check every minute. Currently Failing. 
                //ReplyAsync("Heard");
                WebResponse response = null;
                StreamReader reader = null;
                //string[] testLines = {"TEST 1", "TEST 2" };
                links = File.ReadAllLines(@"PostedLinks.txt");
                //Console.WriteLine("File Currently Lists");
                //foreach(string s in links)
                //{
                //    Console.WriteLine(s);
                // }
                //Console.WriteLine("------------------------");
                //ReplyAsync("There have been " + written.Length + " links posted");
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Method = "GET";
                    response = request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                    textpull = result.Split('<');
                    List<string> newest = new List<string>();
                    for (int i = 0; i < textpull.Length; i++)
                    {
                        if (textpull[i].Contains("href") && textpull[i].Contains("elementor-post__thumbnail__link"))
                        {
                            Match extracted = Regex.Match(textpull[i], @"http(s)?://([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?");
                            if (links.Contains(extracted.ToString()) == false)
                            {
                                newest.Add(extracted.ToString());
                            }
                        }
                    }
                    //ReplyAsync(newest.Count + " New Posts");
                    if (newest.Count > 0)
                    {
                        //File.WriteAllLines(@"PostedLinks.txt", newest.ToArray<string>());
                        foreach (string s in newest)
                        {
                            using (StreamWriter sw = File.AppendText(@"PostedLinks.txt"))
                            {
                                sw.WriteLine(s);
                            }

                            //Console.WriteLine(s);
                            ReplyAsync("@everyone There's a new article up: " + s);
                            //links.Add(s);
                        }
                    }
                    else
                    {
                        ReplyAsync("No New Posts");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERR");
                }
                finally
                {
                    reader.Close();
                    response.Close();
                }
                
            }
            /*for (int i = 0; i < 25; i++)
            {
                ReplyAsync("Running");
                Task.Delay(10000).Wait();
            }*/
        }
    }
    public static void check()
    {

    }
}
