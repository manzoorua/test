using System;
using System.Windows.Forms;

namespace MonitorAgent
{
	public partial class Form1 : Form
	{
		private readonly Guid _agentId = Guid.NewGuid();
		private readonly Agent _agent;

		/// <summary>
		/// Initializes a new instance of the <see cref="Form1"/> class.
		/// </summary>
		public Form1()
		{
			InitializeComponent();

			try
			{
				_agent = new Agent(_agentId);
				_agent.OnPollEvent += Agent_OnPollEvent;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}

			if(!_agent.Start())
			{
				MessageBox.Show(@"Unable to start the agent.");
			}
		}

		protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
		{
			if(!_agent.Stop())
			{
				MessageBox.Show(@"Unable to stop the agent.");
			}
			base.OnClosing(e);
		}

		private void Agent_OnPollEvent(Agent agent)
		{
			if (IsHandleCreated && InvokeRequired)
			{
				BeginInvoke(new MethodInvoker(delegate
				        {
                       		// Update the UI
                       		//
                       		fullImage.Text = agent.ImageFullSize.Width + @"x" + agent.ImageFullSize.Height;
                       		double percent = 100.0*agent.PartialImageSize.Width*
                       		                 agent.PartialImageSize.Height/
                       		                 (agent.ImageFullSize.Width*agent.ImageFullSize.Height);
                       		partialImage.Text = agent.PartialImageSize.Width + @"x" +
                       		                    agent.PartialImageSize.Height + @" [" +
                       		                    percent.ToString("0.0") + @"%]";
                       		lastUpdate.Text = agent.LastPoll.ToLongTimeString();
                       		pollMode.Text = agent.PollMode;
                       		pollInterval.Text = agent.PollInterval.ToString();
				        	currentDirectory.Text = agent.CurrentDirectory;
                       		Text = @"Monitor Agent: " + _agentId;

                       	}));
			}


		}
	}
}
