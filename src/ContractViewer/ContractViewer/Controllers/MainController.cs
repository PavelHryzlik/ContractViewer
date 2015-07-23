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
        private const string SelectBySubject = "SELECT * WHERE { @subject ?p ?o }";
        private const string SelectContracts = 
              @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Uri ?Publisher ?Title ?Created ?Price
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Contract> ;
                             dc:title ?Title ;
                             dc:created ?Created ;
                             gr:hasCurrencyValue ?Price .

                   BIND(REPLACE(str(?Uri), 'contract.*$', 'publisher') AS ?publisherStr)
                   BIND(URI(?publisherStr) AS ?pub ) 

                   ?pub gr:legalName ?Publisher .
                }";
        private const string SelectContractsByPublisher =
              @"PREFIX dc: <http://purl.org/dc/terms/>
                PREFIX gr: <http://purl.org/goodrelations/v1#>

                SELECT ?Uri ?Title ?Created ?Price
                WHERE 
                { 
                   ?Uri a <http://tiny.cc/open-contracting#Contract> ;
                             dc:title ?Title ;
                             dc:created ?Created ;
                             gr:hasCurrencyValue ?Price .

                   BIND(REPLACE(str(?Uri), 'contract.*$', 'publisher') AS ?publisherStr)
                   BIND(URI(?publisherStr) AS ?pub ) 

                   ?pub gr:legalName @publisher .
                }";
        private const string SelectDBpediaInfo =
              @"PREFIX rdfs: <http://www.w3.org/2000/01/rdf-schema#>
                PREFIX dbpedia-owl: <http://dbpedia.org/ontology/>
                
                SELECT DISTINCT ?city ?img
                WHERE {
                   ?city rdfs:label @publisher@cs;
                         dbpedia-owl:thumbnail ?img .       
                }";

        private IEnumerable<Contract> GetContracts(string publisherName)
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));

            SparqlResultSet results = null;
            if (String.IsNullOrEmpty(publisherName))
            {
                results = endpoint.QueryWithResultSet(SelectContracts);
            }
            else
            {
                var queryString = new SparqlParameterizedString {CommandText = SelectContractsByPublisher};
                queryString.SetLiteral("publisher", publisherName);

                results = endpoint.QueryWithResultSet(queryString.ToString());
            }

            var contracts = new List<Contract>();
            var contractType = typeof(Contract);

            foreach (SparqlResult result in results.Results)
            {
                var contract = new Contract();
                if (!String.IsNullOrEmpty(publisherName))
                    contract.Publisher = publisherName;

                foreach (var var in result.Variables)
                {
                    PropertyInfo prop = contractType.GetProperty(var);

                    if (prop != null)
                    {
                        string text;
                        INode value = result.Value(var);

                        switch (value.NodeType)
                        {
                            case NodeType.Literal:
                                var node = W3CSpecHelper.FormatNode(value);

                                if (node is DateNode)
                                {
                                    prop.SetValue(contract, ((DateNode)node).AsDateTime(), null);
                                }
                                else if (node is DateTimeNode)
                                {
                                    prop.SetValue(contract, ((DateTimeNode)node).AsDateTimeOffset(), null);
                                }
                                else if (node is TimeSpanNode)
                                {
                                    prop.SetValue(contract, ((TimeSpanNode)node).AsTimeSpan(), null);
                                }
                                else
                                {
                                    prop.SetValue(contract, ((ILiteralNode)node).Value, null);
                                }
                                break;

                            case NodeType.Uri:
                                var uri = (IUriNode)value;

                                text = uri.Uri.ToString();
                                prop.SetValue(contract, text, null);

                                if (var == "Uri")
                                {
                                    contract.BaseDomain = uri.Uri.Authority.Replace("/", "");
                                    contract.ContractId = uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 2).ToString().Replace("/","");
                                    contract.Version = uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", "");
                                }

                                break;

                            default:
                                text = value.ToString();
                                prop.SetValue(contract, text, null);
                                break;
                        }
                    }
                }

                contracts.Add(contract);
            }

            return contracts;
        }  

        private Contract GetContract(string baseDomain, string contractId, string version, string publisher)
        {
            var queryString = new SparqlParameterizedString();
            queryString.CommandText = SelectBySubject;
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
                        var urinode = ((IUriNode) result.Value(var));

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

                                if (predicate == "title")
                                {
                                    contract.Title = ((ILiteralNode)node).Value;
                                }

                                if (predicate == "created")
                                {
                                    if (node is DateNode)
                                    {
                                        contract.Created = ((DateNode) node).AsDateTime();
                                    }
                                }

                                if (predicate == "hasCurrencyValue")
                                {
                                    contract.Price = ((ILiteralNode)node).Value;
                                }
                                break;

                            case NodeType.Uri:
                                var uri = (IUriNode)value;
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
            var publisher = new Publisher {Name = publisherName};

            var dvPediaQueryString = new SparqlParameterizedString { CommandText = SelectDBpediaInfo };
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
            return View(GetContracts(null));
        }

        public ActionResult ContractDetail(string baseDomain, string contractId, string version, string publisher)
        {
            return View(GetContract(baseDomain, contractId, version, publisher));
        }

        public ActionResult SubjectDetail(string publisher)
        {
            var publisherViewModel = new PublisherViewModel
            {
                Publisher = GetPublisher(publisher),
                Contracts = GetContracts(publisher)
            };

            return View(publisherViewModel);
        }
    }
}