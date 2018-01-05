﻿<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FileManager.ascx.cs" Inherits="SIG.WebUI.Plugins.SigFinder.FileManager" %>
 <div id="container-file">
     <input type="hidden" id="rootDir" value="<%=SIG.Infrastructure.Configs.SettingsManager.File.RootDirectory %>" />
        <div id="dircol">
            <nav class="navbar navbar-default navbar-static-top text-center">
                <button type="button" class="btn btn-default navbar-btn btn-sm" id="btnRoot"><span class="glyphicon glyphicon-home"></span></button>
<%--                <button type="button" class="btn btn-default navbar-btn  btn-sm"><span class="glyphicon glyphicon-plus"></span></button>
                <button type="button" class="btn btn-default navbar-btn  btn-sm"><span class="glyphicon glyphicon-trash"></span></button>--%>
            </nav>
            <aside id="dirbody">
                <ul class="list-unstyled" id="dirTree">
                </ul>

                <nav id="context-menu" class="context-menu">
        <ul class="context-menu__items">
            <li class="context-menu__item">
                <a href="#" class="context-menu__link" data-action="create"><i class="fa fa-plus fa-fw"></i>创建目录</a>
            </li>
            <li class="context-menu__item">
                <a href="#" class="context-menu__link" data-action="rename"><i class="fa fa-edit fa-fw"></i>重命名</a>
            </li>
            <li class="context-menu__item">
                <a href="#" class="context-menu__link" data-action="delete"><i class="fa fa-times fa-fw"></i>删除目录</a>
            </li>
        </ul>
    </nav>
            </aside>
        </div>
        <div id="filecol">
            <nav class="navbar navbar-default navbar-static-top text-center">

                <button type="button" class="btn btn-default navbar-btn btn-sm" id="btnUpload"><span class="glyphicon glyphicon-upload glyphicon-fw"></span>上传</button>
                <button type="button" class="btn btn-default navbar-btn btn-sm" id="btnRefresh" data-dir="<%=SIG.Infrastructure.Configs.SettingsManager.File.RootDirectory %>"><span class="glyphicon glyphicon-refresh glyphicon-fw"></span>刷新</button>
<%--                <button type="button" class="btn btn-default navbar-btn btn-sm"><span class="glyphicon glyphicon-trash"></span></button>
                <div class="btn-group  btn-group-sm navbar-btn" role="group">
                    <button type="button" class="btn btn-default dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                        设置
                    <span class="caret"></span>
                    </button>
                    <ul class="dropdown-menu">
                        <li><a href="#">按日期</a></li>
                        <li><a href="#">按大小</a></li>
                    </ul>
                </div>--%>
            </nav>

            <!-- Modal -->
            <div class="uploadFile" id="uploadFile">
                <header>
                    <button type="button" class="close" id="btnClose"><span aria-hidden="true">&times;</span></button>
                </header>

                <div id="uploader" class="wu-example">
                    <div class="btns" style="display: inline-block">
                        <div id="picker">选择文件</div>
                    </div>
                    <%--     <button id="ctlBtn" type="button" class="btn btn-default"><span class="glyphicon glyphicon-upload"></span> 开始上传</button>        --%>
                    <!--用来存放文件信息-->
                    <div id="thelist" class="uploader-list"></div>

                </div>


            </div>

            <div id="fileList">
            </div>

        </div>
    </div>
