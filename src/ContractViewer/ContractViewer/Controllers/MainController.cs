using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using ContractViewer.Models;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Query;

namespace ContractViewer.Controllers
{
    public class MainController : Controller
    {
        private Contract GetContract(string baseDomain, string contractId, string version, string publisher)
        {
            var queryString = new SparqlParameterizedString { CommandText = SparqlQueryConstants.StudentOpenDataCz.GetContract };
            var contractUri = new Uri(String.Format("http://{0}/contract/{1}/{2}", baseDomain, contractId, version));
            queryString.SetUri("contract", contractUri);

            var endpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));
            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());

            var contract = new Contract
            {
                Uri = contractUri.ToString(),
                BaseDomain = baseDomain,
                ContractId = contractId,
                Version = version,
                Publisher = publisher
            };

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
                                    contract.AwardId = ((ILiteralNode)node).Value;
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
                                    contract.Document = uri.Uri.ToString();
                                }

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

            var slodQueryString = new SparqlParameterizedString { CommandText = SparqlQueryConstants.StudentOpenDataCz.GetPublisherByName };
            slodQueryString.SetLiteral("publisher", publisherName);

            var slodEndpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));
            SparqlResultSet slotResults = slodEndpoint.QueryWithResultSet(slodQueryString.ToString());

            foreach (SparqlResult result in slotResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("ic").ToString()))
                {
                    publisher.Ic = result.Value("ic").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("aresLink").ToString()))
                {
                    publisher.AresUrl = result.Value("aresLink").ToString();
                }
            }



            var lodEndpoint = new SparqlRemoteEndpoint(new Uri("http://linked.opendata.cz/sparql"));


            var lodGetOpeningHoursQueryString = new SparqlParameterizedString { CommandText = SparqlQueryConstants.LinkedOpenDataCz.GetBusinessEntityOpeningHours };
            lodGetOpeningHoursQueryString.SetUri("businessEntity", new Uri(publisher.AresUrl));
            



            SparqlResultSet lodGetOpeningHoursResults = lodEndpoint.QueryWithResultSet(lodGetOpeningHoursQueryString.ToString());

            var localPlaces = new List<LocalPlace>();

            foreach (SparqlResult result in lodGetOpeningHoursResults.Results)
            {

                if (!String.IsNullOrEmpty(result.Value("localPlace").ToString()))
                {
                    var localPlaceStr = result.Value("localPlace").ToString();

                    LocalPlace localPlace;
                    if (localPlaces.Any(l => l.Url == localPlaceStr))
                    {
                        localPlace = localPlaces.First(l => l.Url == localPlaceStr);
                    }
                    else
                    {
                        localPlace = new LocalPlace 
                        {
                            Url = localPlaceStr, 
                            OpeningHoursOfTheDay = new List<OpeningHoursOfTheDay>()
                        };

                        if (!String.IsNullOrEmpty(result.Value("streetAddress").ToString()))
                        {
                            localPlace.StreetAddress = result.Value("streetAddress").ToString();
                        }

                        if (!String.IsNullOrEmpty(result.Value("postalCode").ToString()))
                        {
                            localPlace.PostalCode = result.Value("postalCode").ToString();
                        } 
                    }

                    SetOpeningHours(result, localPlace.OpeningHoursOfTheDay);

                    if (localPlaces.All(l => l.Url != localPlaceStr))
                    {
                        localPlaces.Add(localPlace);
                    }
                } 
            }

            publisher.LocalPlaces = localPlaces;

            






            var dbpdQueryString = new SparqlParameterizedString { CommandText = SparqlQueryConstants.CsDbpediaOrg.GetPublisherInfo };
            dbpdQueryString.SetLiteral("publisher", publisherName);

            var dbpdEndpoint = new SparqlRemoteEndpoint(new Uri("http://cs.dbpedia.org/sparql"));
            SparqlResultSet dbPediaResults = dbpdEndpoint.QueryWithResultSet(dbpdQueryString.ToString());

            foreach (SparqlResult result in dbPediaResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("img").ToString()))
                {
                    publisher.ImageUrl = result.Value("img").ToString();
                }
            }

            return publisher;
        }


        public void SetOpeningHours(SparqlResult result, ICollection<OpeningHoursOfTheDay> openingHours)
        {            
            if (!String.IsNullOrEmpty(result.Value("dayOfWeek").ToString()))
            {
                var dayOfWeekStr = result.Value("dayOfWeek").ToString().Replace("http://purl.org/goodrelations/v1#", String.Empty);

                var openingHour = new OpeningHour();
                if (!String.IsNullOrEmpty(result.Value("open").ToString()) && !String.IsNullOrEmpty(result.Value("close").ToString()))
                {
                    openingHour = new OpeningHour
                    {
                        Open = ((TimeSpanNode)W3CSpecHelper.FormatNode(result.Value("open"))).AsTimeSpan(),
                        Close = ((TimeSpanNode)W3CSpecHelper.FormatNode(result.Value("close"))).AsTimeSpan()
                    };
                }
  
                var dayOfWeek = (DayOfWeekCz) Enum.Parse(typeof (DayOfWeekCz), dayOfWeekStr);

                if (openingHours.Any(d => d.DayOfWeek == dayOfWeek))
                {
                    openingHours.First(d => d.DayOfWeek == dayOfWeek).OpeningHours.Add(openingHour);
                }
                else
                {
                    openingHours.Add( new OpeningHoursOfTheDay
                    {
                        DayOfWeek = dayOfWeek,
                        OpeningHours = new List<OpeningHour> { openingHour }
                    });
                } 
            }
        }





        public ActionResult Index()
        {
            var handler = new SparqlResultHandler();

            return View(handler.GetContracts<Contract>(SparqlQueryConstants.StudentOpenDataCz.GetContracts, null));
        }

        public ActionResult SubjectDetail(string publisher)
        {
            var handler = new SparqlResultHandler();

            var publisherViewModel = new PublisherViewModel
            {
                Publisher = GetPublisher(publisher),
                Contracts = handler.GetContracts<Contract>(SparqlQueryConstants.StudentOpenDataCz.GetContractsByPublisherName, publisher)
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
                Parties = handler.GetContracts<Party>(SparqlQueryConstants.StudentOpenDataCz.GetPartiesByContract, "contract", contract.Uri),
                Attachments = handler.GetContracts<Attachment>(SparqlQueryConstants.StudentOpenDataCz.GetAttachmentsContract, "contract", contract.Uri),
                Amendments = handler.GetContracts<Amendment>(SparqlQueryConstants.StudentOpenDataCz.GetAmendmentsByContract, "contract", contract.Uri),
                Milestones = handler.GetContracts<Milestone>(SparqlQueryConstants.StudentOpenDataCz.GetMilestonesByContract, "contract", contract.Uri)
            };

            return View(contractViewModel);
        }
    }
}