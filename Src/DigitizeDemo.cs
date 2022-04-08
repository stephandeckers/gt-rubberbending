/**
 * @Name Geometries.xaml.cs
 * @Purpose 
 * @Date 05 April 2022, 16:33:09
 * @Author S.Deckers
 * @Description 
 */

namespace Proximus.ifh.SwapInnerDucts
{
	#region -- Using directives --
	using System;
	using System.Windows.Forms;
	using Intergraph.GTechnology.API;

	using d = System.Diagnostics.Debug;
	#endregion

	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class SwapInnerDuctsController 
        : Intergraph.GTechnology.Interfaces.IGTCustomCommandModeless
	{
        /// <summary date="24-05-2019, 12:01:46" author="S.Deckers">
        /// Initialize self
        /// </summary>
        private void Init( )
        {            		
			this.GeometryCreationService = GTClassFactory.Create<IGTGeometryCreationService>();
			this.GeometryCreationService.TargetMapWindow = this.GTApp.Application.ActiveMapWindow;
        }

        private void Window_Closed( object sender, EventArgs e)
        {
			this?.CustomCommandHelper?.Complete();
        }

		private void setGtStatusBar( string message)
		{
			this.GTApp.SetStatusBarText( GTStatusPanelConstants.gtaspcMessage, message);
		}

		public bool						CanTerminate		{ get { return( true); }  }
		public IGTTransactionManager	TransactionManager	{ get; set; }
		public IGTCustomCommandHelper	CustomCommandHelper { get; set; }

		private IGTApplication _GTApp = null;
		private IGTApplication GTApp
		{
			get
			{
				if( this._GTApp == null)
				{
					this._GTApp = GTClassFactory.Create<IGTApplication>();
				}

				return( this._GTApp);
			}
		}

		public void Activate( IGTCustomCommandHelper CustomCommandHelper)
        {
			this.CustomCommandHelper = CustomCommandHelper;

            Init				( );
			SubscribeEvents		( );
        }

		private void DisposeObjects()
		{
			UnSubscribeEvents	( );

			this.GeometryCreationService.RemoveAllGeometries( );
			this.GeometryCreationService = null;
		}

        private void SubscribeEvents( )
        {
			this.CustomCommandHelper.Click		+= CustomCommandHelper_Click;
			this.CustomCommandHelper.DblClick	+= CustomCommandHelper_DblClick;
			this.CustomCommandHelper.KeyUp		+= CustomCommandHelper_KeyUp;
			this.CustomCommandHelper.MouseMove	+= CustomCommandHelper_MouseMove;
        }

		private void UnSubscribeEvents( )
        {
			this.CustomCommandHelper.Click		-= CustomCommandHelper_Click;
			this.CustomCommandHelper.DblClick	-= CustomCommandHelper_DblClick;
			this.CustomCommandHelper.KeyUp		-= CustomCommandHelper_KeyUp;
			this.CustomCommandHelper.MouseMove	-= CustomCommandHelper_MouseMove;
        }

		public void Pause( )
        {}

        public void Resume( )
        {}

        public void Terminate( )
        {
			DisposeObjects();
        }

		private int PntCnt	{ get; set; } = 0;
		private int Idx		{ get; set; }

		private IGTGeometryCreationService GeometryCreationService { get; set; } = GTClassFactory.Create<IGTGeometryCreationService>();		

		private const int StyleId = 5010;

		private void CustomCommandHelper_KeyUp( object sender, GTKeyEventArgs e )
		{
			if (e.KeyCode == (short)Keys.Escape)
			{
				this.GeometryCreationService.RemoveLastPoint( this.Idx);
			}
		}

		private void CustomCommandHelper_Click( object sender, GTMouseEventArgs e )
		{
			if( this.PntCnt == 0)
			{
				this.PntCnt++;
				this.Idx = this.GeometryCreationService.AddGeometry( GTClassFactory.Create<IGTPolygonGeometry>(), StyleId);
				this.GeometryCreationService.AppendPoint( this.Idx, e.WorldPoint);
				return;
			}
			this.GeometryCreationService.AppendPoint( this.Idx, e.WorldPoint);
		}

		private void CustomCommandHelper_MouseMove( object sender, GTMouseEventArgs e )
		{			
			if( this.PntCnt == 0)
			{
				this.setGtStatusBar( "Click to start digitizing");
				return;
			}

			if( this.PntCnt == 1)
			{
				this.setGtStatusBar( "Click to add point");
				this.GeometryCreationService.SetDynamicPoint( this.Idx, e.WorldPoint);
				return;
			}
						
			this.setGtStatusBar( "Click to add point, Doubleclick to end");
			this.GeometryCreationService.SetDynamicPoint( this.Idx, e.WorldPoint);
		}

		private void CustomCommandHelper_DblClick( object sender, GTMouseEventArgs e )
		{
			this.PntCnt = 0;
		}
	}
}
