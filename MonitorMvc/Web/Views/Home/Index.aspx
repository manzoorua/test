<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<MonitorMvc.Models.HomeViewModel>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	Index
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <%= Html.ActionLink("Clear Agent Data", "ClearAgentData") %>

    <h2>Monitored Computers</h2>
    
    <div id="overlay">
		<div>
			<span>
				<a id="overlay-close" href="#">close</a>
			</span>
			<img id="detail-img" height="100%" src="#" alt="details" />
		</div>
    </div>
    
    <div id="screens">
		<h3>Screen Shots</h3>
		<% foreach (var item in Model.Agents) { %>

			<span class="screen_shot">
				<img src="<%= Url.Action("Thumb", "Image", new { agentId = item.AgentId }) %>" alt="screen shot" />
				<% Html.RenderPartial("ComputerThumbView", item); %>
				<a href="#" onclick="return showDetails('<%= item.AgentId %>');">details</a>
			</span>
			
		<% } %>	
	</div>
	
	<div id="footer" style="clear:both;">
		footer content
	</div>
	
	
	
	<script language="javascript" type="text/javascript">
		var intervalId;
		var stopDetails = true;
		$(document).ready(function() {
			$("#overlay").hide();
			$("#overlay-close").click(hideDetails);
			intervalId = setInterval(reloadScreen, 1000);

			$("#detail-img").load(function() {
				if (!stopDetails) {
					refreshDetails();
				}
			});
		});

		var detailsId;
		function showDetails(id) {
			// stop updating the other images
			clearInterval(intervalId);
			
			// set the url of the image
			detailsId = id;

			// set agent to details mode
			var url = "/Monitor/Image/DetailsMode?agentId=" + detailsId;
			$.getJSON(url, null, null);

			// start updating the details image
			stopDetails = false;
			refreshDetails();
			
			// show the overlay
			$("#overlay").show();
			return false;
		}

		function refreshDetails() {
			var now = new Date();
			var url = "/Monitor/Image/Thumb?agentId=" + detailsId + "&t=" + now.getTime();
			$("#detail-img").attr("src", url);
		}

		function hideDetails() {
			// stop refreshing details
			stopDetails = true;
			
			// hide the overlay
			$("#overlay").hide();

			// set agent to details mode
			var url = "/Monitor/Image/ThumbMode?agentId=" + detailsId;
			$.getJSON(url, null, null);
			detailsId = null;
			
			// start the interval for the thumbs
			intervalId = setInterval(reloadScreen, 1000);
			
			return false;
		}
		
		var currentIndex = 0;
		function reloadScreen() {
			var screens = $("span.screen_shot");
			if (screens == undefined || screens == null || screens.size() == 0) {
				return;
			}
			var span = screens[currentIndex];
			currentIndex++;
			if (currentIndex >= screens.size()) {
				currentIndex = 0;
			}
			var url = $(span).find("img").attr("src");
			var parts = url.split("&");
			var urlData = parts[0].replace("Thumb", "ThumbHtml");
			$(span).find("div.data").load(urlData);
			var now = new Date();
			var urlImg = parts[0] + "&t=" + now.getTime();
			$(span).find("img").attr("src", urlImg);
		}
	</script>
</asp:Content>

