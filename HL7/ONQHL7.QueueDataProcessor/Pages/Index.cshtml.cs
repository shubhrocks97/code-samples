using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using ONQHL7.GlobalDb.Interfaces;

namespace ONQHL7.QueueDataProcessor.Pages
{
	public class IndexModel : PageModel
	{
		private readonly ILogger<IndexModel> m_logger;
		public IndexModel(ILogger<IndexModel> a_mLogger)
		{
			m_logger = a_mLogger;
		}

		public void OnGet()
		{
		}
	}
}
