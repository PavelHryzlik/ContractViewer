using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using ContractViewer.Models;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Ontology;
using VDS.RDF.Parsing;
using VDS.RDF.Query;
using WebGrease;
using WebGrease.Css.Extensions;

namespace ContractViewer.Controllers
{
    public class MainController : Controller
    {
        private const string ContractOntology = "https://raw.githubusercontent.com/PavelHryzlik/ContractStandard/master/standard/lod/contract_ontology.ttl";

        private Contract GetContract(string baseDomain, string contractId, string version, string publisher)
        {
            var queryString = new SparqlParameterizedString();
            queryString.CommandText = SparqlQueryConstants.SelectBySubject;
            var contractUri = new Uri(String.Format("http://{0}/contract/{1}/{2}", baseDomain, contractId, version));
            queryString.SetUri("subject", contractUri);

            var endpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));
            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());

            var contract = new Contract();

            //var g = new OntologyGraph();
            //UriLoader.Load(g, new Uri(ContractOntology), new TurtleParser());


            contract.Uri = contractUri.ToString();
            contract.BaseDomain = baseDomain;
            contract.ContractId = contractId;
            contract.Version = version;
            contract.Publisher = publisher;

            foreach (SparqlResult result in results.Results)
            {
                string predicate = String.Empty;
                foreach (var var in result.Variables)
                {
                    if (var == "p")
                    {
                        var urinode = ((IUriNode)result.Value(var));

                        predicate = !String.IsNullOrEmpty(urinode.Uri.Fragment)
                            ? urinode.Uri.Fragment.Replace("#", "")
                            : urinode.Uri.Segments.Last();
                    }

                    if (var == "o")
                    {
                        INode value = result.Value(var);

                        switch (value.NodeType)
                        {
                            case NodeType.Literal:
                                var node = W3CSpecHelper.FormatNode(value);

                                if (predicate == "awardID")
                                {
                                    contract.AwardID = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "title")
                                {
                                    contract.Title = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "contractType")
                                {
                                    contract.ContractType = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "description")
                                {
                                    contract.Description = ((ILiteralNode)node).Value;
                                }

                                if (predicate == "created")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.DateSigned = ((DateNode)node).AsDateTime();
                                    }
                                }
                                if (predicate == "validFrom")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.ValidFrom = ((DateNode)node).AsDateTime();
                                    }
                                }
                                if (predicate == "validUntil")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.ValidUntil = ((DateNode)node).AsDateTime();
                                    }
                                }

                                if (predicate == "competency")
                                {
                                    contract.Competency = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "anonymised")
                                {
                                    contract.Anonymised = ((ILiteralNode)node).Value;
                                }


                                if (predicate == "hasCurrency")
                                {
                                    contract.Currency = ((ILiteralNode)node).Value;
                                }
                                if (predicate == "hasCurrencyValue")
                                {
                                    contract.Amount = ((ILiteralNode)node).Value;
                                }
                                break;

                            case NodeType.Uri:
                                var uri = (IUriNode)value;

                                if (predicate == "contentUrl")
                                {
                                    contract.Document = ((IUriNode)uri).Uri.ToString();
                                }

                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            return contract;
        }

        private Publisher GetPublisher(string publisherName)
        {
            var publisher = new Publisher { Name = publisherName };

            var dvPediaQueryString = new SparqlParameterizedString { CommandText = SparqlQueryConstants.SelectDBpediaInfo };
            dvPediaQueryString.SetLiteral("publisher", publisherName);

            var dbPediaEndpoint = new SparqlRemoteEndpoint(new Uri("http://cs.dbpedia.org/sparql"));
            SparqlResultSet dbPediaResults = dbPediaEndpoint.QueryWithResultSet(dvPediaQueryString.ToString());

            foreach (SparqlResult result in dbPediaResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("img").ToString()))
                {
                    publisher.ImageUrl = result.Value("img").ToString();
                }
            }

            return publisher;
        }





        public ActionResult Index()
        {
            var handler = new SparqlResultHandler();

            return View(handler.GetContracts<Contract>(SparqlQueryConstants.SelectContracts, null));
        }

        public ActionResult SubjectDetail(string publisher)
        {
            var handler = new SparqlResultHandler();

            var publisherViewModel = new PublisherViewModel
            {
                Publisher = GetPublisher(publisher),
                Contracts = handler.GetContracts<Contract>(SparqlQueryConstants.SelectContractsByPublisher, publisher)
            };

            return View(publisherViewModel);
        }

        public ActionResult ContractDetail(string baseDomain, string contractId, string version, string publisher)
        {
            var handler = new SparqlResultHandler();

            var contract = GetContract(baseDomain, contractId, version, publisher);

            var contractViewModel = new ContractViewModel
            {
                Contract = contract,
                Parties = handler.GetContracts<Party>(SparqlQueryConstants.SelectParties, "contract", contract.Uri),
                Attachments = handler.GetContracts<Attachment>(SparqlQueryConstants.SelectAttachments, "contract", contract.Uri),
                Amendments = handler.GetContracts<Amendment>(SparqlQueryConstants.SelectAmendments, "contract", contract.Uri),
                Milestones = handler.GetContracts<Milestone>(SparqlQueryConstants.SelectMilestones, "contract", contract.Uri)
            };

            return View(contractViewModel);
        }
    }
}