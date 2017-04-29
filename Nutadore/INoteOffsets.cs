using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nutadore
{
	interface INoteOffsets
	{
		double offset { set; }
		double headOffset { get; }
	}
}
