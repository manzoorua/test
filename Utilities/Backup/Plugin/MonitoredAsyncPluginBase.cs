using System;
using System.Threading;
using Rlc.Utilities.Log;

namespace Rlc.Utilities.Plugin
{
	/// <summary>
	/// Provides an abstract base class implementation that allows 
	/// a long running process to be monitored. The long running process 
	/// is executed in a new Thread instance (not on the ThreadPool). The
	/// implementation of the abstract Run method must periodically call
	/// the DoHeartbeat method to provide an indication that it is still alive.
	/// 
	/// To Use (minimal implementation)
	/// 1. Derive from this class.
	/// 2. Implement the abstract properties & methods (see description below).
	/// 3. Have a manager object subscribe to the exposed events (OnIsDead at minimum).
	/// 4. Have a manager object call Start
	/// 
	/// </summary>
	public abstract class MonitoredAsyncPluginBase : IPlugin
	{
		#region Abstract Members, Properties, Methods

		/// <summary>
		/// Provides the implementation for the process to be monitored.
		/// 
		/// You must provide the 'heartbeat' to indicate the process is
		///		alive by calling the DoHeartBeat method.
		/// Use the IsStopRequested boolean to allow processes to be
		///		repeated until stopped.
		/// 
		///		while(!IsStopRequested)
		///		{
		///			/* Do your stuff here */
		///		
		///			DoHeartBeat();
		///		}
		/// 
		/// </summary>
		/// <returns></returns>
		protected abstract bool Run();

		/// <summary>
		/// Gets the description of the plugin.
		/// </summary>
		/// <value>The description.</value>
		public abstract string Description { get; }

		/// <summary>
		/// Gets the GUID of the plugin. This should be unique for
		/// each plugin.
		/// </summary>
		/// <value>The id.</value>
		public abstract Guid Id { get; }

		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		/// <value>The name.</value>
		public abstract string Name { get; }

		/// <summary>
		/// Gets the version of the plugin.
		/// </summary>
		/// <value>The version.</value>
		public abstract Version Version { get; }

		#endregion

		#region Events

		public delegate void MonitoredPluginEventHandler(MonitoredAsyncPluginBase source);

		/// <summary>
		/// Occurs when Thread.Abort method has been called to stop the
		/// thread. Calling Thread.Abort can cause resource leaks. This
		/// event allows the manager of these monitored processes to
		/// react to this situation.
		/// </summary>
		public event MonitoredPluginEventHandler OnThreadAbortCalled;
		/// <summary>
		/// Occurs when an exception occurs during the execution of the
		/// abstract Run method.
		/// </summary>
		public event MonitoredPluginEventHandler OnException;
		/// <summary>
		/// Occurs when just after (before any other processing or calls
		/// are made) the Start method is called.
		/// </summary>
		public event MonitoredPluginEventHandler OnStarting;
		/// <summary>
		/// Occurs at the end of the Start method (after the processing
		/// thread has been started).
		/// </summary>
		public event MonitoredPluginEventHandler OnStarted;
		/// <summary>
		/// Occurs when the Run method returns if there was no exception.
		/// If there was an exception then the OnException event is raised.
		/// </summary>
		public event MonitoredPluginEventHandler OnComplete;
		/// <summary>
		/// Occurs right after the Stop method is called (before attempting to
		/// stop the worker thread).
		/// </summary>
		public event MonitoredPluginEventHandler OnStopping;
		/// <summary>
		/// Occurs at the end of the Stop method (after the thread has been
		/// stopped).
		/// </summary>
		public event MonitoredPluginEventHandler OnStopped;
		/// <summary>
		/// Occurs when the worker thread has been determine to be no longer
		/// 'alive'. Alive is defined to be that the DoHeartBeat method has been
		/// called within the last MaxTimBetweenHeartbeats time span.
		/// </summary>
		public event MonitoredPluginEventHandler OnIsDead;
		/// <summary>
		/// Occurs when the DoHeartBeat method has been called.
		/// </summary>
		public event MonitoredPluginEventHandler OnHeartbeat;
		
		#endregion

		#region Private Members

		/// <summary>
		/// The thread instance that is used to execute the long running
		/// process. A call to DoHeartbeat from this thread is required
		/// to indicate that it is still alive.
		/// </summary>
		private Thread _thread;

		/// <summary>
		/// The timer used to monitor the thread for 'alive' status.
		/// </summary>
		private System.Timers.Timer _timer;
	
		#endregion

		#region Private Properties

		/// <summary>
		/// Gets a value indicating whether this instance is alive. This
		/// property uses the LastHeartbeat and MaxTimeExpectedBetweenHeartbeats
		/// properties to determine the status of the thread.
		/// </summary>
		/// <value><c>true</c> if this instance is alive; otherwise, <c>false</c>.</value>
		private bool IsAlive 
		{
			get
			{
				TimeSpan span = DateTime.Now - LastHeartbeat;
				if (span > MaxTimeExpectedBetweenHeartbeats)
				{
					return false;
				}
				return true;
			}
		}

		#endregion

		#region Protected Properties

		/// <summary>
		/// Gets or sets a value indicating whether this instance is stop requested.
		/// This can be used in the Run method to allow a long running thread to check
		/// if a stop has been requested. This allows the thread to clean up.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is stop requested; otherwise, <c>false</c>.
		/// </value>
		protected bool IsStopRequested { get; set; }

		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
		protected ILogger Logger { get; private set; }

		#endregion

		#region Public Properties

		/// <summary>
		/// Gets or sets (private) the last time stamp for the last heartbeat.
		/// </summary>
		/// <value>The last heartbeat.</value>
		public DateTime LastHeartbeat { get; protected set; }

		/// <summary>
		/// Gets or sets (private) the max time expected between heartbeats. This
		/// is set in the constructor of this base class.
		/// </summary>
		/// <value>The max time expected between heartbeats.</value>
		public TimeSpan MaxTimeExpectedBetweenHeartbeats { get; protected set; }

		#endregion

		#region CTORS

		/// <summary>
		/// Initializes a new instance of the <see cref="MonitoredAsyncPluginBase"/> class.
		/// </summary>
		/// <param name="maxTimeExpectedBetweenHeartbeats">The max time expected between heartbeats.</param>
		/// <param name="logger">The logger.</param>
		protected MonitoredAsyncPluginBase(TimeSpan maxTimeExpectedBetweenHeartbeats, ILogger logger)
		{
			MaxTimeExpectedBetweenHeartbeats = maxTimeExpectedBetweenHeartbeats;
			if(logger==null)
			{
				throw new ArgumentNullException("logger");
			}
			Logger = logger;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonitoredAsyncPluginBase"/> class.
		/// </summary>
		/// <param name="maxTimeExpectedBetweenHeartbeatsInSeconds">The max time expected between heartbeats in seconds.</param>
		/// <param name="logger">The logger.</param>
		protected MonitoredAsyncPluginBase(int maxTimeExpectedBetweenHeartbeatsInSeconds, ILogger logger)
		{
			if (maxTimeExpectedBetweenHeartbeatsInSeconds < 0)
			{
				throw new ArgumentOutOfRangeException("maxTimeExpectedBetweenHeartbeatsInSeconds");
			}
			MaxTimeExpectedBetweenHeartbeats = new TimeSpan(0, 0, maxTimeExpectedBetweenHeartbeatsInSeconds);
			if (logger == null)
			{
				throw new ArgumentNullException("logger");
			}
			Logger = logger;
		}
		
		#endregion

		#region Public Methos

		/// <summary>
		/// Starts the monitored process. The following is the pattern that is followed:
		/// 1. OnStarting event is raised
		/// 2. Thread is stared that calls abstract Run
		/// 3. OnStarted event is raised.
		/// </summary>
		/// <returns>true if a new thread was started, false if a thread exists (need to stop the current thread)</returns>
		public bool Start()
		{
			if (_thread != null)
			{
				return false;
			}

			if (OnStarting != null)
			{
				OnStarting(this);
			}

			_thread = new Thread(ThreadProcess);
			_thread.Start();

			if (OnStarted != null)
			{
				OnStarted(this);
			}

			return true;
		}

		/// <summary>
		/// Stops the Run thread using the IsStopRequested flag. If the thread has not
		/// stopped before the timeout specified a call to Thread.Abort is made. The
		/// following pattern is followed:
		/// 1. OnStopping event is raised
		/// 2. The Heartbeat monitor is stopped.
		/// 3. IsStopRequested is set to true
		/// 4. Wait for the thread to complete for specified time out period.
		/// 5. If thread does not complete, a call to Thread.Abort is made and the OnThreadAbortCalled event is raised.
		/// 6. OnStopped event is raised.
		/// </summary>
		/// <param name="timeToWaitBeforeAbort">The time to wait before abort.</param>
		/// <returns></returns>
		public bool Stop(TimeSpan timeToWaitBeforeAbort)
		{
			int totalSecondsToWait;
			int.TryParse(timeToWaitBeforeAbort.TotalSeconds.ToString(), out totalSecondsToWait);
			return Stop(totalSecondsToWait);
		}

		/// <summary>
		/// Stops the Run thread using the IsStopRequested flag. If the thread has not
		/// stopped before the timeout specified a call to Thread.Abort is made. The
		/// following pattern is followed:
		/// 1. OnStopping event is raised
		/// 2. The Heartbeat monitor is stopped.
		/// 3. IsStopRequested is set to true
		/// 4. Wait for the thread to complete for specified time out period.
		/// 5. If thread does not complete, a call to Thread.Abort is made and the OnThreadAbortCalled event is raised.
		/// 6. OnStopped event is raised.
		/// </summary>
		/// <param name="numberOfSecondsToWaitBeforeAbort">The number of seconds to wait before abort.</param>
		/// <returns></returns>
		public bool Stop(int numberOfSecondsToWaitBeforeAbort)
		{
			if (_thread != null && _thread.IsAlive)
			{
				// Raise the stopping event
				//
				if (OnStopping != null)
				{
					OnStopping(this);
				}

				// Stop the heart beat monitor.
				//
				StopHeartbeatMonitor();

				int totalMillisecondsToWait = numberOfSecondsToWaitBeforeAbort > 0 ? 1000 * numberOfSecondsToWaitBeforeAbort : 0;

				// Try to stop by allowing the thread to stop
				//	on its own.
				//
				IsStopRequested = true;
				if (!_thread.Join(totalMillisecondsToWait))
				{
					// Tried to avoid this, but must abort. This
					//	has the potential to leave objects that cannot
					//	be reclaimed by the GC and/or objects in a
					//	unknown state.
					//
					_thread.Abort();

					// Raise abort called event to allow an external
					//	task manager to intelligently deal with the 
					//	situation.
					//
					if (OnThreadAbortCalled != null)
					{
						OnThreadAbortCalled(this);
					}
				}
				_thread = null;

				// Raise the stopped event.
				//
				if (OnStopped != null)
				{
					OnStopped(this);
				}

				return true;
			}
			return false;
		}

		#endregion

		#region Protected Methods

		/// <summary>
		/// Does a 'heartbeat' for the monitored thread. Without a heartbeat with in
		/// the MaxTimeExpectedBetweenHeartbeats interval the thread will no longer
		/// be considered 'alive' and the OnIsDead event will be raised.
		/// </summary>
		protected void DoHeartBeat()
		{
			LastHeartbeat = DateTime.Now;

			if (OnHeartbeat != null)
			{
				OnHeartbeat(this);
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Starts the heartbeat monitor.
		/// </summary>
		private void StartHeartbeatMonitor()
		{
			Logger.Info(Name + ": Starting heartbeat monitor. MaxTimeExpectedBetweenHeartbeats=" + MaxTimeExpectedBetweenHeartbeats.TotalMilliseconds + " milliseconds");
			_timer = new System.Timers.Timer(MaxTimeExpectedBetweenHeartbeats.TotalMilliseconds);
			_timer.Elapsed += TimerElapsed;
			_timer.Start();
		}

		/// <summary>
		/// Stops the heartbeat monitor.
		/// </summary>
		private void StopHeartbeatMonitor()
		{
			if (_timer != null)
			{
				_timer.Stop();

				_timer = null;
			}
			Logger.Info(Name + ": Stopped heartbeat monitor.");
		}

		/// <summary>
		/// Handles the Elapsed event of the _timer control (heartbeat monitor).
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.Timers.ElapsedEventArgs"/> instance containing the event data.</param>
		private void TimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			if (!IsAlive)
			{
				if (OnIsDead != null)
				{
					OnIsDead(this);
				}
			}
		}

		/// <summary>
		/// This is the thread method that calls the abstract Run method. The
		/// following pattern is used:
		/// 1. IsStopRequested flag is set to false.
		/// 2. The heartbeat monitor is started.
		/// 3. The abstract Run method is called.
		/// 4. The heartbeat monitor is stopped.
		/// 5. If an exception occured in the Run method, the OnException event
		/// is raised, otherwise the OnComplete event is raised.
		/// </summary>
		private void ThreadProcess()
		{
			bool wasException = false;
			IsStopRequested = false;

			// Start the heart beat monitor.
			//
			StartHeartbeatMonitor();

			try
			{
				Run();
			}
			catch (Exception ex)
			{
				Logger.Error(Name + ": Exception in the 'Run' method.", ex);
				wasException = true;
			}

			// Stop the heart beat monitor.
			//
			if (!IsStopRequested)
			{
				StopHeartbeatMonitor();
			}

			if (wasException)
			{
				// Raise the exception event
				//
				if (OnException != null)
				{
					OnException(this);
				}
			}
			else
			{
				if (!IsStopRequested)
				{
					// Finished on it's own...raise the run complete event.
					//
					if (OnComplete != null)
					{
						OnComplete(this);
					}
				}
			}
		}

		#endregion
	}
}