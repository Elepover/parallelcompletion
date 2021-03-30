# ParallelCompletion

A simple C# library to enable you to wait for multiple tasks to complete.

## About this branch

This branch is the downgraded (C# 8) version of ParallelCompletion, using .NET Standard 2.1.

## Usage

**Note**: Each instance of `ParallelTokenSource` can only be used once. After `WaitForCompletion()` calls, they were invalidated and calling again will result in `InvalidOperationException`.

### Create and manage tokens yourself

```csharp
using ParallelCompletion;

...

// setup
var pts = new ParallelTokenSource();
var token = pts.CreateToken();

// run tasks
_ = Task.Run(async () =>
{
    ...
    token.Complete();
});

// wait for completion
await pts.WaitForCompletion();
```

### Automatically associate tasks with a `ParallelTokenSource`

```csharp
using ParallelCompletion;

...

// setup
var pts = new ParallelTokenSource();
pts.RunTask( ... );

// wait for completion
await pts.WaitForCompletion();
```

### Use extension method

```csharp
using ParallelCompletion;
using ParallelCompletion.Extensions;

...

// setup
var pts = new ParallelTokenSource();
...

someTask.RegisterTo(pts);

// wait for completion
await pts.WaitForCompletion();
```
