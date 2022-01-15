using System;
using System.Runtime.CompilerServices;
using System.Threading;

static class AsyncHelper
{
	public static ThreadPoolRedirector RedirectToThreadPool() =>
		new ThreadPoolRedirector();
}

public struct ThreadPoolRedirector : INotifyCompletion
{
	public ThreadPoolRedirector GetAwaiter() => this;

	public bool IsCompleted => Thread.CurrentThread.IsThreadPoolThread;

	public void OnCompleted(Action continuation) =>
		ThreadPool.QueueUserWorkItem(o => continuation());

	public void GetResult()
	{
	}
}