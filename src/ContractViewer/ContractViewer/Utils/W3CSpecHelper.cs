using System;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Nodes;
using VDS.RDF.Parsing;

namespace ContractViewer.Utils
{
    public static class W3CSpecHelper
    {
        /// <summary>
        /// Format Boolean, DateTime, Date and Time by WRC spec
        /// </summary>
        /// <param name="node">Input node</param>
        /// <returns>Formated node</returns>
        public static INode FormatNode(INode node)
        {
            if (node != null &&
                node.NodeType == NodeType.Literal &&
                ((ILiteralNode)node).DataType != null)
            {
                switch (((ILiteralNode)node).DataType.ToString())
                {
                    case XmlSpecsHelper.XmlSchemaDataTypeBoolean:
                        var intBool = Int32.Parse(((ILiteralNode)node).Value);
                        return new BooleanNode(node.Graph, Convert.ToBoolean(intBool));

                    case XmlSpecsHelper.XmlSchemaDataTypeDateTime:
                        var dateTime = DateTime.Parse(((ILiteralNode)node).Value);
                        return new DateTimeNode(node.Graph, new DateTimeOffset(dateTime));
                    case XmlSpecsHelper.XmlSchemaDataTypeDate:
                        var date = DateTime.Parse(((ILiteralNode)node).Value);
                        return new DateNode(node.Graph, date);
                    case XmlSpecsHelper.XmlSchemaDataTypeTime:
                        return new TimeSpanNode(node.Graph, TimeSpan.Parse(((ILiteralNode)node).Value.Split('+').First()));
                    default:
                        return node;
                }
            }
            return node;
        }
    }
}