require(["jquery", "jquery.bbiSlider"], function ($) {

    $(function () {
        $(document).ready(function () {
            //轮播图
            $("#slider").bbiSlider({
                delay: 5000
            });

            $("#aboutslider").bbiSlider({
                delay: 5000
            });
            //right menu show ===================================================================
            $("#openav").click(function (e) {

                $("#pusher").fadeIn();
                $("#rightnav").animate({ 'right': '0px' });
                e.preventDefault();
            });

            $("#pusher").click(function () {
                $(this).fadeOut();
                $("#rightnav").animate({ 'right': '-150px' });
            });

            //share
            $("#share").click(function (e) {
                e.preventDefault();
                $("#overing").fadeIn();
            });
            
            $("#btnClose").click(function () {
                $("#overing").fadeOut();
            });

            //当前菜单
            var path = window.location.pathname;
            path = path.replace(/\/$/, "");
            path = decodeURIComponent(path);
            if(path.length>0){
                $('.mainmenu li a').each(function () {
                    var href = $(this).attr('href');
                    if (href.indexOf(path) != -1) {
                        $(this).closest('li').addClass('active');
                    }
                });
            }

            // header fixed nav
            var offset = 0;
           // var duration = 500;
            $(window).scroll(function () {
                if ($(this).scrollTop() > offset) {
                    $('.page-header').addClass("fixed-header");
                } else {
                    $('.page-header').removeClass("fixed-header");
                }
            });
            /*var pathname = (window.location.pathname.match(/[^\/]+$/)[0]);
                $('.mainmenu li a').each(function() {
                    if ($(this).attr('href') == pathname)
                    {
                        $(this).closest("li").addClass('active');
                    }
                });*/

            $.ajax({
                url : '/cart_post.php',
                type : 'POST',
                data : {type:"get"},
                success : function(res) {
                    $("#totalItems").text(res);
                  //  console.log(res);
                }
            });


        })
    });

});
require(["jquery", "mediaelementjs"], function ($) {

    $(function () {
        $(document).ready(function () {
          //  alert("test");
            $('audio,video').mediaelementplayer({
                //mode: 'shim',
                success: function(player, node) {
                    $('#' + node.id + '-mode').html('mode: ' + player.pluginType);
                }
            });
        })
    });
});

require(["jquery","notifier"], function ($) {
    $(function () {
        $(document).ready(function () {

            $('#addToCart').click(function(e){
                //alert("test");
                $that = $(this);
                var productId = $that.attr("data-id");

                $.ajax({
                    url : '/cart_post.php',
                    type : 'POST',
                    data : {id:productId,type:"add"},
                    success : function(res) {

                        if (res) {
                            Notifier.success('宝贝已成功移入购物车。');
                            $("#totalItems").text(res);
                        } else {
                            Notifier.error('操作失败，请再试一次。')
                          //  toastr.error('轮播图删除失败！', '删除轮播图');
                        }
                    }
                });
                e.preventDefault();
            });
        })
    });

});

require(["jquery","jquery.validate"], function ($) {
    $(function () {
        $(document).ready(function () {

            //输入数量
            $("#tbList input.qty").bind("keyup change",function () {
                var quantity = $(this).val();
                if (isNaN(parseInt(quantity))) {
                    quantity = 1;
                }
                if (quantity < 1) {
                    quantity = 1;
                }
                $(this).val(quantity);
                var price =  $(this).attr("data-price");

                var amount = price*quantity;
                $(this).closest("td").next().find(".amount").text(amount.toFixed(2));
            });

            //焦点消失时触发事件
            $("#tbList input.qty").blur(function(){
                var productId = $(this).attr("data-id");
                var qty = $(this).val();
                //提交更新
                $.ajax({
                    url : '/cart_post.php',
                    type : 'POST',
                    data : {id:productId,qty:qty,type:"update"},
                    success : function(res) {
                        if (res) {
                            var result = $.parseJSON(res);
                            $("#total_qty,#totalItems").text(result.total_items);
                            $("#total_amount").text(result.total_price.toFixed(2));

                        } else {
                            Notifier.error('修改失败，请再试一次。');
                        }
                    }
                });

            });

            $("#tbList button.remove").click(function(e){
                var productId = $(this).attr("data-id");
                var $that = $(this);
                //移除商品
                $.ajax({
                    url : '/cart_post.php',
                    type : 'POST',
                    data : {id:productId,type:"delete"},
                    success : function(res) {
                        if (res) {
                            var result = $.parseJSON(res);
                            $("#total_qty,#totalItems").text(result.total_items);
                            $("#total_amount").text(result.total_price.toFixed(2));
                            $that.closest("tr").remove();
                            Notifier.info('宝贝已成功移出购物车。');
                        } else {
                            Notifier.error('移除失败，请再试一次。');
                        }
                    }
                });

            });



            $("form#shopping").validate({
                rules: {
                    fullname: {
                        required: true
                    },
                    phone: {
                        required: true
                    },
                    address: {
                        required: true
                    }

                },
                messages:{
                    fullname: {
                        required:"请输入收件人姓名"
                    },
                    phone: {
                        required: "请输入联系电话"
                    },
                    address: {
                        required: "请输入收货地址"
                    }

                },

                errorClass: "help-block",
                errorElement: "span",
                highlight: function (element, errorClass, validClass) {
                    $(element).parents('.form-group').removeClass('has-success');
                    $(element).parents('.form-group').addClass(' has-error');
                },
                unhighlight: function (element, errorClass, validClass) {
                    $(element).parents('.form-group').removeClass(' has-error');
                    $(element).parents('.form-group').addClass('has-success');
                }
            });

        })
    });

});


require(["jquery", "jquery.appear"], function ($) {

    $(function () {
        $(document).ready(function () {


            $('#lasterProducts .box').appear();

            $(document.body).on('appear', '#lasterProducts .box', function (e, $affected) {
                //  console.log($(this).html());
                var imgs = $(this).find(".opacity0");
                imgs.each(function () {
                    var that = $(this);
                //    alert(that);
                    that.removeClass("opacity0").removeClass('animated').removeClass(that.attr('data-out'))
                        .addClass("opacity100 animated " + that.attr('data-in'));
                });

            });

         /*   $(document.body).on('disappear', '#lasterProducts .box', function (e, $affected) {
                var imgs = $(this).find(".animated");
                imgs.each(function () {
                    var that = $(this);
                    that.removeClass(that.attr('data-in')).addClass(that.attr('data-out')).fadeOut(1000);
                });
            });*/


        })
    });

});

define('modernizr', [], Modernizr);
require(['modernizr'], function (Modernizr) {
   
    //    alert(Modernizr.svg);
    
});
