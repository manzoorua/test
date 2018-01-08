<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Rlc.Monitor.AgentData>" %>


<ul class="directory">

   <%	foreach (string folder in Model.Folders) 
		{
			string name = System.IO.Path.GetFileName(folder);	%>
			
			
			<li class="folder"><input type="checkbox" name="<%= folder.Replace('\\', '/') %>" /><a href="#" onclick="changeDirectory('<%= folder.Replace('\\', '/') %>');"><%= name %></a></li>
			
   <%	}
		foreach (string file in Model.Files) 
		{
			string name = System.IO.Path.GetFileName(file);		%>
			
			<li class="file"><input type="checkbox" name="<%= file.Replace('\\', '/') %>" /><span><%= Html.Encode(name) %></span></li>
			
   <%	} %>
   
</ul>
	       