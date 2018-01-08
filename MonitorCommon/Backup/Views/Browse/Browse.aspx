<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<Rlc.Monitor.AgentData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Browse
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Browse <%=Html.Encode(Model.ComputerName) %> - <%= Html.Encode(Model.UserName) %></h2>
    
    <input id="agentId" type="hidden" value="<%= Html.Encode(Model.AgentId) %>" />
    
	<div id="curPath">loading....</div>
	
	<div id="info">&nbsp;</div>
	
	<a href="#" onclick="fetchFiles()">Request</a>&nbsp;<span id="package"></span>
	
	<script language="javascript" type="text/javascript">
		$(document).ready(function() {
			setInterval("checkForUpdates()", 1000);
		});

		var id = $("#agentId").val();
		var currentDirectory = "";
		function checkForUpdates() {
			var url = "/Monitor/Browse/GetCurrentDirectory?agentId=" + id;
			$.get(url, null, function(data) {
				if (data != "" && data != currentDirectory) {
					currentDirectory = data;

					var url = "/Monitor/Browse/GetDirectoryNav?agentId=" + id;
					$("#curPath").load(url);

					// Need to fetch directory information
					//
					var url = "/Monitor/Browse/GetDirectoryInfo?agentId=" + id;
					$("#info").load(url);
				}
			});
			var url = "/Monitor/Browse/GetPackageList?agentId=" + id;
			$("#package").load(url);
		};

		function changeDirectory(folder) {
			var url = "/Monitor/Browse/ChangeDirectory?agentId=" + id + "&folder=" + folder;
			$.get(url, null, null);
		}

		function fetchFiles() {
			var vals = "";
			$("input[type=checkbox]:checked").each(function() {
				vals += $(this).attr("name") + ";";
			});
			var url = "/Monitor/Browse/RequestFiles?agentId=" + id + "&files=" + vals;

			$.post(url);
		}
	</script>
</asp:Content>

