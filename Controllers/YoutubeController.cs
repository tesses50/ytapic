
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode; 
using YoutubeExplode.Models.MediaStreams;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;

using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;

namespace DL.Controllers
{
    [Route("Grabber")]
    [ApiController]
    public class YoutubeController : ControllerBase
    {
       
        [HttpGet("Video/{id}")]
        public async Task<FileStreamResult> DownloadAsync(string id)
        {
            var YT = new YoutubeClient();
            var strs = await YT.GetVideoMediaStreamInfosAsync(id);
            var strm = strs.Muxed.WithHighestVideoQuality();

            return File(await YT.GetMediaStreamAsync(strm), $"video/{strm.Container.GetFileExtension()}", $"videoplayback.{strm.Container.GetFileExtension()}", true);
        }
		        [HttpGet("VideoHigh/{id}")]
        public async Task<FileStreamResult> DownloadOnlyVideoAsync(string id)
        {
            var YT = new YoutubeClient();
            var strs = await YT.GetVideoMediaStreamInfosAsync(id);
            var strm = strs.Video.WithHighestVideoQuality();

            return File(await YT.GetMediaStreamAsync(strm), $"video/{strm.Container.GetFileExtension()}", $"videoplayback.{strm.Container.GetFileExtension()}", true);
        }
        [HttpGet("Audio/{id}")]
        public async Task<FileStreamResult> DownloadAudioAsync(string id)
        {
            var YT = new YoutubeClient();
            var strs = await YT.GetVideoMediaStreamInfosAsync(id);
            var strm = strs.Audio.WithHighestBitrate();
            return File(await YT.GetMediaStreamAsync(strm), $"audio/{strm.Container.GetFileExtension()}", $"audioplayback.{strm.Container.GetFileExtension()}", true);

        }

        [HttpGet("Search/{id}")]
        public async Task<IActionResult> SearchAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.SearchVideosAsync(id);

            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                
                s += $"{v.Id}\n";
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "query.txt", true);
        }
                [HttpGet("User/{id}")]
        public async Task<IActionResult> UserVideosAsync(string id)
        {
            var YT = new YoutubeClient();
            string cid = await YT.GetChannelIdAsync(id);
            var vids = await YT.GetChannelUploadsAsync(cid,100);
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                var fileName = v.Title;

                string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                string replacement = "_";

                Regex regEx = new Regex(pattern);
                string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");

          
                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;
                
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("UserNew/{date}/{id}")]
        public async Task<IActionResult> NewUserVideosAsync(string date,string id)
        {
            var YT = new YoutubeClient();
            string cid = await YT.GetChannelIdAsync(id);
            var vids = await YT.GetChannelUploadsAsync(cid,100);

            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                if(v.UploadDate.Date >= new DateTime(int.Parse(date.Split("-")[0]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[2])))
                {

                
                var fileName = v.Title;

                string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                string replacement = "_";

                Regex regEx = new Regex(pattern);
                string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");

                
                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;
                
            }}


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Channel/{id}")]
        public async Task<IActionResult> ChannelVideosAsync(string id)
        {
            var YT = new YoutubeClient();
         
            var vids = await YT.GetChannelUploadsAsync(id,100);
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                var fileName = v.Title;

                string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                string replacement = "_";

                Regex regEx = new Regex(pattern);
                string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");

               
                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;
                
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("ChannelNew/{date}/{id}")]
        public async Task<IActionResult> NewChannelVideosAsync(string date, string id)
        {
            var YT = new YoutubeClient();
         
            var vids = await YT.GetChannelUploadsAsync(id);

            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                if (v.UploadDate.Date >= new DateTime(int.Parse(date.Split("-")[0]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[2])))
                {


                    var fileName = v.Title;

                    string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                    string replacement = "_";

                    Regex regEx = new Regex(pattern);
                    string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");

                  
                        s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                        videos++;
                    
                }
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("searchinfo/{id}")]
        public async Task<IActionResult> SearchInfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.SearchVideosAsync(id,12);
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids)
            {
                var fileName = v.Title;

                string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                string replacement = "_";

                Regex regEx = new Regex(pattern);
                string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");
               
                if (videos < 499) {
                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;
            }
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Playlist/{id}")]
        public async Task<IActionResult> PlaylistAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.GetPlaylistAsync(id);
            string s = "";
            foreach (YoutubeExplode.Models.Video v in vids.Videos)
            {
                s += $"{v.Id}\n";
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "query.txt", true);
        }
        [HttpGet("playlistinfo/{id}")]
        public async Task<IActionResult> PlaylistInfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.GetPlaylistAsync(id,100);
            string s = "";
            
            foreach (YoutubeExplode.Models.Video v in vids.Videos)
            {
               
                var fileName = v.Title;

                string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
                string replacement = "_";

                Regex regEx = new Regex(pattern);
                string sanitized = regEx.Replace(fileName, replacement).Replace(" ", "*");
                s += $"{v.Id} {sanitized.Substring(0,Math.Min(sanitized.Length,439))} {v.Author.Replace("*","_").Replace(" ", "*").Substring(0,Math.Min(v.Author.Replace(" ", "-").Length ,439))} {v.Statistics.LikeCount} {v.Statistics.DislikeCount} {v.Statistics.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";

            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Info/{id}")]
        public async Task<IActionResult> InfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.GetVideoAsync(id);



            return File(ASCIIEncoding.Default.GetBytes($"{vids.Title}\n{vids.Thumbnails.HighResUrl}\n{vids.Author}"), "text/plain", "info.txt", true);
        }
        [HttpGet("jpg/{id}")]
        public async Task<IActionResult> Jpg(string id)
        {
            var YT = new YoutubeClient();
            var strs = await YT.GetVideoAsync(id);
            return Redirect(strs.Thumbnails.HighResUrl);
        }

    }

  
}






