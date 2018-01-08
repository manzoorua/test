<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Rlc.Monitor.AgentData>" %>

<div class="data">
	<span class="info"><%= Html.Encode(Model.UserName) %></span>
	<span class="info"><%= Html.Encode(Model.ComputerName)%></span>
	<span class="info"><%= Html.Encode(Model.AgentId)%></span>
	<span class="info<%= Model.LastUpdate<DateTime.Now.AddMinutes(-1.0)?" offline":"" %>"><%= Html.Encode(Model.LastUpdate.ToString())%></span>
</div>