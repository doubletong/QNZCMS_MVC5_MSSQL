using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.ModelBinding;
using Newtonsoft.Json;
using TZGCMS.Data.Entity;
using TZGCMS.Data.Entity.Videos;
using TZGCMS.Infrastructure.Cache;
using TZGCMS.Infrastructure.Configs;
using TZGCMS.Infrastructure.Helper;
using TZGCMS.Infrastructure.Logging;
using TZGCMS.Model.Front.ViewModel.Videos;
using TZGCMS.Service.Videos;
using TZGCMS.WeiXin;
using TZGCMS.WeiXin.Model;

namespace TZGCMS.SiteWeb.Controllers.api
{
    /// <summary>
    /// 视频直播
    /// </summary>
    public class VideosController : ApiController
    {
        private readonly IVideoCategoryServices _categoryServices;
        private readonly IVideoServices _videoServices;
        private readonly IReservationServices _reservationServices;
        private readonly ICacheService _cacheServices;
        private readonly ILoggingService _logger;
        private readonly IMapper _mapper;

       
        public VideosController(IVideoCategoryServices categoryServices, IVideoServices videoServices, 
            IReservationServices reservationServices, ICacheService cacheServices, 
            ILoggingService logger,IMapper mapper)
        {           
            _categoryServices = categoryServices;
            _videoServices = videoServices;
            _reservationServices = reservationServices;
            _cacheServices = cacheServices;
            _logger = logger;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取直播视频分类
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(VideoCategoryFVM))]
        public async Task<IHttpActionResult> GetCategories()
        {
            var categories = await _categoryServices.GetActiveElementsAsync();
            var vm = _mapper.Map<IEnumerable<VideoCategory>, IEnumerable<VideoCategoryFVM>>(categories);
            return Ok(vm);
        }

        /// <summary>
        /// 获取分页直播视频
        /// </summary>
        /// <param name="categoryId">分类ID,如果为0，获取不分类</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [ResponseType(typeof(VideoFVM))]
        public IHttpActionResult GetVidoes(int categoryId=0, int page = 1)
        {
            var pageIndex = page - 1;
            var pageSize = SettingsManager.Video.FrontPageSize;
            int count;
            var videos = _videoServices.GetActivePagedElements(pageIndex, pageSize,string.Empty,categoryId, out count);
            var vm = _mapper.Map<List<Video>, List<VideoFVM>>(videos);

            if (User == null || !User.Identity.IsAuthenticated) return Ok(vm);

            var openId = User.Identity.Name;
            var rese = _reservationServices.GetElementsByOpenId(openId);

            var videoIds = rese.Select(d => d.VideoId).ToArray();

            foreach (var item in vm)
            {
                if (videoIds.Contains(item.Id))
                {
                    item.IsReservation = true;
                }
            }
            // return Ok(vm);

            return Ok(vm);
        }


        /// <summary>
        /// 获取分页往期回放视频
        /// </summary>
        /// <param name="categoryId">分类ID,如果为0，获取不分类</param>
        /// <param name="page">页码</param>
        /// <returns></returns>
        [ResponseType(typeof(VideoFVM))]
        public IHttpActionResult GetArchiveVidoes(int categoryId=0, int page = 1)
        {
            var pageIndex = page - 1;
            var pageSize = SettingsManager.Video.FrontPageSize;
            int count;
            var videos = _videoServices.GetActiveArchivePagedElements(pageIndex, pageSize, string.Empty, categoryId, out count);
          
            var vm = _mapper.Map<List<Video>, List<VideoFVM>>(videos);
            return Ok(vm);
        }
        /// <summary>
        /// 预约视频,如果返回401状态，跳转 "http://域名/" 重新授权
        /// </summary>
        /// <param name="videoId"></param>
        /// <returns></returns>
        [HttpPost]
        [ResponseType(typeof(bool))]
        public async Task<HttpResponseMessage> ReservationVideo(int videoId)
        {
          
            if (!User.Identity.IsAuthenticated)
            {
                return Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "还未授权");
            }
            var openId = User.Identity.Name;
            try
            {
                var video = _videoServices.GetById(videoId);

                if (video == null)
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "不存在此视频！");
                }

                var vm = new Reservation
                {
                    VideoId = videoId,
                    OpenId = openId.ToString(),
                    CreatedDate = DateTime.Now
                };

                var message = new SingleSendMessagesVM { ToUser = openId.ToString(), MsgType = "text", Text = new Message { Content = $"你已成功预约了直播：【{video.Title}】" } };
                var returnMsg = "预约成功";

                var res = _reservationServices.GetById(vm.VideoId, vm.OpenId);

                if (res != null)
                {
                    _reservationServices.Delete(res);
                    message.Text = new Message { Content = $"你已成功取消了直播：【{video.Title}】的预约" };
                    returnMsg = "已取消预约";
                }
                else
                {
                    _reservationServices.Create(vm);
                }
                
               
                var json = JsonConvert.SerializeObject(message, Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings { ContractResolver = new LowercaseContractResolver() });
              
                HttpContent content = new StringContent(json);
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                string accesstoken;
                if (_cacheServices.IsSet("access_token"))
                {
                    accesstoken = (string)_cacheServices.Get("access_token");
                }
                else
                {
                    var atvm = await WeChatHepler.GetAccessTokenAsync(SettingsManager.WeiXin.AppId, SettingsManager.WeiXin.AppSecret);
                    accesstoken = atvm.access_token;
                    _cacheServices.Set("access_token", accesstoken, 120);
                }
                var result = await WeChatHepler.SingleSendMessagesAsync(accesstoken, content);
                _logger.Info($"单发结果[errcode:{result.ErrCode},errmsg:{result.ErrMsg}]；消息主体：{json}");

                return Request.CreateResponse(HttpStatusCode.OK, returnMsg);
            }
            catch (Exception ex)
            {
                _logger.Fatal(ex);
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message,ex);
            }
           
        }

        [HttpGet]
        public IHttpActionResult test()
        {
           // var openId = System.Web.HttpContext.Current.Session["openid"];
            return Ok(System.Web.HttpContext.Current.User.Identity.Name);

        }
        [HttpGet]
        public async Task<IHttpActionResult> testb()
        {
            // var openId = System.Web.HttpContext.Current.Session["openid"];
            var videos = await _videoServices.GetNoticedElementsAsync();
            return Ok(videos.Select(d =>new
                {
                    d.Title,d.StartDate
                }
            ));

        }
        
    }
}