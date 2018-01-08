<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Rlc.Monitor.AgentData>" %>

<%	if (Model.FileDataMessage != null)
	{	%>
		
		<%= Html.ActionLink("Download", "DownloadPackage", new {agentId = Model.AgentId}) %>

<%  } %>