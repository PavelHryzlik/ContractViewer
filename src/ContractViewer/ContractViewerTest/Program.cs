using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ContractViewer.Controllers;
using ContractViewer.Models;

namespace ContractViewerTest
{
    /// <summary>
    /// Very simple program from testing performance of web application
    /// </summary>
    class Program
    {
        private static readonly SparqlQueryHandler Handler = new SparqlQueryHandler();

        static void Main()
        {
            var stopWatch = Stopwatch.StartNew();
            const string publisher = "Třebíč";
            const string publisherId = "00290629";

            // write results to output.txt
            using (StreamWriter sw = new StreamWriter(@"output.txt"))
            {
                try
                {
                    // Test main page data (Publishers and Contracts)
                    var test1Results = new List<long>();
                    for (int i = 0; i < 50; i++)
                    {
                        Handler.GetPublishers();
                        Handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContracts);

                        long test1Elapsed = stopWatch.ElapsedMilliseconds;
                        test1Results.Add(test1Elapsed);
                        sw.WriteLine("Test1 - MainPage (Publishers and Contracts): " + test1Elapsed + "ms");
                        Console.WriteLine("Test1 - MainPage (Publishers and Contracts): " + test1Elapsed + "ms");
                        stopWatch.Restart();
                    }

                    test1Results.Remove(test1Results.Max());
                    test1Results.Remove(test1Results.Min());
                    var test1Avereage = test1Results.Average(); // Compute result

                    sw.WriteLine("Test1 - MainPage (Publishers and Contracts): " + test1Avereage + "ms");
                    Console.WriteLine("Test1 - MainPage (Publishers and Contracts): " + test1Avereage + "ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    // Test SubjectDetail page
                    var test2Results = new List<long>();
                    for (int i = 0; i < 50; i++)
                    {
                        Handler.GetPublisher(publisher, publisherId);
                        Handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContractsByPublisherIc, "publisherId", publisherId, LocalNodeType.Literal);

                        long test2Elapsed = stopWatch.ElapsedMilliseconds;
                        test2Results.Add(test2Elapsed);
                        sw.WriteLine("Test2 - SubjectDetail: " + test2Elapsed + "ms");
                        Console.WriteLine("Test2 - SubjectDetail: " + test2Elapsed + "ms");
                        stopWatch.Restart();
                    }

                    test2Results.Remove(test2Results.Max());
                    test2Results.Remove(test2Results.Min());
                    var test2Avereage = test2Results.Average(); // Compute result

                    sw.WriteLine("Test2 - SubjectDetail AvereageTime: " + test2Avereage + "ms");
                    Console.WriteLine("Test2 - SubjectDetail AvereageTime: " + test2Avereage + "ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    // Test ContractDetail page
                    var test3Results = new List<long>();
                    for (int i = 0; i < 50; i++)
                    {
                        var contract = Handler.GetContract("http://localhost:7598/", "51", "1", publisher);

                        Handler.GetEntities<Party>(Constants.StudentOpenDataCz.GetPartiesByContract, "contract", contract.Uri, LocalNodeType.Uri);
                        Handler.GetEntities<Attachment>(Constants.StudentOpenDataCz.GetAttachmentsByContract, "contract", contract.Uri, LocalNodeType.Uri);
                        Handler.GetEntities<Amendment>(Constants.StudentOpenDataCz.GetAmendmentsByContract, "contract", contract.Uri, LocalNodeType.Uri);
                        Handler.GetEntities<Milestone>(Constants.StudentOpenDataCz.GetMilestonesByContract, "contract", contract.Uri, LocalNodeType.Uri);

                        long test3Elapsed = stopWatch.ElapsedMilliseconds;
                        test3Results.Add(test3Elapsed);
                        sw.WriteLine("Test3 - ContractDetail: " + test3Elapsed + "ms");
                        Console.WriteLine("Test3 - ContractDetail: " + test3Elapsed + "ms");
                        stopWatch.Restart();
                    }

                    test3Results.Remove(test3Results.Max());
                    test3Results.Remove(test3Results.Min());
                    var test3Avereage = test3Results.Average(); // Compute result

                    sw.WriteLine("Test3 - ContractDetail AvereageTime : " + test3Avereage + "ms");
                    Console.WriteLine("Test3 - ContractDetail AvereageTime : " + test3Avereage + "ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                try
                {
                    // Test PublicContracts page
                    var test4Results = new List<long>();
                    for (int i = 0; i < 50; i++)
                    {
                        const string subjectUri = "http://linked.opendata.cz/resource/business-entity/CZ43871020";
                        Handler.GetEntities<PublicContract>(Constants.LinkedOpenDataCz.GetBusinessEntityPublicContracts, "businessEntity", subjectUri, LocalNodeType.Uri, Constants.LinkedOpenDataCz.SparqlEndpointUri);

                        long test4Elapsed = stopWatch.ElapsedMilliseconds;
                        test4Results.Add(test4Elapsed);
                        sw.WriteLine("Test4 - PublicContracts: " + test4Elapsed + "ms");
                        Console.WriteLine("Test4 - PublicContracts: " + test4Elapsed + "ms");
                        stopWatch.Restart();
                    }

                    test4Results.Remove(test4Results.Max());
                    test4Results.Remove(test4Results.Min());
                    var test4Avereage = test4Results.Average(); // Compute result

                    sw.WriteLine("Test4 - PublicContracts AvereageTime: " + test4Avereage + "ms");
                    Console.WriteLine("Test4 - PublicContracts AvereageTime: " + test4Avereage + "ms");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }

            stopWatch.Stop();

            Console.ReadLine();
        }
    }
}
