using ADX.CTRL.Configuration;

using GK.AppCore.Abstractions;
using GK.AppCore.Workers;

using Microsoft.Extensions.DependencyInjection;

using System;
using System.Threading.Tasks;

namespace STSOrgSyncV2
{
    // ================================================================================
    public class ApplicationControl : GK.AppCore.ApplicationControl, IApplicationControl
    {
        readonly IServiceProvider _serviceProvider;

        //readonly IMessageQueueFactory _messageQueueFactory;

        bool _ready = false;

        // -----------------------------------------------------------------------------
        public ApplicationControl(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            _serviceProvider = serviceProvider;

            SetupADXCTRL(serviceProvider);

            //_messageQueueFactory = _serviceProvider.GetService<IMessageQueueFactory>();
        }

        // -----------------------------------------------------------------------------
        void SetupADXCTRL(IServiceProvider serviceProvider)
        {
            // Register global cancellation token @ ADXCTRL component
            var adxCtrlAppBuilder = serviceProvider.GetService<IADXCTRLAppBuilder>();

            // Signals to ADXCTRL to handle events, possibly filter events away and otherwise store valid events into the filtered queue.
            adxCtrlAppBuilder.UseFilterQueue = true;

            // Signals to ADXCTRL to enable events to be consumed from either unfiltered or filtered queue.
            // And for each consumed event call IADXEventHandler.HandleMessageAsync().
            adxCtrlAppBuilder.EnableADXEventHandler = true;

            adxCtrlAppBuilder.Prepare(RootCancellationToken);
        }

        // -----------------------------------------------------------------------------
        public override async void Start()
        {
            base.Start();

            if (!_ready)
            {
                await PrepareAsync();
            }
            else
            {
                GoGoGo();
            }
        }

        // -----------------------------------------------------------------------------
        async Task PrepareAsync()
        {
            ServicesRepo.AddService(_serviceProvider.GetService<IHousemaidWorker>());

            //PrepareADXConsumer(subscription);

            _ready = true;

            await Task.Yield();

            GoGoGo();
        }

        // -----------------------------------------------------------------------------
        void GoGoGo()
        {
            ServicesRepo.StartAllServices();
        }

        // -----------------------------------------------------------------------------
        //void PrepareADXConsumer(ISubscription subscription)
        //{
        //    var rootName = $"SUB-{subscription.Name.ToUpperInvariant()}-OUTgoingADEvents";

        //    IPublisherConfig publisherConfig = _messageQueueFactory.CreatePublisherConfig(rootName);
        //    publisherConfig.AddBindingQueue(_messageQueueFactory.CreateQueueName(rootName));

        //    // Assign publisher to local var so it will be available to NotificationsController class over GetIncomingPublisher() below...
        //    var outgoingPublisher = _messageQueueFactory.CreatePublisher(publisherConfig);

        //    subscription.Publisher = outgoingPublisher;

        //    // ---

        //    var eventHandler = _outgoingHandlerFactory.CreateOutgoingHandler(subscription);

        //    // Build consumer used for consuming persisted events from above incoming (unfiltered) queue
        //    var consumerConfig = _messageQueueFactory.CreateConsumerConfig(rootName);
        //    consumerConfig.AddMessageHandler(eventHandler);

        //    var consumer = _messageQueueFactory.CreateConsumer(consumerConfig);
        //    ServicesRepo.AddService(consumer);
        //}
    }
}
