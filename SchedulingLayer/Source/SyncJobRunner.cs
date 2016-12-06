using Quartz;
using Quartz.Impl;
using System.Runtime.CompilerServices;

namespace Organisation.SchedulingLayer
{
    [DisallowConcurrentExecution]
    public class SyncJobRunner : IJob
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static bool initialized = false;

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void Init()
        {
            if (!initialized)
            {
                log.Info("Starting SchedulingLayer");

                // get a scheduler
                IScheduler sched = StdSchedulerFactory.GetDefaultScheduler();
                sched.Start();

                // define the job and tie it to our HelloJob class
                IJobDetail job = JobBuilder.Create<SyncJobRunner>()
                    .WithIdentity("syncJob", "syncGroup")
                    .Build();

                // execute updater every minute
                ITrigger trigger = TriggerBuilder.Create()
                  .WithIdentity("syncTrigger", "syncGroup")
                  .StartNow()
                  .WithSimpleSchedule(x => x
                      .WithIntervalInMinutes(1)
                      .RepeatForever())
                  .Build();

                sched.DeleteJob(job.Key); // delete the job if it's already there from some previous execution
                sched.ScheduleJob(job, trigger);
                initialized = true;
            }
        }

        public void Execute(IJobExecutionContext context)
        {
            SyncJob.Run();
        }
    }
}
