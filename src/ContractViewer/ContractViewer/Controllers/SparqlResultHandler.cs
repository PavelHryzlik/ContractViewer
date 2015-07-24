using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VDS.RDF.Query;
using System.Reflection;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace ContractViewer.Controllers
{
    public class SparqlResultHandler
    {
        public IEnumerable<T> GetContracts<T>(string query, string variable, string substituteValue)
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));

            var queryString = new SparqlParameterizedString { CommandText = query };
            queryString.SetUri(variable, new Uri(substituteValue));

            SparqlResultSet results = endpoint.QueryWithResultSet(queryString.ToString());

            var contracts = new List<T>();
            var contractType = typeof(T);

            foreach (SparqlResult result in results.Results)
            {
                var contract = Activator.CreateInstance(typeof(T));

                foreach (var var in result.Variables)
                {
                    PropertyInfo prop = contractType.GetProperty(var);

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
                                    PropertyInfo propUri = contractType.GetProperty("BaseDomain");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Authority.Replace("/", ""), null);

                                    propUri = contractType.GetProperty("ContractId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 2).ToString().Replace("/", ""), null);

                                    propUri = contractType.GetProperty("Version");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = contractType.GetProperty("AttachmentId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = contractType.GetProperty("AmendmentId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 1).ToString().Replace("/", ""), null);

                                    propUri = contractType.GetProperty("LocalID");
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

                contracts.Add((T)contract);
            }

            return contracts;
        }

        public IEnumerable<T> GetContracts<T>(string query, string publisherName)
        {
            var endpoint = new SparqlRemoteEndpoint(new Uri("http://student.opendata.cz/sparql"));

            SparqlResultSet results = null;
            if (String.IsNullOrEmpty(publisherName))
            {
                results = endpoint.QueryWithResultSet(query);
            }
            else
            {
                var queryString = new SparqlParameterizedString { CommandText = query };
                queryString.SetLiteral("publisher", publisherName);

                results = endpoint.QueryWithResultSet(queryString.ToString());
            }

            var contracts = new List<T>();
            var contractType = typeof(T);

            foreach (SparqlResult result in results.Results)
            {
                var contract = Activator.CreateInstance(typeof(T));
                if (!String.IsNullOrEmpty(publisherName))
                {
                    PropertyInfo prop = contractType.GetProperty("Publisher");
                    if (prop != null)
                        prop.SetValue(contract, publisherName, null);
                }

                foreach (var var in result.Variables)
                {
                    PropertyInfo prop = contractType.GetProperty(var);

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
                                    PropertyInfo propUri = contractType.GetProperty("BaseDomain");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Authority.Replace("/", ""), null);

                                    propUri = contractType.GetProperty("ContractId");
                                    if (propUri != null)
                                        propUri.SetValue(contract, uri.Uri.Segments.GetValue(uri.Uri.Segments.Length - 2).ToString().Replace("/", ""), null);

                                    propUri = contractType.GetProperty("Version");
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

                contracts.Add((T)contract);
            }

            return contracts;
        }
    }
}