var SIG = (function () {
    var instance;

    //获取根目录列表
    function getDirectories(url) {

        $.getJSON(url, function (result) {          

            var dirs = '';
            $.each(result, function (i, item) {
           //     console.log(item);
                var isHidden = item.HasChildren ? "" : "hidden";
                dirs += '<li><div class="exp"><a href="#" class="btnDir" data-loaded="0" data-path="' + item.DirPath.toLowerCase() + '" ' + isHidden + '><span class="glyphicon glyphicon-plus glyphicon-fw"></span></a></div><a href="#" class="btnFile" data-path="' + item.DirPath.toLowerCase() + '"><span class="glyphicon glyphicon-folder-close glyphicon-fw"></span>' + item.Name + '</a></li>';
            });

            $("#dirTree").html(dirs);
           
        });
    }

    //获取子目录列表
    function getSubDirectories(url, li) {
        li.find("ul").remove();
        //console.log(li);
        $.getJSON(url, function (result) {
           
            var item = "";
            $.each(result, function (key, val) {
                // debugger;
                var isHidden = val.HasChildren ? "" : "hidden";
                item += '<li>' +
                            '<div class="exp"><a href="#" class="btnDir" data-loaded="0" data-path="' + val.DirPath.toLowerCase() + '" ' + isHidden + '>' +
                                '<span class="glyphicon glyphicon-plus glyphicon-fw"></span>' +
                            '</a></div>' +
                            '<a href="#" class="btnFile" data-path="' + val.DirPath.toLowerCase() + '"><span class="glyphicon glyphicon-folder-close glyphicon-fw"></span>' + val.Name + '</a>' +
                        '</li>';

            });

            $('<ul/>', { html: item }).addClass("subTree").appendTo(li);

            li.children(".exp").find("a").attr("data-loaded", "1").find("span").removeClass("glyphicon-plus").addClass("glyphicon-minus");
          
        });
    }
    
   

    //获取文件列表
    function getFiles(url) {
        $.getJSON(url, function (result) {         
            loadFiles(result);           
        });
    }
    

    function loadFiles(result) {
        $('#fileList').empty(); // Clear the table body.

        $.each(result, function (key, val) {
            // debugger;
            var item = '<div class="col-sm-2 itembox">' +
                 '<div class="panel panel-default item" data-file="' + val.FilePath + '" data-name="' + val.Name + '">' +
                        '<div class="panel-body filebox text-center">' +
                        '<img src="' + val.ImgUrl + '" class="img-responsive" />' +
                        '</div>' +
                        '<div class="panel-footer">' +
                        '<div class="boxfooter"><span>' + val.Name + '</span><br/>' +
                        '<small>' + val.CreatedDate + '</small><br /> ' +
                        '<div>' +
                        '<div class="btn-group btn-group-xs pull-right" role="group">' +
                        '<button type="button" class="btn btn-default rename" title="重命名"><span class="glyphicon glyphicon-pencil"></span></button>' +
                        '<button type="button" class="btn btn-default download" title="下载"><span class="glyphicon glyphicon-download"></span></button>' +
                        '<button type="button" class="btn btn-default btnDelete" title="删除"><span class="glyphicon glyphicon-trash"></span></button>' +
                        '</div>' + val.FileSize + 'KB</div>' +
                        '</div>' +
                        '</div>' +
                         '</div>' +
                        '</div>';
            $(item).appendTo($('#fileList'));
        });
    };


    //打开当前的路径
    function loadCurrentURL(url) {
        url = url.toLowerCase();
        var baseUrl = "/Uploads/";

        if (url.startsWith(baseUrl.toLowerCase())) {
            var dir = url.split("/");
            var index = url.indexOf(dir[dir.length - 1]) - 1;

            var subStr = url.substring(9, index);
            var subDir = subStr.split("/");
            var goDir = baseUrl;
            for (var i = 0; i < subDir.length; i++) {
                goDir = goDir + subDir[i];
                goDir = goDir.toLowerCase();

                var li = $("a[data-path='" + goDir + "']").eq(0).closest("li");

                if (i < (subDir.length - 1)) {
                    //  debugger;
                    var urlDir = "/api/filemanager/GetSubDirectories?dir=" + goDir;
                    SIG.getInstance().getSubDirectories(urlDir, li);
                    goDir = goDir + "/";
                } else {
              
                    var urlDir = "/api/filemanager/GetSubFiles?dir=" + goDir;
                  
                    SIG.getInstance().getFiles(urlDir);
                    $("#btnRefresh").attr("data-dir", goDir);

                    setTimeout(function () {
                        $("[data-path='" + goDir + "']").eq(1).addClass("active").children("span").removeClass("glyphicon-folder-close").addClass("glyphicon-folder-open");
                        $("#fileList div[data-file='" + url + "']").addClass("active");
                    }, 1000);

                }


            }
            //alert(url.substring(9, index));
        }

    }

    function renameFile(oldpath, newpath, item) {
        
        $.ajax({
            type: "POST",
            contentType: "application/json",
            url: "/api/filemanager/RenameFile?filePath=" + oldpath + "&newFilePath=" + newpath,
            //  data: JSON.stringify({filePath: filePath }),
            dataType: 'json',
            success: function (result) {
                if (result === "OK") {
                    // container.remove()
                    toastr.success("已重名文件", "重命名")
                    item.attr("data-file", newpath);
                   
                    item.find(".boxfooter").children("span").text(newpath.split('/').pop());

                } else if (result === "NO") {
                    toastr.warning("此文件名已经存在！", "重命名")
                } else {
                    toastr.error(result, "重命名")
                }

            }
        });
    }

    function Initialize() {
        var url = "/api/filemanager/RootDirectories"
        getDirectories(url);

        var url2 = "/api/filemanager/RootDirFiles"
        getFiles(url2);
        $("#btnRefresh").attr("data-dir",$("#rootDir").val());
    }

    function test() {
        alert("aaaaa");
    }

    function createInstance() {
        return {
            Initialize:Initialize,
            getFiles: getFiles,
            getSubDirectories: getSubDirectories,
            getDirectories: getDirectories,
            renameFile: renameFile,
            loadCurrentURL :loadCurrentURL,
            test: test
        }
    }
    return {
        getInstance: function () {
            return instance || (instance = createInstance());
        }
    }

}());


//右键菜单
(function () {

    "use strict";

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //
    // H E L P E R    F U N C T I O N S
    //
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    /**
     * Function to check if we clicked inside an element with a particular class
     * name.
     * 
     * @param {Object} e The event
     * @param {String} className The class name to check against
     * @return {Boolean}
     */
    function clickInsideElement(e, className) {
        var el = e.srcElement || e.target;

        if (el.classList.contains(className)) {
            return el;
        } else {
            while (el = el.parentNode) {
                if (el.classList && el.classList.contains(className)) {
                    return el;
                }
            }
        }

        return false;
    }

    /**
     * Get's exact position of event.
     * 
     * @param {Object} e The event passed in
     * @return {Object} Returns the x and y position
     */
    function getPosition(e) {
        var posx = 0;
        var posy = 0;

        if (!e) var e = window.event;

        if (e.pageX || e.pageY) {
            posx = e.pageX;
            posy = e.pageY;
        } else if (e.clientX || e.clientY) {
            posx = e.clientX + document.body.scrollLeft + document.documentElement.scrollLeft;
            posy = e.clientY + document.body.scrollTop + document.documentElement.scrollTop;
        }

        return {
            x: posx,
            y: posy
        }
    }

    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////
    //
    // C O R E    F U N C T I O N S
    //
    //////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////////////////////////////////////////

    /**
     * Variables.
     */
    var contextMenuClassName = "context-menu";
    var contextMenuItemClassName = "context-menu__item";
    var contextMenuLinkClassName = "context-menu__link";
    var contextMenuActive = "context-menu--active";

    var taskItemClassName = "btnFile";
    var taskItemInContext;

    var clickCoords;
    var clickCoordsX;
    var clickCoordsY;

    var menu = document.querySelector("#context-menu");
    var menuItems = menu.querySelectorAll(".context-menu__item");
    var menuState = 0;
    var menuWidth;
    var menuHeight;
    var menuPosition;
    var menuPositionX;
    var menuPositionY;

    var windowWidth;
    var windowHeight;

    /**
     * Initialise our application's code.
     */
    function init() {
        contextListener();
        clickListener();
        keyupListener();
        resizeListener();
    }

    /**
     * Listens for contextmenu events.
     */
    function contextListener() {
        document.addEventListener("contextmenu", function (e) {
            taskItemInContext = clickInsideElement(e, taskItemClassName);

            if (taskItemInContext) {
                e.preventDefault();
                toggleMenuOn();
                positionMenu(e);
            } else {
                taskItemInContext = null;
                toggleMenuOff();
            }
        });
    }

    /**
     * Listens for click events.
     */
    function clickListener() {
        document.addEventListener("click", function (e) {
            var clickeElIsLink = clickInsideElement(e, contextMenuLinkClassName);

            if (clickeElIsLink) {
                e.preventDefault();
                menuItemListener(clickeElIsLink);
            } else {
                var button = e.which || e.button;
                if (button === 1) {
                    toggleMenuOff();
                }
            }
        });
    }

    /**
     * Listens for keyup events.
     */
    function keyupListener() {
        window.onkeyup = function (e) {
            if (e.keyCode === 27) {
                toggleMenuOff();
            }
        }
    }

    /**
     * Window resize event listener
     */
    function resizeListener() {
        window.onresize = function (e) {
            toggleMenuOff();
        };
    }

    /**
     * Turns the custom context menu on.
     */
    function toggleMenuOn() {
        if (menuState !== 1) {
            menuState = 1;
            menu.classList.add(contextMenuActive);
        }
    }

    /**
     * Turns the custom context menu off.
     */
    function toggleMenuOff() {
        if (menuState !== 0) {
            menuState = 0;
            menu.classList.remove(contextMenuActive);
        }
    }

    /**
     * Positions the menu properly.
     * 
     * @param {Object} e The event
     */
    function positionMenu(e) {
        clickCoords = getPosition(e);
        clickCoordsX = clickCoords.x;
        clickCoordsY = clickCoords.y;

        menuWidth = menu.offsetWidth + 4;
        menuHeight = menu.offsetHeight + 4;

        windowWidth = window.innerWidth;
        windowHeight = window.innerHeight;

        var parentOffset = $("#dirbody").offset();

        if ((windowWidth - clickCoordsX) < menuWidth) {
            menu.style.left = windowWidth - menuWidth - parentOffset.left + "px";
        } else {
            menu.style.left = clickCoordsX - parentOffset.left + "px";
        }

        if ((windowHeight - clickCoordsY) < menuHeight) {
            menu.style.top = windowHeight - menuHeight - parentOffset.top + "px";
        } else {
            menu.style.top = clickCoordsY - parentOffset.top + "px";
        }
    }

    /**
     * Dummy action function that logs an action when a menu item link is clicked
     * 
     * @param {HTMLElement} link The link that was clicked
     */
    function menuItemListener(link) {
      //  console.log("Task ID - " + taskItemInContext.getAttribute("data-path") + ", Task action - " + link.getAttribute("data-action"));

        var filePath = taskItemInContext.getAttribute("data-path");
        var action = link.getAttribute("data-action");
        switch (action) {
            case "create":
                var newName = prompt("创建子目录", ""); 
                if (newName.length > 0) {                
                    $.ajax({
                        type: "POST",
                        contentType: "application/json",
                        url: "/api/filemanager/CreateDir?filePath=" + filePath + "&dir=" + newName,
                        //  data: JSON.stringify({filePath: filePath }),
                        dataType: 'json',
                        success: function (result) {
                            if (result === "OK") {                                
                                //    toastr.success("创建新目录", "创建目录")
                                var li = taskItemInContext.closest("li");
                              
                                var urlDir = "/api/filemanager/GetSubDirectories?dir=" + filePath;
                                SIG.getInstance().getSubDirectories(urlDir, $(li));
                            } else if (result === "NO") {
                                toastr.warning("此目录已存在！", "创建目录")
                            }
                            else {
                                toastr.error(result, "创建目录")
                            }

                        }
                    }); 
                }               

                break;
            case "delete":

                $.ajax({
                    type: "POST",
                    contentType: "application/json",
                    url: "/api/filemanager/DeleteDir?filePath=" + filePath,
                    //  data: JSON.stringify({filePath: filePath }),
                    dataType: 'json',
                    success: function (result) {
                        if (result === "OK") {                        
                            var li = taskItemInContext.closest("li"), parentLi = $(li).closest("ul").closest("li"),
                            parentPath = $(li).closest("ul").prevAll("a:first").attr("data-path");

                        //    console.log(parentLi);

                            var urlDir = "/api/filemanager/GetSubDirectories?dir=" + parentPath
                            SIG.getInstance().getSubDirectories(urlDir, parentLi);

                        } else if (result === "NO") {
                            toastr.warning("此目录还有文件存在！", "删除目录")
                        }
                        else {
                            toastr.error(result, "删除目录")
                        }

                    }
                });

                break;
            case "rename":
             
                var dirName = filePath.split('/').pop();

                var newName = prompt("重命名", dirName);
                if (newName!=null) {
                    var index = filePath.length - dirName.length;
                    var newPath = filePath.substr(0,index) + newName;
                    debugger;
                  
                    $.ajax({
                        type: "POST",
                        contentType: "application/json",
                        url: "/api/filemanager/RenameDir?filePath=" + filePath + "&newFilePath=" + newPath,
                        //  data: JSON.stringify({filePath: filePath }),
                        dataType: 'json',
                        success: function (result) {
                            if (result === "OK") {
                                var li = taskItemInContext.closest("li"), parentLi = $(li).closest("ul").closest("li"),
                                parentPath = $(li).closest("ul").prevAll("a:first").attr("data-path");

                                var urlDir = "/api/filemanager/GetSubDirectories?dir=" + parentPath
                                SIG.getInstance().getSubDirectories(urlDir, parentLi);

                            } else if (result === "NO") {
                                toastr.warning("此目录名已经存在！", "重命名目录")
                            }
                            else {
                                toastr.error(result, "重命名目录")
                            }

                        }
                    });
                   // SIG.getInstance().renameFile(filePath, newPath, download);
                   
                }


                break;
        }

        toggleMenuOff();
    }

    /**
     * Run the app.
     */
    init();

})();


//page js
function closeUploader() {
    $("#thelist").html("");
    $("#picker").text("选择文件")
    $("#uploadFile").removeClass("show").animate({ top: "-100px" }, 600);
}

$(function () {

    $("#btnUpload").on("click", function () {
        
        if ($("#uploadFile").hasClass("show")) return;
       

        $("#uploadFile").animate({ top: "50px" }, 600).addClass("show");
        var filePath = $("#dirTree a.active").attr("data-path");       
        var serverUrl = (filePath !== undefined) ? filePath : "";
        var $list = $('#thelist'), $btn = $('#ctlBtn'), state = 'pending', uploader = WebUploader.create({
            // 不压缩image  
            resize: false,
            auto: true,
            // swf文件路径  
            swf: 'http://localhost/plugins/webuploader/Uploader.swf',
            // 文件接收服务端。  
            // server: "/plugins/webuploader/fileupload.ashx",
            server: "/bbi-admin/file/UpLoadProcess",
            
            formData: { path: serverUrl },
            // 选择文件的按钮。可选。  
            // 内部根据当前运行是创建，可能是input元素，也可能是flash.  
            pick: '#picker',
            compress: null
        });

        // 当有文件添加进来的时候  
        uploader.on('fileQueued', function (file) {
            $list.append('<div id="' + file.id + '" class="item">' +
                '<h4 class="info">' + file.name + '</h4>' +
                '<p class="state">等待上传...</p>' +
            '</div>');
        });

        // 文件上传过程中创建进度条实时显示。  
        uploader.on('uploadProgress', function (file, percentage) {
            var $li = $('#' + file.id),
                $percent = $li.find('.progress .progress-bar');

            // 避免重复创建  
            if (!$percent.length) {
                $percent = $('<div class="progress progress-striped active">' +
                  '<div class="progress-bar" role="progressbar" style="width: 0%">' +
                  '</div>' +
                '</div>').appendTo($li).find('.progress-bar');
            }

            $li.find('p.state').text('上传中');

            $percent.css('width', percentage * 100 + '%');
        });

        uploader.on('uploadSuccess', function (file) {
            $('#' + file.id).find('p.state').text('已上传');
            closeUploader();

            if (filePath !== undefined) {
                var url = "/api/filemanager/GetSubFiles?dir=" + filePath;

                SIG.getInstance().getFiles(url);
                // $("#btnRefresh").attr("data-dir", dir);
            } else {
                //载入初始目录
                SIG.getInstance().Initialize();
            }



        });

        uploader.on('uploadError', function (file) {
            $('#' + file.id).find('p.state').text('上传出错');
        });

        uploader.on('uploadComplete', function (file) {
            $('#' + file.id).find('.progress').fadeOut();
        });

        uploader.on('all', function (type) {
            if (type === 'startUpload') {
                state = 'uploading';
            } else if (type === 'stopUpload') {
                state = 'paused';
            } else if (type === 'uploadFinished') {
                state = 'done';
            }

            //if (state === 'uploading') {
            //    $btn.text('暂停上传');
            //} else {
            //    $btn.text('开始上传');
            //}
        });

        //$btn.on('click', function () {
        //    if (state === 'uploading') {
        //        uploader.stop();
        //    } else {
        //        uploader.upload();
        //    }
        //});


    })
    $("#btnClose").on("click", function () {
        closeUploader();
    })



    //载入初始目录
    SIG.getInstance().Initialize();


    $("body").delegate("#btnRoot", "click", function (e) {
        e.preventDefault();

        SIG.getInstance().Initialize();

    });

    $("body").delegate("a.btnDir", "click", function (e) {
        //$(".btnDir").on("click", function (e) {

        e.preventDefault();
        var dir = $(this).attr("data-path");

        var isLoaded = $(this).attr("data-loaded")
        if (isLoaded === "0") {
            var li = $(this).closest("li"),
            urlDir = "/api/filemanager/GetSubDirectories?dir=" + dir;
            SIG.getInstance().getSubDirectories(urlDir, li);

        } else {

            $(this).closest("li").find("ul").remove();
            $(this).children("span").removeClass("glyphicon-minus").addClass("glyphicon-plus");
            $(this).attr("data-loaded", "0");
        }


    });

    $("body").delegate("a.btnFile", "click", function (e) {

        //  $(".btnFile").on("click", function (e) {
        e.preventDefault();

        $("#dirTree a.active").removeClass("active").children("span").removeClass("glyphicon-folder-open").addClass("glyphicon-folder-close");
        $(this).addClass("active");
        var dir = $(this).attr("data-path"),
            url = "/api/filemanager/GetSubFiles?dir=" + dir;

        $(this).children("span").removeClass("glyphicon-folder-close").addClass("glyphicon-folder-open");

        SIG.getInstance().getFiles(url);
        $("#btnRefresh").attr("data-dir", dir);

    });

    $("body").delegate("#btnRefresh", "click", function (e) {

        //  $(".btnFile").on("click", function (e) {
        e.preventDefault();
        var dir = $(this).attr("data-dir"),
            url = "/api/filemanager/GetSubFiles?dir=" + dir;

        SIG.getInstance().getFiles(url);

    });

    //选择文件
    $("body").delegate("div.item", "click", function (e) {

        //  $(".btnFile").on("click", function (e) {
        e.preventDefault();

        if ($(this).hasClass("active")) {
            $(this).removeClass("active");
        } else {
           // $("#fileList .item.active").removeClass("active");
            $(this).addClass("active");
        }

    });


    $("body").delegate(".rename", "click", function (e) {
        e.preventDefault();
        var download = $(this).closest(".item");
        var filePath = download.attr("data-file");

        var filename = filePath.split('/').pop();

        var newName = prompt("重命名", filename);
        if (newName!=null) {

            var index = filePath.length - filename.length;
            var newPath = filePath.substr(0, index) + newName;
            //   var newPath = filePath.replace(filename, newName);
            var oldext = filePath.split('.').pop().toLowerCase();
            var newext = newName.split('.').pop().toLowerCase();

            if (oldext === newext) {
                SIG.getInstance().renameFile(filePath, newPath, download);

            } else {
                if (confirm("改变文件后缀名，可能导致文件不可用，是否要修改？")) {
                    SIG.getInstance().renameFile(filePath, newPath, download);
                }

            }

        }


    });


    $("body").delegate(".download", "click", function (e) {
        e.preventDefault();
        var download = $(this).closest(".item");
        var filePath = download.attr("data-file");
        // debugger;
        location.href = "/api/filemanager/Download?filePath=" + filePath;

    });

    $("body").delegate(".btnDelete", "click", function (e) {
        e.preventDefault();
        var download = $(this).closest(".item");
        var filePath = download.attr("data-file");
        var container = $(this).closest(".itembox");
        // debugger;
        $.ajax({
            type: "POST",
            contentType: "application/json",
            url: "/api/filemanager/DeleteFile?filePath=" + filePath,
            //  data: JSON.stringify({filePath: filePath }),
            dataType: 'json',
            success: function (result) {
                if (result === "OK") {
                    container.remove()
                    toastr.success("已删除文件", "删除文件")
                } else {
                    toastr.error(result, "删除文件")
                }

            }
        });


    });

})