<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Rlc.Monitor.AgentData>" %>


	<%
	string[] parts = Model.CurrentDirectory.Split(new char[]{'\\', '/'});
	string folder = "";
	%>
	
	<ul class="dir-nav">
		<%	foreach (string part in parts)
			{ 
				folder += part;
				if(!string.IsNullOrEmpty(part))
				{	%>
					<li><a href="#" onclick="javascript:changeDirectory('<%= folder %>');"><%= part %>\</a></li>
		<%		}
				folder += "/";
				%>
			
		<%	}	%>
	</ul>