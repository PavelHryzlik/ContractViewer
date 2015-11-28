using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using ContractViewer.Models;
using VDS.RDF.Query;
using System.Reflection;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace ContractViewer.Controllers
{
    public enum LocalNodeType
    {
        None,
        Uri,
        Literal
    }

    public class SparqlQueryHandler
    {
        public IEnumerable<T> GetEntities<T>(string query, LocalNodeType nodeType = LocalNodeType.None)
        {
            return GetEntities<T>(query, String.Empty, String.Empty, nodeType);
        }

        public IEnumerable<T> GetEntities<T>(string query, string variable, string substituteValue, LocalNodeType nodeType, string endpointUri = Constants.StudentOpenDataCz.SparqlEndpointUri)
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri(endpointUri));

            var queryString = new SparqlParameterizedString { CommandText = query };

            switch (nodeType)
            {
                case LocalNodeType.Uri:
                    queryString.SetUri(variable, new Uri(substituteValue));
                    break;
                case LocalNodeType.Literal:
                    queryString.SetLiteral(variable, substituteValue);
                    break;
            }

            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());

            var entities = new List<T>();
            var entityType = typeof(T);

            foreach (SparqlResult result in results.Results)
            {
                var contract = Activator.CreateInstance(typeof(T));

                if (!String.IsNullOrEmpty(substituteValue))
                {
                    PropertyInfo prop = entityType.GetProperty("Publisher");
                    if (prop != null)
                        prop.SetValue(contract, substituteValue, null);
                }

                foreach (var var in result.Variables)
                {
                    PropertyInfo prop = entityType.GetProperty(var);

                    if (prop != null)
                    {
                        string text;
                        INode value = result.Value(var);
                        if (value == null)
                            continue;

                        switch (value.NodeType)
                        {
                            case NodeType.Literal:
                                var node = W3CSpecHelper.FormatNode(value);

                                if (node is BooleanNode)
                                {
                                    prop.SetValue(contract, ((BooleanNode)node).AsBoolean() ? "Ano" : "Ne", null);
                                }
                                else if (node is DateNode)
                                {
                                    prop.SetValue(contract, ((DateNode)node).AsDateTime(), null);
                                }
                                else if (node is DateTimeNode)
                                {
                                    prop.SetValue(contract, ((DateTimeNode)node).AsDateTime(), null);
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
                                    PropertyInfo propUri = entityType.GetProperty("BaseDomain");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Authority.Replace("/", ""), null);

                                    propUri = entityType.GetProperty("ContractId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 2).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("Version");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("AttachmentId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("AmendmentId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = entityType.GetProperty("LocalID");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);
                                }

                                break;

                            default:
                                text = value.ToString();
                                prop.SetValue(contract, text, null);
                                break;
                        }
                    }
                }

                entities.Add((T)contract);
            }

            return entities;
        }

        public Contract GetContract(string baseDomain, string contractId, string version, string publisher)
        {
            var queryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetContract };
            var contractUri = new Uri(String.Format("http://{0}/contract/{1}/{2}", baseDomain, contractId, version));
            queryString.SetUri("contract", contractUri);

            var endpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));
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

                                if (predicate == "publicContract")
                                {
                                    contract.AwardUrl = uri.Uri.ToString();
                                }

                                break;
                        }
                    }
                }
            }

            return contract;
        }

        public Publisher GetPublisher(string publisherName)
        {
            var publisher = new Publisher { Name = publisherName };

            GetPublisherByName(ref publisher, publisherName);

            GetPublisherOpeningHours(ref publisher);

            GetPublisherCoordinates(ref publisher);

            GetPublisherImage(ref publisher, publisherName);

            return publisher;
        }

        public IEnumerable<Publisher> SetPublishers()
        {
            var subjects = new List<Publisher>();

            var slodQueryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetNumberOfContracts };

            var slodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));
            SparqlResultSet slotResults = slodEndpoint.QueryWithResultSet(slodQueryString.ToString());

            foreach (SparqlResult result in slotResults.Results)
            {
                var publisher = new Publisher();
                if (!String.IsNullOrEmpty(result.Value("Institute").ToString()))
                {
                    publisher.Name = result.Value("Institute").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("ContractSum").ToString()))
                {
                    publisher.NumberOfContracts = Convert.ToInt32(((ILiteralNode)result.Value("ContractSum")).Value);
                }

                //TODO
                if (publisher.Name == "Třebíč")
                {
                    publisher.GeoPoint = new GeoPoint { Latitude = (decimal)49.22, Longitude = (decimal)15.88 };
                }
                if (publisher.Name == "Děčín")
                {
                    publisher.GeoPoint = new GeoPoint {Latitude = (decimal) 50.7736, Longitude = (decimal) 14.1961};
                }

                subjects.Add(publisher);
            }

            return subjects;
        }

        private void GetPublisherByName(ref Publisher publisher, string publisherName)
        {
            var slodQueryString = new SparqlParameterizedString { CommandText = Constants.StudentOpenDataCz.GetPublisherByName };
            slodQueryString.SetLiteral("publisher", publisherName);

            var slodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.StudentOpenDataCz.SparqlEndpointUri));
            SparqlResultSet slotResults = slodEndpoint.QueryWithResultSet(slodQueryString.ToString());

            foreach (SparqlResult result in slotResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("publisher").ToString()))
                {
                    publisher.Url = result.Value("publisher").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("ic").ToString()))
                {
                    publisher.ID = result.Value("ic").ToString();
                }

                if (!String.IsNullOrEmpty(result.Value("aresLink").ToString()))
                {
                    publisher.AresUrl = result.Value("aresLink").ToString();
                }
            }
        }

        private void GetPublisherOpeningHours(ref Publisher publisher)
        {
            var lodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.LinkedOpenDataCz.SparqlEndpointUri));

            var lodGetOpeningHoursQueryString = new SparqlParameterizedString { CommandText = Constants.LinkedOpenDataCz.GetBusinessEntityOpeningHours };
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
                            OpeningHours = new SortedDictionary<DayOfWeekCz, ICollection<OpeningHour>>()
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

                    SetOpeningHours(result, localPlace.OpeningHours);

                    if (localPlaces.All(l => l.Url != localPlaceStr))
                    {
                        localPlaces.Add(localPlace);
                    }
                }
            }

            publisher.LocalPlaces = localPlaces;
        }

        private void SetOpeningHours(SparqlResult result, SortedDictionary<DayOfWeekCz, ICollection<OpeningHour>> openingHours)
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

                var dayOfWeek = (DayOfWeekCz)Enum.Parse(typeof(DayOfWeekCz), dayOfWeekStr);

                if (openingHours.Any(d => d.Key == dayOfWeek))
                {
                    openingHours[dayOfWeek].Add(openingHour);
                    openingHours[dayOfWeek] = openingHours[dayOfWeek].OrderBy(o => o.Open).ToList();
                }
                else
                {
                    openingHours[dayOfWeek] = new List<OpeningHour> {openingHour};
                }
            }
        }

        private void GetPublisherCoordinates(ref Publisher publisher)
        {
            var lodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.LinkedOpenDataCz.SparqlEndpointUri));

            var lodRuianLinkQueryString = new SparqlParameterizedString { CommandText = Constants.LinkedOpenDataCz.GetBusinessEntityRuianLink };
            lodRuianLinkQueryString.SetUri("businessEntity", new Uri(publisher.AresUrl));

            SparqlResultSet lodGetRuianLinkResults = lodEndpoint.QueryWithResultSet(lodRuianLinkQueryString.ToString());

            var sparqlResult = lodGetRuianLinkResults.FirstOrDefault(result => !String.IsNullOrEmpty(result.Value("ruianLink").ToString()));
            if (sparqlResult != null)
            {
                var rlodEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.RuianLinkedOpenDataCz.SparqlEndpointUri));

                var rlodCoordinatesQueryString = new SparqlParameterizedString { CommandText = Constants.RuianLinkedOpenDataCz.GetBusinessEntityCoordinates };
                rlodCoordinatesQueryString.SetUri("addressPoint", new Uri(sparqlResult.Value("ruianLink").ToString()));

                SparqlResultSet rlodCoordinatesResults = rlodEndpoint.QueryWithResultSet(rlodCoordinatesQueryString.ToString());

                foreach (SparqlResult result in rlodCoordinatesResults.Results)
                {
                    var geoPoint = new GeoPoint();
                    if (!String.IsNullOrEmpty(result.Value("latitude").ToString()))
                    {
                        geoPoint.Latitude = Convert.ToDecimal(result.Value("latitude").ToString(), CultureInfo.InvariantCulture);
                    }

                    if (!String.IsNullOrEmpty(result.Value("longitude").ToString()))
                    {
                        geoPoint.Longitude = Convert.ToDecimal(result.Value("longitude").ToString(), CultureInfo.InvariantCulture);
                    }
                    publisher.GeoPoint = geoPoint;
                }
            }
        }

        private void GetPublisherImage(ref Publisher publisher, string publisherName)
        {
            var dbpdQueryString = new SparqlParameterizedString { CommandText = Constants.CsDbpediaOrg.GetPublisherInfo };
            dbpdQueryString.SetLiteral("publisher", publisherName);

            var dbpdEndpoint = new SparqlRemoteEndpoint(new Uri(Constants.CsDbpediaOrg.SparqlEndpointUri));
            SparqlResultSet dbPediaResults = dbpdEndpoint.QueryWithResultSet(dbpdQueryString.ToString());

            foreach (SparqlResult result in dbPediaResults.Results)
            {
                if (!String.IsNullOrEmpty(result.Value("img").ToString()))
                {
                    publisher.ImageUrl = result.Value("img").ToString();
                }
            }
        }
    }
}