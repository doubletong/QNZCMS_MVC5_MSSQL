/// <reference path="jquery-2.1.1.js" />

if (!Modernizr.svg) {
    $('img[src*="svg"]').attr('src', function () {
        return $(this).attr('src').replace('.svg', '.png');
    });
}




//社交图标动画
$('.social a.animated').hover(function () {
    $(this).addClass('bounce');
}, function () {
    $(this).removeClass('bounce');
});

$('.social a.weixin').hover(function () {
    $(this).children('.qrcode').fadeIn();
}, function () {
    $(this).children('.qrcode').fadeOut();
});


$(function () {


    var winheight = $(window).height();
    //$('.categories').css({ 'marginTop': winheight + 'px' });
    $('#slider').height(winheight - 31 - $('#pageHeader').height());

    $(window).resize(function () {
        var winheight = $(window).height();
        $('#slider').height(winheight - 31-$('#pageHeader').height());
    });

    
    $('.openav a').click(function (e) {
        e.preventDefault();
        $('#nav-overlying').addClass('flipInY').fadeIn(1000,function () {
            $(this).removeClass('flipInY');
        });
    });

    //关闭导航层
    $('.overlying a.close').click(function (e) {
        e.preventDefault();
        $('#nav-overlying').addClass('flipOutY').fadeOut(1000, function () {
            $(this).removeClass('flipOutY');
        });
    });

    $('.encircle a').click(function (e) {

        e.preventDefault();
        var topHeight = $(".slider-list li").outerHeight() + 92;
        $('html body').animate({ scrollTop: topHeight }, 600);
    });


    // 鼠标经过产品动画
    $('#lastproductlist a.item, #productlist a').hover(function () {
        $(this).children('div.roll').stop().animate({ 'top': '0' },600);
    }, function () {
        $(this).children('div.roll').stop().animate({ 'top': '100%' }, 600);
    })

   

    // 浮动菜单 ===================================================================
    $(window).scroll(function () {
     
        if ($(document).scrollTop() < 31) {
            $('#pageHeader').removeClass('fixed-header');           
        } else {
            $('#pageHeader').addClass('fixed-header');           
        }
    });


    //响应式导航 ===================================================================
    $("#mainav>li.downav").hover(function () {        
        $(this).find(".subnav").fadeIn();
    }, function () {
        $(this).find(".subnav").fadeOut();
    });

    $(".openmenu a").click(function (e) {

        $("#pusher").fadeIn();
        $("#rightnav").animate({ 'right': '0px' });
        e.preventDefault();
    });

    $("#pusher").click(function () {
        $(this).fadeOut();
        $("#rightnav").animate({ 'right': '-150px' });
    });

    
});


