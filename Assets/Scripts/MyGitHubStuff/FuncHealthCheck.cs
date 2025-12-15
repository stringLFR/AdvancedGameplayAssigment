using System.Diagnostics;
using System.Collections.Generic;


namespace FuncHealthChecks
{
    //You need to have debughing active and be in Debug mode to see the writeLines strings in the output!
    //ALso that this is mostly checking things in debug mode and not in release, which should be way quicker that debug!
    //And Finaly. This is a simple and light way to check perfromace outside of the heavier stuff!
    //Also remmember to test more than ones, since if something changed the compiler might add some extra milliseconds!

    public static class FuncHealthCheck
    {
        public class FuncHealthData
        {
            public bool isInspecting;
            public long startTime;
            public long startMemory;

            public FuncHealthData()
            {
                isInspecting = false;
                startTime = 0;
                startMemory = 0;
            }
        }

        static Dictionary<string,FuncHealthData> FunctionPatients;

        public static void AddInspector(string funcNameKey)
        {
            #if DEBUG

            if (FunctionPatients == null) FunctionPatients = new Dictionary<string, FuncHealthData>();

            if (FunctionPatients.ContainsKey(funcNameKey) == true) return;

            FunctionPatients.Add(funcNameKey, new FuncHealthData()); 

            Debug.WriteLine($"FuncHealthCheck->FunctionPatient {funcNameKey} added!");

            #endif
        }

        public static void StartInspection(string funcNameKey)
        {
            #if DEBUG

            if (FunctionPatients.TryGetValue(funcNameKey, out var result) == true)
            {
                if (result.isInspecting == true) return;

                result.isInspecting = true;

                Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: Starting inspection!");

                result.startMemory = System.GC.GetTotalMemory(true);

                //THis needs to be last!
                result.startTime = Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000); //Doing this to avoid making a stopwatch object and add unecesary memory!
            }

            #endif
        }

        public static void StopInspection(string funcNameKey)
        {
            #if DEBUG

            if (FunctionPatients.TryGetValue(funcNameKey, out var result) == true)
            {
                if (result.isInspecting == false) return;

                //This needs to be first!
                long endTime = Stopwatch.GetTimestamp() / (Stopwatch.Frequency / 1000); //Doing this to avoid making a stopwatch object and add unecesary memory!

                result.isInspecting = false;

                long duration = endTime - result.startTime; 

                long currentMemory = System.GC.GetTotalMemory(true);

                Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: Function Took {duration} milliseconds!");
                Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: And total memory was changed from {result.startMemory} to {currentMemory}!");

                if (result.startMemory - currentMemory == 0) Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: So no memory change!");
                else if (result.startMemory > currentMemory) Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: Memory became less by {result.startMemory - currentMemory}!");
                else Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: Memory became greater by {currentMemory - result.startMemory}!");

                Debug.WriteLine($"FuncHealthCheck->FunctionPatients->{funcNameKey}: In short a percentage change of {(double)(currentMemory - result.startMemory) / result.startMemory}!");

                result.startTime = 0;
                result.startMemory = 0;
            }

            #endif
        }

    }
}


