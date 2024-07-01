namespace SynthMapUpdate.Helpers
{
    public class BeatmapManager(List<string> questSongList)
    {
        public List<string> QuestSongList { get; set; } = questSongList;
        private List<BeatmapRecord> Queue { get; set; } = [];

        public int CheckForNewBeatmaps(List<JsonPageData?> jsonPages)
        {
            jsonPages.ForEach(page => AddQueue(page));

            return Queue.Count;
        }

        private void AddQueue(JsonPageData? page)
        {
            if (page == null || page.Data == null) return;

            foreach (var beatmap in page.Data)
            {
                var exists = QuestSongList.Where(x => x == beatmap.Filename).FirstOrDefault();
                if (string.IsNullOrEmpty(exists))
                {
                    Queue.Add(new BeatmapRecord { Filename = beatmap.Filename, DownloadUrl = beatmap.DownloadUrl });
                }
            }
        }

        public async Task DownloadQueue()
        {
            try
            {
                if (Queue.Count == 0) return;

                using HttpClient httpClient = new();
                httpClient.BaseAddress = new Uri(Globals.SynthRidersDownloadBaseUrl);

                var parallelOptions = new ParallelOptions { MaxDegreeOfParallelism = 10 };
                await Parallel.ForEachAsync(Queue, parallelOptions, async (beatmap, token) =>
                {
                    if (!string.IsNullOrEmpty(beatmap.DownloadUrl) && !string.IsNullOrEmpty(beatmap.Filename))
                    {
                        Console.WriteLine($"Downloading {beatmap.Filename}");
                        var fileStream = await DownloadFileStream(beatmap.DownloadUrl);
                        if (fileStream != Stream.Null)
                        {
                            DeviceManager.UploadSynthFile(fileStream, beatmap.Filename);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Download failed {0}", ex.Message);
            }
        }

        private async Task<Stream> DownloadFileStream(string url)
        {
            using HttpClient httpClient = new();
            httpClient.BaseAddress = new Uri(Globals.SynthRidersDownloadBaseUrl);
            try
            {
                var fileStream = await httpClient.GetStreamAsync(url);
                return fileStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return Stream.Null;
            }
        }
    }

    public record class BeatmapRecord
    {
        public string? Filename { get; set; }
        public string? DownloadUrl { get; set; }
    }
}
