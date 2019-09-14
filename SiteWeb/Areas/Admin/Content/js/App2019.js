var Common = {
    //消息提示
    ShowBox: function (status, message, title) {
        switch (status) {
            case 1:
                toastr.success(message, title)
                break;
            case 2:
                toastr.error(message, title)
                break;
            case 3:
                toastr.info(message, title)
                break;
            case 4:
                toastr.warning(message, title)
        }
    },
    ShowBoxWithFunc: function (data, title, func) {

        switch (data.Status) {
            case 1:
                toastr.success(data.Message, title)
                func();
                break;
            case 2:
                toastr.error(data.Message, title)
                break;
            case 3:
                toastr.info(data.Message, title)
                break;
            case 4:
                toastr.warning(data.Message, title)
        }
    },
    ShowBoxWithFuncBack: function (data, title, func) {

        switch (data.Status) {
            case 1:
                toastr.success(data.Message, title)
                func(data.Id, data.Data);
                break;
            case 2:
                toastr.error(data.Message, title)
                break;
            case 3:
                toastr.info(data.Message, title)
                break;
            case 4:
                toastr.warning(data.Message, title)
        }
    },
    SubmitBack: function (data, title, container) {

        switch (data.Status) {
            case 1:
                toastr.success(data.Message, title)
                if (container !== undefined)
                    container.html(data.Data)
                break;
            case 2:
                toastr.error(data.Message, title)
                break;
            case 3:
                toastr.info(data.Message, title)
                break;
            case 4:
                toastr.warning(data.Message, title)
        }
    },
    PageSizeSet: function (url, title, pageSize, func) {   //分页设置

        $.post(url, { pageSize: pageSize }, function (data) {

            switch (data.Status) {
                case 1:
                    //toastr.success(data.Message, title);
                    func();
                    break;
                case 2:
                    toastr.error(data.Message, title);
                    break;
                case 3:
                    toastr.info(data.Message, title);
                    break;
                case 4:
                    toastr.warning(data.Message, title);
                    break;
            }
        });
    },

    SingleActionWithFunc: function (url, title, that, func) {   //真假值修改操作

        $.post(url, $("#anti-form").serialize(), function (data) {

            switch (data.Status) {
                case 1:
                    toastr.success(data.Message, title);
                    func(that);
                    break;
                case 2:
                    toastr.error(data.Message, title);
                    break;
                case 3:
                    toastr.info(data.Message, title);
                    break;
                case 4:
                    toastr.warning(data.Message, title);
                    break;
            }
        });
    },
    SingleActionWithFuncBack: function (url, title, that, func) {   //真假值修改操作

        $.post(url, $("#anti-form").serialize(), function (data) {

            switch (data.Status) {
                case 1:
                    toastr.success(data.Message, title);
                    func(that, data.Data);
                    break;
                case 2:
                    toastr.error(data.Message, title);
                    break;
                case 3:
                    toastr.info(data.Message, title);
                    break;
                case 4:
                    toastr.warning(data.Message, title);
                    break;
            }
        });
    },

    SingleAction: function (url, title, isTips) {   //真假值修改操作
        $.post(url, $("#anti-form").serialize(), function (data) {
            if (!isTips)
                return;

            switch (data.Status) {
                case 1:
                    toastr.success(data.Message, title);
                    setTimeout(function () {
                        location.reload();
                    }, 1000);
                    break;
                case 2:
                    toastr.error(data.Message, title);
                    break;
                case 3:
                    toastr.info(data.Message, title);
                    break;
                case 4:
                    toastr.warning(data.Message, title);
                    break;
            }
        });
    },



    DeleteItem: function (url, title, that) {   //删除

        $.post(url, $("#anti-form").serialize(), function (data) {
            switch (data.Status) {
                case 1:
                    toastr.success(data.Message, title);
                    that.closest('.item-container').remove();
                    break;
                case 2:
                    toastr.error(data.Message, title);
                    break;
                case 3:
                    toastr.info(data.Message, title);
                    break;
                case 4:
                    toastr.warning(data.Message, title);
                    break;
            }
        });
    }



};



var singleEelFinder = {
    percent: 70,
    baseUrl: "/tools/elfinder/default",
    baseForTingMCEUrl: "/tools/elfinder/ForTinyMCE4",
    selectActionFunction: null,
    elFinderCallback: function (fileUrl) {
        this.selectActionFunction(fileUrl);
    },
    open: function () {
        var w = 800,
            h = 600; // default sizes
        if (window.screen) {
            w = window.screen.width * this.percent / 100;
            h = window.screen.height * this.percent / 100;
        }
        var x = screen.width / 2 - w / 2;
        var y = screen.height / 2 - h / 2;

        window.open(this.baseUrl, "_blank", 'height=' + h + ',width=' + w + ',left=' + x + ',top=' + y);
    },
    elFinderBrowser: function (callback, value, meta) {

        tinyMCE.activeEditor.windowManager.openUrl({
            url: "/tools/elfinder/ForTinyMCE4",
            title: '文件管理器',
            width: 1100,
            height: 600

        });

        window.addEventListener('message', function (event) {
            var data = event.data;
            callback(data.content);

        });

        return false;
    },
    ImagesUploadHandler: function (blobInfo, success, failure) {
        var xhr, formData;

        xhr = new XMLHttpRequest();
        xhr.withCredentials = false;
        xhr.open('POST', '/Tools/TinyMCE/TinymceUpload');

        xhr.onload = function () {
            var json;

            if (xhr.status !== 200) {
                failure('HTTP Error: ' + xhr.status);
                return;
            }

            json = JSON.parse(xhr.responseText);

            if (!json || typeof json.location !== 'string') {
                failure('Invalid JSON: ' + xhr.responseText);
                return;
            }

            success(json.location);
        };


        var description = '';

        jQuery(tinymce.activeEditor.dom.getRoot()).find('img').not('.loaded-before').each(
            function () {
                description = $(this).attr("alt");
                $(this).addClass('loaded-before');
            });

        formData = new FormData();
        formData.append('file', blobInfo.blob(), blobInfo.filename());
        formData.append('description', description); //found now))

        xhr.send(formData);
    }
};


$(function () {
    var resetwidth = function () {       

        var winheight = $(window).height(), winwidth = $(window).width();

        var height = Math.max($("#sidebar-nav").height(), $("#rightcol").height(), winheight);
      //  $("#sidebar-nav").height(height);
      //  $("#rightcol").height(height);

        $('#rightcol,#sidebar-nav').css({ 'min-height': height + "px" });
        if (winwidth <= 768) {
            $('#rightcol').css({ 'width': winwidth + "px" });
        } else {
            $('#rightcol').css({ 'width': "auto" });
        }
    }

    resetwidth();

    $(window).resize(function () {
        resetwidth();
    });

    var pid = $('.mainmenu a.active').closest('li').attr("data-parent");
    $('.mainmenu li[data-parent=' + pid + ']').fadeIn();

 //查子项数量
    $.each($(".down-nav>a"), function (index, value) {
        var id = $(value).attr("data-id");
        var licount = $('.mainmenu li[data-parent=' + id + ']').length;
        var html = '<span class="badge">' + licount + '</span>';
        $(value).append(html);     
    });

    $(".down-nav>a").click(function (e) {
        e.preventDefault();

        var id = $(this).attr("data-id");       
        $('.mainmenu li[data-parent=' + id + ']').slideToggle();        

        //var n = $(this).next("ul");
        var li = $(this).closest('li');
        //n.slideToggle();

        li.toggleClass('nav-open');
        
    });


    $('.closemenu a').on('click', function (e) {

        //  $('#rightcol').css({ 'margin-left': '0' });
        closenav();
        e.preventDefault();
    });

    $('#openav').on('click', function (e) {

        var marginLeft = $('#rightcol').css("margin-left");
        // console.log($('#rightcol').css("margin-left"));

        if (marginLeft === '0px') {
            $('#rightcol').animate({ 'marginLeft': '170' }, 'fast');
            $('#wrapper').removeClass('nonav');

        } else {
            closenav();
        }

        e.preventDefault();
    });

    var closenav = function () {
        $('#rightcol').animate({ 'marginLeft': '0' }, 'fast');
        $('#wrapper').addClass("nonav");
    }



    $('a.expand').click(function (e) {
        $(this).closest('.box').addClass('box-fixed');
        $(this).hide();
        $(this).next('a').show();
        e.preventDefault();
    });
    $('a.compress').click(function (e) {
        $(this).closest('.box').removeClass('box-fixed');
        $(this).hide();
        $(this).prev('a').show();
        e.preventDefault();
    });
});
