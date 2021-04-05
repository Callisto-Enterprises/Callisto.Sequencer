using Callisto.Sequencer.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace SequencerIntegrationTests.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class QueueController : ControllerBase
    {
        private readonly ILogger<QueueController> _logger;
        private readonly ISequencerTaskQueue _queue;

        public QueueController(ILogger<QueueController> logger, ISequencerTaskQueue queue)
        {
            _logger = logger;
            _queue = queue;
        }

        [HttpGet]
        public IActionResult Queue()
        {
            _logger.LogInformation("Queuing Task");
            _queue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2), token);
                _logger.LogInformation("This is a queued task 🕳");
            });
            return Ok(new { status = "Task Queued Successfully" });
        }
    }
}