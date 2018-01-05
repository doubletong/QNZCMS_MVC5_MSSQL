using AutoMapper;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using TZGCMS.Data.Entity.Ads;
using TZGCMS.Model.Front.ViewModel.Ads;
using TZGCMS.Service.Ads;

namespace SiteWeb.Controllers.api
{
    /// <summary>
    /// 轮播图，广告图
    /// </summary>
    public class CarouselsController : ApiController
    {

        private readonly ICarouselServices _carouselServices;
        private readonly IPositionServices _positionServices;
        private readonly IMapper _mapper;
        /// <summary>
        /// 注入
        /// </summary>
        /// <param name="carouselServices"></param>
        public CarouselsController(ICarouselServices carouselServices, IPositionServices positionServices, IMapper mapper)
        {
            _carouselServices = carouselServices;
            _positionServices = positionServices;
            _mapper = mapper;
        }
              

        /// <summary>
        /// 获取广告位图片
        /// </summary>
        /// <param name="code">广告位代码</param>
        /// <returns></returns>
        // GET: api/Carousels/5
        [ResponseType(typeof(CarouselFVM))]
        public async Task<IHttpActionResult> GetCarousels(string code)
        {
            var position = await _positionServices.GetByCode(code);
            if (position == null || !position.Carousels.Any())
            {
                return NotFound();
            }
            var vm = _mapper.Map<IEnumerable<Carousel>, IEnumerable<CarouselFVM>>(position.Carousels);

            return Ok(vm);
        }
        

       
    }
}