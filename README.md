# ParallelCompletion

A simple C# library to enable you to wait for multiple tasks to complete.

## .NET 5 requirement

ParallelCompletion used two new C# 9 features in code:
- `new()` constructor calls
- `TaskCompletionSource` without a return type

You can manually mitigate them and downgrade to C# 8 or earlier in order to use this library.

## Usage

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
