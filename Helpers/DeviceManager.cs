using SharpAdbClient;
using System.Net;

namespace SynthMapUpdate.Helpers
{
    public class DeviceManager
    {
        private AdbClient AdbClient { get; set; } = new();
        private static DeviceData? QuestDevice { get; set; }

        public string StartAdbServer(string adbPath)
        {
            var server = new AdbServer();
            server.StartServer(adbPath, restartServerIfNewer: false);

            return server.GetStatus().ToString();
        }

        public bool GetDeviceByModel(string model)
        {
            try
            {
                var devices = AdbClient.GetDevices();
                var questDevice = devices.Where(x => x.Model == model).First();
                if (questDevice == null)
                {
                    return false;
                }

                QuestDevice = questDevice;

                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CheckSynthFolder()
        {
            try
            {
                string command = $"ls {Globals.CustomSongsFolderPath} | grep .none";
                var receiver = new ConsoleOutputReceiver();
                await AdbClient.ExecuteRemoteCommandAsync(command, QuestDevice, receiver, CancellationToken.None);

                string result = receiver.ToString();
                if (result.EndsWith(": No such file or directory\n"))
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<List<string>> GetSynthFiles()
        {
            try
            {
                var beatmaps = new List<string>();

                var command = $"ls {Globals.CustomSongsFolderPath} | grep .synth";
                var receiver = new ConsoleOutputReceiver();
                await AdbClient.ExecuteRemoteCommandAsync(command, QuestDevice, receiver, CancellationToken.None);

                using StringReader reader = new(receiver.ToString());
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    beatmaps.Add(line);
                }

                return beatmaps;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return [];
            }
        }

        public static void UploadSynthFile(Stream file, string fileName)
        {
            try
            {
                string filePath = $"{Globals.CustomSongsFolderPath}{fileName}";

                using var service = new SyncService(new AdbSocket(new IPEndPoint(IPAddress.Loopback, AdbClient.AdbServerPort)), QuestDevice);

                service.Push(file, filePath, 660, DateTime.Now, null, CancellationToken.None);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
