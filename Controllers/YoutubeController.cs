
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YoutubeExplode;
using System.Text;
using System.Net.Http.Headers;
using System.Net.Http;
using System.IO;
using YoutubeExplode.Channels;
using System.Net;
using System.Text.RegularExpressions;
using System.Drawing;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Videos;
using AngleSharp.Common;
using System.Runtime.InteropServices;

namespace DL.Controllers
{
    [Route("Grabber")]
    [ApiController]
 
    public class YoutubeController : ControllerBase
    {
        private static bool ChannelIsValid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            // Channel IDs should start with these characters
            if (!id.StartsWith("UC", StringComparison.Ordinal))
                return false;

            // Channel IDs are always 24 characters
            if (id.Length != 24)
                return false;

            return !Regex.IsMatch(id, @"[^0-9a-zA-Z_\-]");
        }
       public string fixstr(string inputString){
     
            string pattern = " *[\\~#%&*{}/:<>?|\"-]+ *";
            string replacement = "_";

            Regex regEx = new Regex(pattern);
            string sanitized = regEx.Replace(inputString, replacement);
string asAscii = Encoding.ASCII.GetString(
    Encoding.Convert(
        Encoding.UTF8,
        Encoding.GetEncoding(
            Encoding.ASCII.EncodingName,
            new EncoderReplacementFallback("_"),
            new DecoderExceptionFallback()
            ),
        Encoding.UTF8.GetBytes(sanitized)
    )
);
return asAscii.Replace(';',' ');
    }
        [HttpGet("Video/{id}")]
        public async Task<FileStreamResult> DownloadAsync(string id)
        {
            var YT = new YoutubeClient();
            var videoname0 = await YT.Videos.GetAsync(id);


            var strs = await YT.Videos.Streams.GetManifestAsync(id);
            var high = strs.GetMuxed().WithHighestVideoQuality();
            string sanitized = fixstr(videoname0.Title);

            return File(YT.Videos.Streams.GetAsync(high).Result, $"video/{high.Container.Name}", $"{sanitized}.{high.Container.Name}", true);
        }
   
        [HttpGet("VideoHigh/{id}")]
        public async Task<FileStreamResult> DownloadOnlyVideoAsync(string id)
        {
            var YT = new YoutubeClient();
            var videoname0 = await YT.Videos.GetAsync(id);


           string sanitized = fixstr(videoname0.Title);
           var strs = await YT.Videos.Streams.GetManifestAsync(id);
            var high = strs.GetVideo().WithHighestVideoQuality();


            return File(YT.Videos.Streams.GetAsync(high).Result, $"video/{high.Container.Name}", $"{sanitized}.{high.Container.Name}", true);
        }
        [HttpGet("Audio/{id}")]
        public async Task<FileStreamResult> DownloadAudioAsync(string id)
        {
            var YT = new YoutubeClient();
            var videoname0 = await YT.Videos.GetAsync(id);


           string sanitized = fixstr(videoname0.Title);
           var strs = await YT.Videos.Streams.GetManifestAsync(id);
            var high = strs.GetAudioOnly().WithHighestBitrate();


            return File(YT.Videos.Streams.GetAsync(high).Result, $"audio/{high.Container.Name}", $"{sanitized}.{high.Container.Name}", true);

        }

        [HttpGet("Search/{id}")]
        public async Task<IActionResult> SearchAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids1 = YT.Search.GetVideosAsync(id);
            var vids = await vids1.ToListAsync();
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids)
            {

                s += $"{v.Id}\n";
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "query.txt", true);
        }
         [HttpGet("ChannelNew2/{date}/{id}")]
        public async Task<IActionResult> NewUser2VideosAsync(string date, string id)
        {
            var YT = new YoutubeClient();
         var vids = ChannelIsValid(id) ? new ChannelId(id) : (await YT.Channels.GetByUserAsync(id)).Id;
            var ien = YT.Channels.GetUploadsAsync(vids);
            var vids2 = await ien.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                if (v.UploadDate.Date >= new DateTime(int.Parse(date.Split("-")[0]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[2])))
                {


                    var fileName = v.Title;

                     string sanitized = fixstr(fileName).Replace(' ','*');

                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;

                } }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Channel2/{id}")]
        public async Task<IActionResult> User2VideosAsync(string id)
        {
        var YT = new YoutubeClient();

            var vids = ChannelIsValid(id) ? new ChannelId(id) : (await YT.Channels.GetByUserAsync(id)).Id;
            var ien = YT.Channels.GetUploadsAsync(vids);
            var vids2 = await ien.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                var fileName = v.Title;


                     string sanitized = fixstr(fileName).Replace(' ','*');

                s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                videos++;

            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("User/{id}")]
        public async Task<IActionResult> UserVideosAsync(string id)
        {
            var YT = new YoutubeClient();

            var vids = await YT.Channels.GetByUserAsync(id);
            var ien = YT.Channels.GetUploadsAsync(vids.Id);
            var vids2 = await ien.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                var fileName = v.Title;


                     string sanitized = fixstr(fileName).Replace(' ','*');

                s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                videos++;

            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("UserNew/{date}/{id}")]
        public async Task<IActionResult> NewUserVideosAsync(string date, string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.Channels.GetByUserAsync(id);
            var ien = YT.Channels.GetUploadsAsync(vids.Id);
            var vids2 = await ien.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                if (v.UploadDate.Date >= new DateTime(int.Parse(date.Split("-")[0]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[2])))
                {


                    var fileName = v.Title;

                     string sanitized = fixstr(fileName).Replace(' ','*');

                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;

                } }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Channel/{id}")]
        public async Task<IActionResult> ChannelVideosAsync(string id)
        {
            var YT = new YoutubeClient();


            var ien = YT.Channels.GetUploadsAsync(id);
            var vids2 = await ien.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                var fileName = v.Title;
 string sanitized = fixstr(fileName).Replace(' ','*');
                s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                videos++;

            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("ChannelNew/{date}/{id}")]
        public async Task<IActionResult> NewChannelVideosAsync(string date, string id)
        {
            var YT = new YoutubeClient();

            var ien = YT.Channels.GetUploadsAsync(id);
            var vids2 = await ien.ToListAsync();

            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                if (v.UploadDate.Date >= new DateTime(int.Parse(date.Split("-")[0]), int.Parse(date.Split("-")[1]), int.Parse(date.Split("-")[2])))
                {


                    var fileName = v.Title;
 string sanitized = fixstr(fileName).Replace(' ','*');

                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;

                }
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("searchinfo/{id}")]
        public async Task<IActionResult> SearchInfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = YT.Search.GetVideosAsync(id);
            var vids2 = await vids.ToListAsync();
            int videos = 0;
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                var fileName = v.Title;

                string sanitized = fixstr(fileName).Replace(" ", "*");

                if (videos < 499) {
                    s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";
                    videos++;
                }
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Playlist/{id}")]
        public async Task<IActionResult> PlaylistAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = YT.Search.GetVideosAsync(id);
            var vids2 = await vids.ToListAsync();
            string s = "";
            foreach (YoutubeExplode.Videos.Video v in vids2)
            {
                s += $"{v.Id}\n";
            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "query.txt", true);
        }
        [HttpGet("playlistinfo/{id}")]
        public async Task<IActionResult> PlaylistInfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = YT.Playlists.GetVideosAsync(id);
            var vids2 = await vids.ToListAsync();
            string s = "";

            foreach (YoutubeExplode.Videos.Video v in vids2)
            {

                 var fileName = v.Title;

                     string sanitized = fixstr(fileName).Replace(' ','*');
                s += $"{v.Id} {sanitized.Substring(0, Math.Min(sanitized.Length, 439))} {v.Author.Replace("*", "_").Replace(" ", "*").Substring(0, Math.Min(v.Author.Replace(" ", "-").Length, 439))} {v.Engagement.LikeCount} {v.Engagement.DislikeCount} {v.Engagement.ViewCount} {v.UploadDate.ToString("MM/dd/yyyy")} {v.Duration.Hours}:{v.Duration.Minutes}:{v.Duration.Seconds} ENDLINE\n";

            }


            return File(ASCIIEncoding.Default.GetBytes(s), "text/plain", "info.txt", true);
        }
        [HttpGet("Info/{id}")]
        public async Task<IActionResult> InfoAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.Videos.GetAsync(id);



            return File(ASCIIEncoding.Default.GetBytes($"{vids.Title}\n{vids.Thumbnails.HighResUrl}\n{vids.Author}"), "text/plain", "info.txt", true);
        }
        [HttpGet("jpg/{id}")]
        public async Task<IActionResult> Jpg(string id)
        {
            var YT = new YoutubeClient();
            var strs = await YT.Videos.GetAsync(id);
            return Redirect(strs.Thumbnails.HighResUrl);
        }
        private bool TryNormalizeVL(string videoUri, out string normalized)
        {
            // If you fix something in here, please be sure to fix in 
            // DownloadUrlResolver.TryNormalizeYoutubeUrl as well.

            normalized = null;

            var builder = new StringBuilder(videoUri);

            videoUri = builder.Replace("youtu.be/", "youtube.com/watch?v=")
                .Replace("youtube.com/embed/", "youtube.com/watch?v=")
                .Replace("/v/", "/watch?v=")
                .Replace("/watch#", "/watch?")
                .ToString();

            var query = new Query(videoUri);

            string value;

            if (!query.TryGetValue("v", out value))
                return false;

            normalized = "https://youtube.com/watch?v=" + value;
            return true;
        }
        [HttpGet("ConvertString/{id}/{prefix}")]
        public async Task<IActionResult> REDIR(string id,string prefix){
            string newid;
            TryNormalizeVL(id,out newid);
            return Redirect(prefix+newid);
        }
        [HttpGet("Descript/{id}")]
        public async Task<IActionResult> DescAsync(string id)
        {
            var YT = new YoutubeClient();
            var vids = await YT.Videos.GetAsync(id);



            return File(ASCIIEncoding.Default.GetBytes(vids.Description), "text/plain", "desc.txt");
        }
       
    [HttpGet("ChannelImage/{channel}")]
    public async Task<IActionResult> ChannelImage(string channel)
    {
        var YT = new YoutubeClient();
    var  channe2l=  await  YT.Channels.GetAsync(channel);
         
        return Redirect(channe2l.LogoUrl);

    }
        [HttpGet("UserImage/{channel}")]
        public async Task<IActionResult> UserImage(string channel)
        {
            var YT = new YoutubeClient();
            var channe2l = await YT.Channels.GetByUserAsync(channel);

            return Redirect(channe2l.LogoUrl);

        }
    }
}






