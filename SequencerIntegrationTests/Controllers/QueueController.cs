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

        [HttpGet("Single")]
        public IActionResult SingleQueue()
        {
            _logger.LogInformation("Queuing Single Task");
            _queue.QueueBackgroundWorkItem(async token =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2), token);
                _logger.LogInformation("This is a queued task 🕳");
            });
            return Ok(new { status = "Task Queued Successfully" });
        }

        [HttpGet("Parallel")]
        public IActionResult ParallelQueue()
        {
            _logger.LogInformation("Queuing Parallel Task");
            _queue.QueueBackgroundWorkItem(async token =>
            {
                _logger.LogInformation("First Task Started");
                await Task.Delay(TimeSpan.FromSeconds(2), token);
                _logger.LogInformation("First Task Completed! 😀");
            });

            for (int i = 0; i < 50; i++)
            {
                QueueTask(i);
            }
           
            return Ok(new { status = "Tasks Queued Successfully" });
        }

        private void QueueTask(int i)
        {
            _queue.QueueBackgroundWorkItem(async token =>
            {
                var label = $"[TASK] No. {i} Executed";
                var random = new Random();
                await Task.Delay(random.Next(50, 1000), token);
                _logger.LogInformation(label);
            });
        }
    }
}