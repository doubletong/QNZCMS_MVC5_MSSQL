/**
 * Created by striving on 2019/5/8.
 */
var mobileFlag=true;
if ((navigator.userAgent.match(/(phone|pad|pod|iPhone|iPod|ios|iPad|Android|Mobile|BlackBerry|IEMobile|MQQBrowser|JUC|Fennec|wOSBrowser|BrowserNG|WebOS|Symbian|Windows Phone)/i))) {
    mobileFlag=false
}
var wow = new WOW({
    boxClass: 'wow',
    animateClass: 'animated',
    offset: -100,
    mobile: mobileFlag,
    live: true
});
wow.init();

$(function(){
    //years
    $('.current_year').click(function(){
        $('.years_list').slideToggle(200)
    });
    //product_detail_video
    $('.product_detail_video_wrap .play_btn').click(function(){
        $(this).fadeOut(100);
        $('.product_detail_video_wrap video')[0].play();
        $('.product_detail_video_wrap video')[0].controls = true;
    });
    //product_detail_case
    var productDetailCaseSwiperView= 3,productDetailCaseSwiperColumn= 1,productDetailCaseSwiperBetween=20;
    if($(window).width()<600){
        productDetailCaseSwiperView=2;
        productDetailCaseSwiperColumn=1;
        productDetailCaseSwiperBetween=10
    }
    var productDetailCaseSwiper = new Swiper('.product_detail_case_wrap .swiper-container',{
        speed:600,
        slidesPerView : productDetailCaseSwiperView,
        slidesPerGroup : productDetailCaseSwiperView,
        slidesPerColumn : productDetailCaseSwiperColumn,
        spaceBetween : productDetailCaseSwiperBetween,
        pagination : '.product_detail_case_pagination',
        paginationClickable :true,
        paginationBulletRender: function (swiper, index, className) {
            return '<span class="' + className + '">' + (index + 1) + '</span>';
        }
    });
    //about_team
    var aboutTeamSwiperView=5;
    if($(window).width()<993){
        aboutTeamSwiperView=3
    }
    if($(window).width()<769){
        aboutTeamSwiperView=2
    }
    var aboutTeamSwiper = new Swiper('.about_team_nav .swiper-container', {
        slidesPerView: aboutTeamSwiperView,
        slidesPerGroup : aboutTeamSwiperView,
        spaceBetween: 1,
        prevButton:'.about_team_nav .swiper-button-prev',
        nextButton:'.about_team_nav .swiper-button-next'
    });
    $('.about_team_nav .swiper-slide').click(function(){
        $(this).addClass('active').siblings().removeClass('active');
        $('.about_team_b_item').eq($(this).index()).addClass('active').siblings().removeClass('active')
    });
    //about_team_detail
    var aboutTeamDetailSwiperView=5;
    var flag=true;
    if($(window).width()<1367){
        aboutTeamDetailSwiperView=3
    }
    if($(window).width()<600){
        aboutTeamDetailSwiperView=2;
    }
    var aboutTeamDetailSwiper = new Swiper('.about_team_detail_list .swiper-container', {
        slidesPerView: aboutTeamDetailSwiperView,
        centeredSlides: flag,
        loop:true,
        spaceBetween: '1%',
        prevButton:'.about_team_detail_list .swiper-button-prev',
        nextButton:'.about_team_detail_list .swiper-button-next'
    });

});
//tab_nav
function domTab(id){
    $('.tab_nav li').click(function(){
        $(this).addClass('active').siblings().removeClass('active');
        $(id).eq($(this).index()).addClass('active').siblings().removeClass('active')
    })
}