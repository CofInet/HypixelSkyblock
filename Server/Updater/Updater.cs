using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Coflnet;
using dev;
using Hypixel.NET;
using Hypixel.NET.SkyblockApi;
using Microsoft.EntityFrameworkCore;

namespace hypixel
{
    public class Updater
    {
        private const string LAST_UPDATE_KEY = "lastUpdate";
        private string apiKey;
        private bool abort;
        private static bool minimumOutput;

        public event Action OnNewUpdateStart;
        /// <summary>
        /// Gets invoked when an update is done
        /// </summary>
        public event Action OnNewUpdateEnd;

        public static DateTime LastPull { get; internal set; }
        public static int UpdateSize { get; internal set; }

        private static ConcurrentDictionary<string, BinInfo> LastUpdateBins = new ConcurrentDictionary<string, BinInfo>();

        private static ConcurrentDictionary<string, bool> AlreadyChecked = new ConcurrentDictionary<string, bool>();

        ConcurrentDictionary<string, int> AuctionCount;
        public static ConcurrentDictionary<string, int> LastAuctionCount;

        /// <summary>
        /// Limited task factory
        /// </summary>
        TaskFactory taskFactory;
        private HypixelApi hypixel;

        public Updater(string apiKey)
        {
            this.apiKey = apiKey;

            var scheduler = new LimitedConcurrencyLevelTaskScheduler(3);
            taskFactory = new TaskFactory(scheduler);
        }

        /// <summary>
        /// Downloads all auctions and save the ones that changed since the last update
        /// </summary>
        public async Task<DateTime> Update()
        {
            if (!minimumOutput)
                Console.WriteLine($"Usage bevore update {System.GC.GetTotalMemory(false)}");
            var updateStartTime = DateTime.UtcNow.ToLocalTime();

            try
            {
                if (hypixel == null)
                    hypixel = new HypixelApi(apiKey, 50);

                if (lastUpdateDone == default(DateTime))
                    lastUpdateDone = await CacheService.Instance.GetFromRedis<DateTime>(LAST_UPDATE_KEY);

                if (lastUpdateDone == default(DateTime))
                    lastUpdateDone = new DateTime(2017, 1, 1);
                lastUpdateDone = await RunUpdate(lastUpdateDone);
                FileController.SaveAs(LAST_UPDATE_KEY, lastUpdateDone);
                await CacheService.Instance.SaveInRedis(LAST_UPDATE_KEY, lastUpdateDone);
                FileController.Delete("lastUpdateStart");
            }
            catch (Exception e)
            {
                Logger.Instance.Error($"Updating stopped because of {e.Message} {e.StackTrace}  {e.InnerException?.Message} {e.InnerException?.StackTrace}");
                FileController.Delete("lastUpdateStart");
                throw e;
            }

            ItemDetails.Instance.Save();

            await StorageManager.Save();
            return lastUpdateDone;
        }

        DateTime lastUpdateDone = default(DateTime);

        async Task<DateTime> RunUpdate(DateTime updateStartTime)
        {
            /* Task.Run(()
                  => BinUpdater.GrabAuctions(hypixel)
             );*/
            BinUpdater.GrabAuctions(hypixel);
            long max = 1;
            var lastUpdate = lastUpdateDone; // new DateTime (1970, 1, 1);
            //if (FileController.Exists ("lastUpdate"))
            //    lastUpdate = FileController.LoadAs<DateTime> ("lastUpdate").ToLocalTime ();

            var lastUpdateStart = new DateTime(0);
            if (FileController.Exists("lastUpdateStart"))
                lastUpdateStart = FileController.LoadAs<DateTime>("lastUpdateStart").ToLocalTime();

            if (!minimumOutput)
                Console.WriteLine($"{lastUpdateStart > lastUpdate} {DateTime.Now - lastUpdateStart}");
            FileController.SaveAs("lastUpdateStart", DateTime.Now);

            TimeSpan timeEst = new TimeSpan(0, 1, 1);
            Console.WriteLine($"Updating Data {DateTime.Now}");

            // add extra miniute to start to catch lost auctions
            lastUpdate = updateStartTime - new TimeSpan(0, 1, 0);
            DateTime lastHypixelCache = lastUpdate;

            var tasks = new List<Task>();
            int sum = 0;
            int doneCont = 0;
            object sumloc = new object();
            var firstPage = await hypixel?.GetAuctionPageAsync(0);
            max = firstPage.TotalPages;
            if (firstPage.LastUpdated == updateStartTime)
            {
                // wait for the server cache to refresh
                await Task.Delay(5000);
                return updateStartTime;
            }
            OnNewUpdateStart?.Invoke();

            var cancelToken = new CancellationToken();
            AuctionCount = new ConcurrentDictionary<string, int>();


            for (int i = 0; i < max; i++)
            {
                var index = i;
                await Task.Delay(200);
                tasks.Add(taskFactory.StartNew(async () =>
                {
                    try
                    {
                        var res = index != 0 ? await hypixel?.GetAuctionPageAsync(index) : firstPage;
                        if (res == null)
                            return;

                        max = res.TotalPages;

                        if (index == 0)
                        {
                            lastHypixelCache = res.LastUpdated;
                            // correct update time
                            Console.WriteLine($"Updating difference {lastUpdate} {res.LastUpdated}\n");
                        }

                        // spread out the saving load burst
                        //await Task.Delay(index * 150);
                        var val = await Save(res, lastUpdate);
                        lock (sumloc)
                        {
                            sum += val;
                            // process done
                            doneCont++;
                        }
                        PrintUpdateEstimate(index, doneCont, sum, updateStartTime, max);
                    }
                    catch (Exception e)
                    {
                        try // again
                        {
                            var res = await hypixel?.GetAuctionPageAsync(index);
                            var val = await Save(res, lastUpdate);
                        }
                        catch (System.Exception ex)
                        {
                            Logger.Instance.Error($"Single page ({index}) could not be loaded twice because of {e.Message} {e.StackTrace} {e.InnerException?.Message}");
                        }
                    }

                }, cancelToken).Unwrap());
                PrintUpdateEstimate(i, doneCont, sum, updateStartTime, max);

                // try to stay under 600MB
                if (System.GC.GetTotalMemory(false) > 500000000)
                {
                    Console.Write("\t mem: " + System.GC.GetTotalMemory(false));
                    System.GC.Collect();
                }
                //await Task.Delay(100);
            }

            foreach (var item in tasks)
            {
                //Console.Write($"\r {index++}/{updateEstimation} \t({index}) {timeEst:mm\\:ss}");
                if (item != null)
                    await item;
                PrintUpdateEstimate(max, doneCont, sum, updateStartTime, max);
            }

            if (AuctionCount.Count > 2)
                LastAuctionCount = AuctionCount;

            //BinUpdateSold(currentUpdateBins);

            if (sum > 10)
                LastPull = DateTime.Now;

            Console.WriteLine($"Updated {sum} auctions {doneCont} pages");
            UpdateSize = sum;

            OnNewUpdateEnd?.Invoke();

            return lastHypixelCache;
        }

        internal void UpdateForEver()
        {
            CancellationTokenSource source = new CancellationTokenSource();
            // Fail save
            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromMinutes(5));
                    if (lastUpdateDone > DateTime.Now.Subtract(TimeSpan.FromMinutes(6)))
                        continue;
                    dev.Logger.Instance.Error("Restarting updater");
                    source.Cancel();
                    source = new CancellationTokenSource();
                    StartNewUpdater(source.Token);
                }
            });
            StartNewUpdater(source.Token);
        }

        private void StartNewUpdater(CancellationToken token)
        {
            Task.Run(async () =>
            {
                minimumOutput = true;
                while (true)
                {
                    try
                    {
                        var start = DateTime.Now;
                        var lastCache = await Update();
                        if (abort || token.IsCancellationRequested)
                        {
                            Console.WriteLine("Stopped updater");
                            break;
                        }
                        Console.WriteLine($"--> started updating {start} cache says {lastCache} now its {DateTime.Now}");
                        await WaitForServerCacheRefresh(lastCache);
                    }
                    catch (Exception e)
                    {
                        Logger.Instance.Error("Updater encountered an outside error: " + e.Message);
                        await Task.Delay(15000);
                    }

                }
            }, token);
        }

        private static async Task WaitForServerCacheRefresh(DateTime hypixelCacheTime)
        {
            // cache refreshes every 60 seconds, 2 seconds extra to fix timing issues
            var timeToSleep = hypixelCacheTime.Add(TimeSpan.FromSeconds(62)) - DateTime.Now;
            if (timeToSleep.Seconds > 0)
                await Task.Delay(timeToSleep);
        }

        static void PrintUpdateEstimate(long i, long doneCont, long sum, DateTime updateStartTime, long max)
        {
            var index = sum;
            // max is doubled since it is counted twice (download and done)
            var updateEstimation = index * max * 2 / (i + 1 + doneCont) + 1;
            var ticksPassed = (DateTime.Now.ToLocalTime().Ticks - updateStartTime.Ticks);
            var timeEst = new TimeSpan(ticksPassed / (index + 1) * updateEstimation - ticksPassed);
            if (!minimumOutput)
                Console.Write($"\r Loading: ({i}/{max}) Done With: {doneCont} Total:{sum} {timeEst:mm\\:ss}");
        }

        // builds the index for all auctions in the last hour

        async Task<int> Save(GetAuctionPage res, DateTime lastUpdate)
        {
            int count = 0;


            var processed = res.Auctions.Where(item =>
                {
                    // nothing changed if the last bid is older than the last update
                    return !(item.Bids.Count > 0 && item.Bids[item.Bids.Count - 1].Timestamp < lastUpdate ||
                        item.Bids.Count == 0 && item.Start < lastUpdate);
                })
                .Select(a =>
                {
                    if (Program.Migrated)
                        ItemDetails.Instance.AddOrIgnoreDetails(a);
                    count++;
                    var auction = new SaveAuction(a);
                    return auction;
                }).ToList();


            if (DateTime.Now.Minute % 30 == 7)
                foreach (var a in res.Auctions)
                {
                    var auction = new SaveAuction(a);
                    AuctionCount.AddOrUpdate(auction.Tag, k =>
                    {
                        return DetermineWorth(0, auction);
                    }, (k, c) =>
                    {
                        return DetermineWorth(c, auction);
                    });
                }

            var ended = res.Auctions.Where(a => a.End < DateTime.Now).Select(a => new SaveAuction(a));
            /* var variableHereToRemoveWarning = taskFactory.StartNew(async () =>
             {
                 await Task.Delay(TimeSpan.FromSeconds(20));
                 await ItemPrices.Instance.AddEndedAuctions(ended);
             });*/


            if (Program.FullServerMode)
                Indexer.AddToQueue(processed);
            else
                FileController.SaveAs($"apull/{DateTime.Now.Ticks}", processed);


            var started = processed.Where(a => a.Start > lastUpdate).ToList();

            // do not slow down the update
            var min = DateTime.Now - TimeSpan.FromMinutes(15);
            Flipper.FlipperEngine.Instance.NewAuctions(started.Where(a=>a.Start > min));
            foreach (var auction in started)
            {
                SubscribeEngine.Instance.NewAuction(auction);
            }





            return count;
        }

        private static int DetermineWorth(int c, SaveAuction auction)
        {
            var price = auction.HighestBidAmount == 0 ? auction.StartingBid : auction.HighestBidAmount;
            if (price > 500_000)
                return c + 1;
            return c - 20;
        }

        internal void Stop()
        {
            abort = true;
        }
    }
}
