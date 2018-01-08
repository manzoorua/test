<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MonitorMvc.Models.HomeViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Folder Browser</h2>
       
	<% foreach (var item in Model.Agents) { %>

		<span class="computer">
			<a href="<%= Url.Action("Browse", "Browse", new { agentId = item.AgentId }) %>">
				<img src="<%= Url.Content("~/Content/computer.png") %>" alt="computer" />
				<%= Html.Encode(item.ComputerName) %> - <%= Html.Encode(item.UserName) %>
			</a>
		</span>
		
	<% } %>	

</asp:Content>
