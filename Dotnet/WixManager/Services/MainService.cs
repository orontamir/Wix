using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using WixManager.Services;
using System;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Text;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace WixManager.Service
{
    public class MainService : IHostedService
    {
        private readonly IConfiguration _Configuration;
        private static  bool _processIsRunning = false;
        private const string _bearer = "";
       // private const  int MaxThreads = 4;
        public MainService( IConfiguration configuration)
        {
            _Configuration = configuration;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _processIsRunning = true;
            string filePath = _Configuration.GetSection("PathFile") != null ? (_Configuration.GetSection("PathFile").Value) : "C:\\Wix\\queries";
            int maxThreads = _Configuration.GetSection("MaxThreads") != null ? int.Parse((_Configuration.GetSection("MaxThreads").Value)) : 4;

            using HttpClient gitHubClient = new HttpClient();
            gitHubClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_bearer}");
            gitHubClient.DefaultRequestHeaders.Add("User-Agent", "wix");
            int counter = 0;

            using var cts = new CancellationTokenSource();
            try
            {
                while (_processIsRunning)
                {

                    try 
                    {
                        //Read All files from the Directory 
                        foreach (string filename in Directory.EnumerateFiles(filePath, "*.txt"))
                        {
                            //Read file text line by line
                            var lines = File.ReadAllLines(filename);
                            await Parallel.ForEachAsync(
                                TaskRun(lines),
                                new ParallelOptions { MaxDegreeOfParallelism = maxThreads },
                                async  (line, _)  =>
                                {
                                    try
                                    {
                                        HttpResponseMessage response = await gitHubClient.GetAsync(line);
                                        //Return result
                                        string result = await response.Content.ReadAsStringAsync();

                                        LogService.LogInformation(result);
                                        //Parser message Total Count by Regex
                                        Regex regex = new Regex("\"total_count\":(\\d+)");
                                        Match match = regex.Match(result);

                                        if (match.Success)
                                        {
                                            int totalCount = int.Parse(match.Groups[1].Value);
                                            //counter 
                                            counter += totalCount;
                                        }
                                        LogService.LogInformation( $"Total Counter: {counter}");
                                    }
                                    catch (Exception ex) { LogService.LogError($"Error message: {ex.Message}"); }
                                }
                           );

                            if (File.Exists(filename))
                            {
                                //Delete file after finish read it
                                File.Delete(filename);

                            }

                        }
                        
                        Thread.Sleep(1000);
                    }
                    catch(Exception ex) { LogService.LogError($"Error message: {ex.Message}"); }
                }
            }
            catch (Exception ex) { LogService.LogError($"Error message: {ex.Message}"); }

        }

        private IEnumerable<string> TaskRun(string[] lines)
        {
            foreach (string line in lines) {
                yield return line;
            }

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _processIsRunning = false;
            return Task.CompletedTask;
        }
    }
}
