using System;
using System.Linq;
using System.Activities;
using System.Activities.Statements;
using System.Activities.XamlIntegration;

namespace WorkflowConsoleApplication
{

    class Program
    {
        static void Main(string[] args)
        {
            ActivityXamlServicesSettings settings = new ActivityXamlServicesSettings
            {
                CompileExpressions = true
            };

            Activity workflow = ActivityXamlServices.Load("Workflow1.xaml", settings);
            WorkflowInvoker.Invoke(workflow);
            Console.WriteLine("Press <enter> to exit" + System.Environment.CurrentDirectory);
            Console.ReadLine();
        }
    }
}
