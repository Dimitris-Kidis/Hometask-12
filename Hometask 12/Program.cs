using System;


// 1. Create a program using events
//     - (1) Define type
//     - (2) Define event member
//     - (3) Define a notifying method
//     - (4) Define a method triggering an event

namespace App
{
    public class Program
    {
        static void Main(string[] args)
        {
            var order = new Order { Item = "Pizza with extra cheese" };

            var orderingService = new FoodOrderingService();
            var appService = new AppService();
            var mailService = new MailService();
            var fileService = new FileService();

            orderingService.Subscribe(appService);
            orderingService.Subscribe(mailService);
            orderingService.Subscribe(fileService);

            orderingService.PrepareOrder(order);

            orderingService.Unsubscribe(appService);
            orderingService.PrepareOrder(order);


        }
    }


    public class Order
    {
        public string Item { get; set; }
        public string Ingredients { get; set; }
    }


    public interface IService
    {
        void FoodPrepareHandler(object source, FoodPreparedEventArgs eventArgs);
    }


    public class AppService : IService
    {
        public void FoodPrepareHandler(object source, FoodPreparedEventArgs eventArgs)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"AppService: your food '{eventArgs.Order.Item}' is prepared.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class MailService : IService
    {
        public void FoodPrepareHandler(object source, FoodPreparedEventArgs eventArgs)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"MailService: your food '{eventArgs.Order.Item}' is prepared.");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class FileService : IService
    {
        public void FoodPrepareHandler(object source, FoodPreparedEventArgs eventArgs)
        {
            try
            {
                FileStream fileStream = File.Open("OrderLog.txt",
                    FileMode.Append, FileAccess.Write);

                StreamWriter fileWriter = new StreamWriter(fileStream);

                fileWriter.WriteLine(eventArgs.Order.Item + " ordered at " + System.DateTime.Now.ToString() + " by " + Environment.UserName + " on " + Environment.OSVersion);
                fileWriter.Flush();
                fileWriter.Close();

            }
            catch (IOException ioe)
            {
                Console.WriteLine(ioe);
            }
        }
    }


    // 1.
    public class FoodPreparedEventArgs : EventArgs
    {
        public Order Order { get; set; }
    }



    public class FoodOrderingService
    {
        // 2.
        //public delegate void FoodPreparedEventHandler(object source, FoodPreparedEventArgs args);
        //private event FoodPreparedEventHandler FoodPrepared;
        private event EventHandler<FoodPreparedEventArgs> FoodPreparedEventHandler = default!;

        public void Subscribe(IService service)
        {
            FoodPreparedEventHandler += service.FoodPrepareHandler;
        }

        public void Unsubscribe(IService service)
        {
            FoodPreparedEventHandler -= service.FoodPrepareHandler;
        }

        // 3.
        protected virtual void OnFoodPrepared(Order order)
        {
            if (FoodPreparedEventHandler != null)
                FoodPreparedEventHandler(this, new FoodPreparedEventArgs { Order = order });
        }

        // 4.
        public void PrepareOrder(Order order)
        {
            Console.WriteLine($"Preparing your order '{order.Item}', please wait...");
            Thread.Sleep(1000);

            OnFoodPrepared(order);
        }
    }






}


