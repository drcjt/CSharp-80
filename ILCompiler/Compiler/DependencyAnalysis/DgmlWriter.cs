using System.Xml;

namespace ILCompiler.Compiler.DependencyAnalysis
{
    internal class DgmlWriter : IDisposable, IDependencyGraphNodeVisitor, IDependencyGraphEdgeVisitor
    {
        private readonly XmlWriter _xmlWrite;
        private bool _done = false;

        public DgmlWriter(XmlWriter xmlWriter) 
        { 
            _xmlWrite = xmlWriter;
            _xmlWrite.WriteStartDocument();
            _xmlWrite.WriteStartElement("DirectedGraph", "http://schemas.microsoft.com/vs/2009/dgml");
        }

        public void WriteNodesAndEdges(DependencyGraphTraverser traverser)
        {
            _xmlWrite.WriteStartElement("Nodes");
            traverser.TraverseNodes();
            _xmlWrite.WriteEndElement();

            _xmlWrite.WriteStartElement("Links");
            traverser.TraverseEdges();
            _xmlWrite.WriteEndElement();
        }

        public static void WriteDependencyGraphToStream(Stream stream, IDependencyNode root)
        {
            XmlWriterSettings writerSettings = new()
            {
                Indent = true,
                IndentChars = " "
            };

            using (XmlWriter xmlWriter = XmlWriter.Create(stream, writerSettings))
            {
                using (var dgmlWriter = new DgmlWriter(xmlWriter))
                {
                    var nodeVisitor = new DependencyGraphTraverser(dgmlWriter, dgmlWriter, root);
                    dgmlWriter.WriteNodesAndEdges(nodeVisitor);
                }
            }
        }

        public void Close()
        {
            if (!_done)
            {
                _done = true;
                _xmlWrite.WriteStartElement("Properties");
                {
                    _xmlWrite.WriteStartElement("Property");
                    _xmlWrite.WriteAttributeString("Id", "Label");
                    _xmlWrite.WriteAttributeString("Label", "Label");
                    _xmlWrite.WriteAttributeString("DataType", "String");
                    _xmlWrite.WriteEndElement();

                    _xmlWrite.WriteStartElement("Property");
                    _xmlWrite.WriteAttributeString("Id", "Reason");
                    _xmlWrite.WriteAttributeString("Label", "Reason");
                    _xmlWrite.WriteAttributeString("DataType", "String");
                    _xmlWrite.WriteEndElement();
                }
                _xmlWrite.WriteEndElement();

                _xmlWrite.WriteEndElement();
                _xmlWrite.WriteEndDocument();
            }
        }

        void IDisposable.Dispose()
        {
            Close();
        }

        private readonly Dictionary<IDependencyNode, int> _nodeMappings = new();
        private int _nodeNextId = 0;

        private void AddNode(IDependencyNode node)
        {
            AddNode(node, node.Name);
        }

        private void AddNode(IDependencyNode node, string label)
        {
            int nodeId = _nodeNextId++;

            _nodeMappings.Add(node, nodeId);

            _xmlWrite.WriteStartElement("Node");
            _xmlWrite.WriteAttributeString("Id", nodeId.ToString());
            _xmlWrite.WriteAttributeString("Label", label);
            _xmlWrite.WriteEndElement();
        }

        public void VisitNode(IDependencyNode node)
        {
            AddNode(node);
        }

        private readonly HashSet<Tuple<IDependencyNode, IDependencyNode>> _edgesVisited = new();

        public void VisitEdge(IDependencyNode depender, IDependencyNode dependedOn)
        {
            var edge = new Tuple<IDependencyNode, IDependencyNode>(depender, dependedOn);
            if (!_edgesVisited.Contains(edge))
            {
                _edgesVisited.Add(edge);

                _xmlWrite.WriteStartElement("Link");
                _xmlWrite.WriteAttributeString("Source", _nodeMappings[depender].ToString());
                _xmlWrite.WriteAttributeString("Target", _nodeMappings[dependedOn].ToString());
                _xmlWrite.WriteAttributeString("Stroke", "#00FF00");
                _xmlWrite.WriteEndElement();
            }
        }
    }
}
