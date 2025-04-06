using Library;
using Quartz;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Server.Envir
{
    [DisallowConcurrentExecution]
    public class LoopAction : IJob
    {
        static Dictionary<string, Task> _threads = new Dictionary<string, Task>();
        public Task Execute(IJobExecutionContext context)
        {
            var action = context.MergedJobDataMap["action"] as Action;

            var threadId = context.MergedJobDataMap["threadId"].ToString();
            Thread.CurrentThread.Name = threadId;
            SEnvir.Now = Time.Now;
            action?.Invoke();
            return Task.CompletedTask;
        }
    }
}
