using SynthMapUpdate.Helpers;

// Create helper class
var deviceManager = new DeviceManager();

// Start adb server
string adbPath = "/usr/bin/adb";
var server = deviceManager.StartAdbServer(adbPath);
Console.WriteLine("Server: {0}", server);

// Get quest 3 by model
string deviceModel = "Quest_3";
var device = deviceManager.GetDeviceByModel(deviceModel);
if (!device)
{
    Console.WriteLine("{0} not found", deviceModel);
    return;
}

// Check for custom songs folder
bool customSongsFolder = await deviceManager.CheckSynthFolder();
if (!customSongsFolder)
{
    Console.WriteLine("Custom songs folder not found!");
    return;
}

// Get current beatmaps on device
var beatmaps = await deviceManager.GetSynthFiles();
Console.WriteLine("{0} maps found", beatmaps.Count);

// Get beatmaps from synthriderz.com
var synthApi = new SynthriderzApiData();
var apiPageList = await synthApi.DownloadJsonPages();
Console.WriteLine("{0} pages fetched from api.synthriderz.com", apiPageList.Count);

// Check and download new maps
Console.WriteLine("Checking for new maps...");
var beatmapManager = new BeatmapManager(beatmaps);
var count = beatmapManager.CheckForNewBeatmaps(apiPageList);
if (count > 0)
{
    Console.WriteLine("{0} new maps found", count);
    await beatmapManager.DownloadQueue();
}

Console.WriteLine("You are up to date!");
