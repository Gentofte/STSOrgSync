using Quartz;
using Quartz.Impl;
using System.Runtime.CompilerServices;

namespace Organisation.IntegrationLayer
{
    internal class TokenCacheUpdater : IJob
    {
        private static bool initialized = false;

        // bootstrap the cache - mandatory to call this
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Init()
        {
            if (!initialized)
            {
                TokenCache.Init();

                // construct a scheduler factory
                ISchedulerFactory schedFact = new StdSchedulerFactory();

                // get a scheduler
                IScheduler sched = schedFact.GetScheduler();
                sched.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<TokenCacheUpdater>()
                    .WithIdentity("tokenCacheUpdaterJob", "tokenCacheUpdaterGroup")
                    .Build();

                // execute updater every 10 minutes
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity("tokenCacheUpdaterTrigger", "tokenCacheUpdaterGroup")
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInMinutes(10)
                      .RepeatForever())
                  .Build();

                sched.ScheduleJob(job, trigger);
                initialized = true;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            TokenCache.IssueAllTokens();
        }
    }
}
