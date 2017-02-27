using System;
using System.Threading.Tasks;

namespace Trilistnik
{
	interface IDataLoader
	{
		Task LoadStartData();
		Task LoadAdditionalData();
		void OnInternetLost();
	}
}
