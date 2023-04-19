# Callisto.Sequencer

Simple Background Task Queue Service

## Installation

Install the package from NuGet:

```bash
dotnet add package Callisto.Sequencer
```

## Usage

### Basic

```csharp
// Program.cs
using Callisto.Sequencer;

// Register the service
builder.Services.AddSequencer(25); // 25 is the number of parallel tasks to run, default is 1

// ...
var app = builder.Build();

app.UseEndpoints(endpoints =>
{
    //...
    endpoints.MapPost("/api/background", (ISequencerTaskQueue queue) =>
    {
        queue.QueueBackgroundWorkItem(async token =>
        {
            await Task.Delay(TimeSpan.FromSeconds(2), token);
        });
    });
});
```
