using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ContractViewer.Controllers;
using ContractViewer.Models;

namespace ContractViewerTest
{
    class Program
    {
        private static readonly SparqlQueryHandler _handler = new SparqlQueryHandler();

        static void Main(string[] args)
        {
            var stopWatch = Stopwatch.StartNew();
            var publisher = "Třebíč";

            try
            {
                // Test main page data
                for (int i = 0; i < 50; i++)
                {
                    _handler.GetPublishers();
                    _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContracts);
                }
                Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            stopWatch.Restart();

            try
            {
                // Test SubjectDetail
                for (int i = 0; i < 50; i++)
                {
                    _handler.GetPublisher(publisher);
                    _handler.GetEntities<Contract>(Constants.StudentOpenDataCz.GetContractsByPublisherName, "publisher",publisher, LocalNodeType.Literal);
                }
                Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            stopWatch.Restart();

            try
            {
                // Test ContractDetail
                for (int i = 0; i < 50; i++)
                {
                    var contract = _handler.GetContract("http://localhost:7598/", "51", "1", publisher);

                    _handler.GetEntities<Party>(Constants.StudentOpenDataCz.GetPartiesByContract, "contract", contract.Uri,LocalNodeType.Uri);
                    _handler.GetEntities<Attachment>(Constants.StudentOpenDataCz.GetAttachmentsContract, "contract", contract.Uri, LocalNodeType.Uri);
                    _handler.GetEntities<Amendment>(Constants.StudentOpenDataCz.GetAmendmentsByContract, "contract", contract.Uri, LocalNodeType.Uri);
                    _handler.GetEntities<Milestone>(Constants.StudentOpenDataCz.GetMilestonesByContract, "contract", contract.Uri, LocalNodeType.Uri);
                }

                Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms"); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            stopWatch.Restart();

            try
            {
                // Test PublicContracts
                for (int i = 0; i < 50; i++)
                {
                    var subjectUri = "http://linked.opendata.cz/resource/business-entity/CZ43871020";
                    _handler.GetEntities<PublicContract>(Constants.LinkedOpenDataCz.GetBusinessEntityPublicContracts,"businessEntity", subjectUri, LocalNodeType.Uri, Constants.LinkedOpenDataCz.SparqlEndpointUri);
                }

                Console.WriteLine(stopWatch.ElapsedMilliseconds + "ms");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            stopWatch.Stop();

            Console.ReadLine();
        }
    }
}
