<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FMPopu.ascx.cs" Inherits="SIG.WebUI.Plugins.SigFinder.FMPopu" %>
<%@ Register Src="~/Plugins/SigFinder/FileManager.ascx" TagPrefix="uc1" TagName="FileManager" %>

<!-- FileManager modal -->
<div class="modal fade bs-example-modal-lg" tabindex="-1" role="dialog" aria-labelledby="myLargeModalLabel" id="fileManagerModal">
  <div class="modal-dialog modal-lg" role="document">
    <div class="modal-content">
        <uc1:FileManager runat="server" ID="FileManager" />
        <div class="modal-footer" style="border:none;">
             <button type="button" class="btn btn-primary" id="selectImage">确定</button>
        <button type="button" class="btn btn-default" data-dismiss="modal">取消</button>
       
      </div>
    </div>
       
  </div>
</div>