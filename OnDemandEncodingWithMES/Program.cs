using Microsoft.WindowsAzure.MediaServices.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace OnDemandEncodingWithMES
{
    class Program
    {
        // Read values from the App.config file.
        private static readonly string _mediaServicesAccountName =
            ConfigurationManager.AppSettings["MediaServicesAccountName"];
        private static readonly string _mediaServicesAccountKey =
            ConfigurationManager.AppSettings["MediaServicesAccountKey"];

        private static readonly string _mediaFiles =
                Path.GetFullPath(@"../..\Media");

        private static readonly string _presetFiles =
            Path.GetFullPath(@"../..\Presets");

        // Field for service context.
        private static CloudMediaContext _context = null;
        private static MediaServicesCredentials _cachedCredentials = null;

        static void Main(string[] args)
        {
            try
            {
                // Create and cache the Media Services credentials in a static class variable.
                _cachedCredentials = new MediaServicesCredentials(
                                _mediaServicesAccountName,
                                _mediaServicesAccountKey);
                // Used the chached credentials to create CloudMediaContext.
                _context = new CloudMediaContext(_cachedCredentials);


               
                // If you want to secure your high quality input media files with strong encryption at rest on disk,
                // use AssetCreationOptions.StorageEncrypted instead of AssetCreationOptions.None.

                IAsset inputAsset =
                    UploadFile(Path.Combine(_mediaFiles, @"BigBuckBunny.mp4"), AssetCreationOptions.None);

                IAsset thumbnailAsset = GenerateThumbnail(inputAsset, AssetCreationOptions.None);

                IAsset audioOnly = EncodeToAudioOnly(inputAsset, AssetCreationOptions.None);
                // If you want to secure your high quality encoded media files with strong encryption at rest on disk,
                // use AssetCreationOptions.StorageEncrypted instead of AssetCreationOptions.None.
                // 
                // If your asset is AssetCreationOptions.StorageEncrypted, 
                // make sure to call ConfigureClearAssetDeliveryPolicy defined below.

                IAsset encodedAsset =
                    EncodeToAdaptiveBitrateMP4s(inputAsset, AssetCreationOptions.None);


                // If your want to delivery a storage encrypted asset, 
                // you must configure the asset’s delivery policy.
                // Before your asset can be streamed, 
                // the streaming server removes the storage encryption and 
                //streams your content using the specified delivery policy.

                // ConfigureClearAssetDeliveryPolicy(encodedAsset);

                PublishAssetGetURLs(encodedAsset);
            }
            catch (Exception exception)
            {
                // Parse the XML error message in the Media Services response and create a new
                // exception with its content.
                exception = MediaServicesExceptionParser.Parse(exception);

                Console.Error.WriteLine(exception.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static public IAsset UploadFile(string fileName, AssetCreationOptions options)
        {
            IAsset inputAsset = _context.Assets.CreateFromFile(
                fileName,
                options,
                (af, p) =>
                {
                    Console.WriteLine("Uploading '{0}' - Progress: {1:0.##}%", af.Name, p.Progress);
                });

            Console.WriteLine("Asset {0} created.", inputAsset.Id);

            return inputAsset;
        }

        static public IAsset EncodeToAdaptiveBitrateMP4s(IAsset asset, AssetCreationOptions options)
        {

            // Prepare a job with a single task to transcode the specified asset
            // into a multi-bitrate asset.

            IJob job = _context.Jobs.CreateWithSingleTask(
                "Media Encoder Standard",
                "H264 Multiple Bitrate 720p",
                asset,
                "Adaptive Bitrate MP4",
                options);

            Console.WriteLine("Submitting transcoding job...");


            // Submit the job and wait until it is completed.
            job.Submit();

            job = job.StartExecutionProgressTask(
                j =>
                {
                    Console.WriteLine("Job state: {0}", j.State);
                    Console.WriteLine("Job progress: {0:0.##}%", j.GetOverallProgress());
                },
                CancellationToken.None).Result;

            Console.WriteLine("Transcoding job finished.");

            IAsset outputAsset = job.OutputMediaAssets[0];

            return outputAsset;
        }


        static public IAsset GenerateThumbnail(IAsset asset, AssetCreationOptions options)
        {

            // Load the XML (or JSON) from the local file.
            string configuration = File.ReadAllText(Path.Combine(_presetFiles, @"ThumbnailPreset_JSON.json"));

            IJob job = _context.Jobs.CreateWithSingleTask(
                "Media Encoder Standard",
                configuration,
                asset,
                "Thumbnail",
                options);

            Console.WriteLine("Submitting transcoding job...");


            // Submit the job and wait until it is completed.
            job.Submit();

            job = job.StartExecutionProgressTask(
                j =>
                {
                    Console.WriteLine("Job state: {0}", j.State);
                    Console.WriteLine("Job progress: {0:0.##}%", j.GetOverallProgress());
                },
                CancellationToken.None).Result;

            Console.WriteLine("Transcoding job finished.");

            IAsset outputAsset = job.OutputMediaAssets[0];

            return outputAsset;
        }

        static public IAsset EncodeToAudioOnly(IAsset asset, AssetCreationOptions options)
        {
            // Load the XML (or JSON) from the local file.
            string configuration = File.ReadAllText(Path.Combine(_presetFiles, @"AudioOnlyPreset_JSON.json"));

            IJob job = _context.Jobs.CreateWithSingleTask(
                "Media Encoder Standard",
                configuration,
                asset,
                "Audio only",
                options);

            Console.WriteLine("Submitting transcoding job...");


            // Submit the job and wait until it is completed.
            job.Submit();

            job = job.StartExecutionProgressTask(
                j =>
                {
                    Console.WriteLine("Job state: {0}", j.State);
                    Console.WriteLine("Job progress: {0:0.##}%", j.GetOverallProgress());
                },
                CancellationToken.None).Result;

            Console.WriteLine("Transcoding job finished.");

            IAsset outputAsset = job.OutputMediaAssets[0];

            return outputAsset;
        }


        static public void PublishAssetGetURLs(IAsset asset)
        {
            // Publish the output asset by creating an Origin locator for adaptive streaming,
            // and a SAS locator for progressive download.

            _context.Locators.Create(
                LocatorType.OnDemandOrigin,
                asset,
                AccessPermissions.Read,
                TimeSpan.FromDays(30));

            _context.Locators.Create(
                LocatorType.Sas,
                asset,
                AccessPermissions.Read,
                TimeSpan.FromDays(30));


            IEnumerable<IAssetFile> mp4AssetFiles = asset
                    .AssetFiles
                    .ToList()
                    .Where(af => af.Name.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase));

            // Get the Smooth Streaming, HLS and MPEG-DASH URLs for adaptive streaming,
            // and the Progressive Download URL.
            Uri smoothStreamingUri = asset.GetSmoothStreamingUri();
            Uri hlsUri = asset.GetHlsUri();
            Uri mpegDashUri = asset.GetMpegDashUri();

            // Get the URls for progressive download for each MP4 file that was generated as a result
            // of encoding.
            List<Uri> mp4ProgressiveDownloadUris = mp4AssetFiles.Select(af => af.GetSasUri()).ToList();


            // Display  the streaming URLs.
            Console.WriteLine("Use the following URLs for adaptive streaming: ");
            Console.WriteLine(smoothStreamingUri);
            Console.WriteLine(hlsUri);
            Console.WriteLine(mpegDashUri);
            Console.WriteLine();

            // Display the URLs for progressive download.
            Console.WriteLine("Use the following URLs for progressive download.");
            mp4ProgressiveDownloadUris.ForEach(uri => Console.WriteLine(uri + "\n"));
            Console.WriteLine();

            // Download the output asset to a local folder.
            string outputFolder = "job-output";
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }

            Console.WriteLine();
            Console.WriteLine("Downloading output asset files to a local folder...");
            asset.DownloadToFolder(
                outputFolder,
                (af, p) =>
                {
                    Console.WriteLine("Downloading '{0}' - Progress: {1:0.##}%", af.Name, p.Progress);
                });

            Console.WriteLine("Output asset files available at '{0}'.", Path.GetFullPath(outputFolder));
        }
    }
}
