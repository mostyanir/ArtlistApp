// using System;
// using System.Runtime.InteropServices;
// using FFmpeg.AutoGen;

using System;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
// using FFmpeg.AutoGen;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Xabe.FFmpeg;
using Xabe.FFmpeg.Enums;
using System.Threading.Tasks;
using Xabe.FFmpeg.Model;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ArtlistApp.API.Controllers
{
    [Route("api/videos")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public VideosController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }


        [HttpPost("upload"), DisableRequestSizeLimit]
        public async Task<IActionResult> Upload()
        {
            try
            {
                var file = Request.Form.Files[0];
                var folderName = Path.Combine("Resources", "Videos");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    fileName = fileName.Replace(" ", "_");
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
        
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                    //GetThumbnail(dbPath);
                    var response = await doTheMagic(fullPath);
                    return Ok(response);
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"upload failed - {ex}");
                return StatusCode(500, $"Internal server error - {ex}");
            }
        }

        private static async Task<Object> doTheMagic(String url)
        {
            //Set directory where app should look for FFmpeg executables.
            FFmpeg.ExecutablesPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FFmpeg");
            //Get latest version of FFmpeg. It's great idea if you don't know if you had installed FFmpeg.
            await FFmpeg.GetLatestVersion().ConfigureAwait(false);

            var seconds = new List<int> (new int[] {1, 3});
            seconds.Sort();

            var snapshotsList= await GetThumbnails(url, seconds);
        
            var fileToConvert = new FileInfo(url);
            //Run conversion
            var h264Path = await RunConversion(fileToConvert).ConfigureAwait(false);

            var m3u8Path = await GenerateHlsFiles(url, fileToConvert);

            var response = new {items = new [] {
                new {snapshots = snapshotsList , h264Url = h264Path, m3u8Path = m3u8Path}
            }};

            return response;
        }

        private static async Task<List<string>> GetThumbnails(String url, List<int> seconds)
        {
            var folderName = Path.Combine("Resources", "output");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var snapshotsArray = new List<string>();
            
            foreach (int s in seconds.ToArray())
                {
                    string output = Path.Combine(pathToSave, Guid.NewGuid() + "_s" + s + FileExtensions.Png);
                    IConversionResult result = await Conversion.Snapshot(url, output, TimeSpan.FromSeconds(s)).Start();
                    snapshotsArray.Add(output);
                }
            return snapshotsArray;
        }


        private static async Task<string> GenerateHlsFiles(String url, FileInfo fileToConvert)
        {
            var fileName = fileToConvert.Name.Split('.')[0];
            var folderName = Path.Combine("Resources", "hls");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var execPath = Directory.GetCurrentDirectory() + "/ffmpeg";

            try{
                Process process = new Process();
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                
                process.StartInfo.FileName = execPath;

                process.StartInfo.Arguments = "-i " + url + " -codec: copy -start_number 0 -hls_time 10 -hls_list_size 0 -f hls " + pathToSave + "/" + fileName + ".m3u8";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true ;
                process.Start();

                //Uri fileUri = new Uri(new Uri("file://"), url);
                //IConversionResult conv = await Conversion.SaveM3U8Stream(fileUri, pathToSave + "/" + fileToConvert.Name.Split('.')[0]).Start();
            
            }catch (Exception ex){
                Console.WriteLine("Exception caught.", ex);
            }
            var m3u8Path = pathToSave + "/" + fileName + ".m3u8";
            return m3u8Path;
        }

        private static async Task<string> RunConversion(FileInfo fileToConvert)
        {
            //Save file to the same location with changed extension 
            string outputFileName = fileToConvert.Extension == ".mp4" ?
            fileToConvert.DirectoryName + "/" + fileToConvert.Name.Split('.')[0] + "_2.mp4"
            : Path.ChangeExtension(fileToConvert.FullName, ".mp4");

            var mediaInfo = await MediaInfo.Get(fileToConvert).ConfigureAwait(false);
            var videoStream = mediaInfo.VideoStreams.First();
            var audioStream = mediaInfo.AudioStreams.First();

            //Change some parameters of video stream
            videoStream
                //Set size to 480p
                .SetSize(VideoSize.Hd480)
                //Set codec which will be used to encode file. If not set it's set automatically according to output file extension
                .SetCodec(VideoCodec.H264);

            //Create new conversion object
            var conversion = Conversion.New()
                //Add video stream to output file
                .AddStream(videoStream)
                //Add audio stream to output file
                .AddStream(audioStream)
                //Set output file path
                .SetOutput(outputFileName)
                //SetOverwriteOutput to overwrite files. It's useful when we already run application before
                .SetOverwriteOutput(true)
                //Disable multithreading
                .UseMultiThread(false)
                //Set conversion preset. You have to chose between file size and quality of video and duration of conversion
                .SetPreset(ConversionPreset.UltraFast);
            //Add log to OnProgress
            conversion.OnProgress += async (sender, args) =>
            {
                //Show all output from FFmpeg to console
                await Console.Out.WriteLineAsync($"[{args.Duration}/{args.TotalLength}][{args.Percent}%] {fileToConvert.Name}").ConfigureAwait(false);
            };
            //Start conversion
            await conversion.Start().ConfigureAwait(false);

            await Console.Out.WriteLineAsync($"Finished converion file [{fileToConvert.Name}]").ConfigureAwait(false);

            return outputFileName;
        }

    }
}