
/*
Plugin Name: bbiSlider 1.0.0
Author: Wuya
Homepage: http://heiniaozhi.cn
Github: https://github.com/doubletong/jQuerypulgins
*/
(function ($) {

    //index 为默认索引
    $.fn.bbiSlider = function (opts) {
        var defaults = {
            delay: 3000, // 当前索引
            prevNext: true,
            dotNav: true
        };
        var options = $.extend(defaults, opts);


        var lis = $(this).find('ul.slider-list li');
        //初始化
        slider.initialize($(this), lis);


        $(window).resize(function () {
            // slider.initialize($(this), lis);
            var winwidth = $(this).width();
            var ulwidth = winwidth * lis.length
            var ul = $(this).find('ul.slider-list').width(ulwidth);
            lis.css({ 'width': winwidth + 'px' });

        });

        var links = $(this).find('div.slider-nav a');

        //循环背景
        var interval = setInterval(function () {
            slider.slideSwitch(lis, links);
        }, options.delay);


        if (options.dotNav) {
            // 幻片导航
            links.click(function (e) {
                e.preventDefault();
                clearInterval(interval); // stop the interval

                var linkIndex = $(this).index();
                slider.chang(linkIndex, lis, links); //切换图片

                interval = setInterval(function () {
                    slider.slideSwitch(lis, links);
                }, options.delay);

            });
        } else {
            links.hide();
        }

        if (options.prevNext) {
            //左右导航
            $(this).children('a.prev,a.next').click(function (e) {

                e.preventDefault();
                clearInterval(interval); // stop the interval

                var dir = $(this).hasClass('prev') ? -1 : 1;
                // Get the li that is currently visible
                var current = links.filter('a.active');

                var currentIndex = links.index(current);
                // Get the element that should be shown next according to direction
                var newIndex = dir < 0 ? (currentIndex - 1) : (currentIndex + 1);

                // If we've reached the end, select first/last depending on direction
                if (newIndex < 0 || newIndex > (links.length - 1)) {
                    newIndex = dir < 0 ? (links.length - 1) : 0;
                }

                slider.chang(newIndex, lis, links);


                interval = setInterval(function () {
                    slider.slideSwitch(lis, links);
                }, options.delay);

            });
        } else {
            $(this).children('a.prev,a.next').hide();
        }



        // var pageWidth = $(window).width(), pageHeight = $(window).height();

        //图片缓冲      
        lis.each(function () {
            var $currli = $(this);

            $currli.append("<div class='loading'></div>");
            var imgurl = $(this).attr("data-pc-img");
            var imgalt = $(this).attr("data-title");

            var image = new Image();
            $(image).attr({'src': imgurl,'alt':imgalt}).load(function () {

                $(this).remove();
                $currli.find('div.loading').remove();
                $currli.css("background-image", 'url(' + imgurl + ')');

            });

        });


    }


    var slider = {
        initialize: function (slider, lis) { //initialize slider
            var winwidth = slider.width();
            var ulwidth = winwidth * lis.length
            var ul = slider.find('ul.slider-list').width(ulwidth);
            lis.css({ 'width': winwidth + 'px' });

            var prevNext = '<a href="#" class="prev"><span class="icon-angle-left"></span></a><a href="#" class="next"><span class="icon-angle-right"></span></a>';
            slider.append(prevNext);

            var navs = "";
            for (var i = 0; i < lis.length; i++) {
                navs += '<a href="#"></a>';
            }
            slider.append('<div class="slider-nav">' + navs + '</div>');
            var links = $('.slider-nav a');
            this.chang(0, lis, links);
        },
        chang: function (index, lis, links) {  //set slider active
            var activeli = lis.eq(index);
            activeli.closest('ul').animate({ 'marginLeft': -index * activeli.width() + 'px' }, 500);
            var imgs = activeli.find('img');
            imgs.each(function () {
                var that = $(this);
                setTimeout(function () {
                    that.removeClass(that.attr('data-out')).addClass(that.attr('data-in')).fadeIn(1000);
                }, 500);


                setTimeout(function () {
                    that.removeClass(that.attr('data-in')).addClass(that.attr('data-out')).fadeOut(1000);
                }, 4000);
            });

            //    activeli.siblings().fadeOut(1000);
            //	activeli.siblings().find('img').removeClass('animated').hide();
            //	activeli.siblings().find('img')

            links.eq(index).addClass("active").siblings().removeClass("active");
        },

        slideSwitch: function (lis, links) { //loop display slider
            var current = links.filter(".active");
            var currentIndex = links.index(current);
            var nextIndex = (currentIndex == (links.length - 1)) ? 0 : currentIndex + 1

            this.chang(nextIndex, lis, links);

        }
    }


})(jQuery);