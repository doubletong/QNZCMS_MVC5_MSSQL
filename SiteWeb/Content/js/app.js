
$(document).ready(function() {
    //var e = function() { 40 < $(window).scrollTop() ? $("#site-header").addClass("fixheader") : $("#site-header").removeClass("fixheader") };
    //e(), $(window).on("scroll", function() { e() }), $("#toTop").click(function(e) { return e.preventDefault(), jQuery("html, body").animate({ scrollTop: 0 }, 600), !1 }), $(".weixincode").click(function(e) { e.preventDefault(), jQuery(".over").fadeIn() }), $(".over").click(function(e) { e.preventDefault(), jQuery(this).fadeOut() })


    $(".menu-toggle").on('click', function () {
        $(this).toggleClass("on");
        $("#mainav").slideToggle();
    });

    //$(".subnav a").on("touchstart", function (e) {
    //    "use strict";
    //    var link = $(this).attr("href");
    //    location.href = link;
    //});


    $("#btnsetemail").on("click", function(e) {
        e.preventDefault();
        var email = $("#inputEmail").val();
        if (email.length === 0) {
            alert("请输入邮箱！");
        } else {
            $.ajax({
                type: "post",
                url: "subscribe.php?email=" + email,
                success: function(response) {
                    alert(response);
                }
            });
        }

    });

    //$("#btnSearch").click(function(e){
    //    $(".site-header").toggleClass("forSearch");
    //});


    //$(".mainav li.down").hover(function(e){      
    //    $(this).find("dl").stop().fadeIn();       
    //},function(e){
    //    $(this).find("dl").stop().fadeOut();
    //});

    //$(".overmenu li.down>a").click(function (e) {
    //    e.preventDefault();
    //    $(this).next(".subnav").slideToggle();
    //    $(this).closest("li").toggleClass("open");
    //});
});

let openav = document.getElementById("openav");
openav.addEventListener("click", function () {

    this.classList.toggle("opened");
    document.getElementById("sitenav").classList.toggle("opened");
    document.getElementById("logo").classList.toggle("opened");

});

window.addEventListener('scroll', function (e) {
   
    const pageHeader = document.getElementById("pageHeader");
    const totop = window.scrollY;
    if (totop > 0) {
        pageHeader.classList.add("fixed-header");
    } else {
        pageHeader.classList.remove("fixed-header");
    }

});

const totop = document.getElementById("toTop");
totop.addEventListener("click", function () {
    scrollToTop();
});

function scrollToTop() {
    var position = document.body.scrollTop || document.documentElement.scrollTop;
    if (position) {
        window.scrollBy(0, -Math.max(10, Math.floor(position / 10)));
        scrollAnimation = setTimeout('scrollToTop()', 10);
    }
    else clearTimeout(scrollAnimation);
}		