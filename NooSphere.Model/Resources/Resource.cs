using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ABC.Model.Primitives;

namespace ABC.Model.Resources
{
    public class Resource : Noo, IResource
    {
        public Resource()
		{
			BaseType = typeof( IResource ).Name;
		}

    }
}
