using System.Collections.Generic;
using ABC.Model.Primitives;


namespace ABC.Model
{
	public class Action : Noo
	{
		public Action()
		{
			InitializeProperties();
		}


		#region Initializers

		void InitializeProperties()
		{
			Resources = new List<FileResource>();
		}

		#endregion


		#region Properties

		List<FileResource> _resources;

		public List<FileResource> Resources
		{
			get { return _resources; }
			set
			{
				_resources = value;
				OnPropertyChanged( "Resources" );
			}
		}

		#endregion
	}
}