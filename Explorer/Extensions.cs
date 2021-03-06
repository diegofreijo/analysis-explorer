﻿using Microsoft.Win32;
using Model;
using Model.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Xml.Linq;

namespace Explorer
{
	static class Extensions
	{
		public static string ToFullDisplayName(this IMethodReference method)
		{
			var result = new StringBuilder();

			result.AppendFormat("{0}::{1}", method.ContainingType.GetFullName(), method.GenericName);

			var parameters = string.Join(", ", method.Parameters.Select(p => p.ToParameterString()));
			result.AppendFormat("({0})", parameters);

			return result.ToString();
		}

		public static string ToDisplayName(this IMethodReference method)
		{
			var result = new StringBuilder();

			result.Append(method.GenericName);

			var parameters = string.Join(", ", method.Parameters.Select(p => p.ToParameterString()));
			result.AppendFormat("({0})", parameters);

			return result.ToString();
		}

		private static string ToParameterString(this IMethodParameterReference parameter)
		{
			var kind = string.Empty;

			switch (parameter.Kind)
			{
				case MethodParameterKind.Out:
					kind = "out ";
					break;

				case MethodParameterKind.Ref:
					kind = "ref ";
					break;
			}

			return string.Format("{0}{1}", kind, parameter.Type);
		}

		public static IEnumerable<T> Join<T>(this IEnumerable<T> src, Func<T> separator)
		{
			var srcArr = src.ToArray();

			for (int i = 0; i < srcArr.Length; i++)
			{
				yield return srcArr[i];

				if (i < srcArr.Length - 1)
				{
					yield return separator();
				}
			}
		}

		public static Graph CreateGraphFromDGML(string dgml)
		{
			var vertices = new Dictionary<string, VertexViewModelBase>();
			var graph = new Graph();
			var xml = XDocument.Parse(dgml);
			var ns = xml.Root.Name.Namespace;
			var nodetag = string.Format("{{{0}}}Node", ns);
			var linktag = string.Format("{{{0}}}Link", ns);

			foreach (var node in xml.Descendants(nodetag))
			{
				var id = node.Attribute("Id").Value;
				var label = node.Attribute("Label").Value;
				var vertex = new VertexViewModelBase(id, label);

				SetAttribute(node, "Background", value => vertex.BackgroundColor = value);

				vertices.Add(id, vertex);
				graph.AddVertex(vertex);
			}

			foreach (var node in xml.Descendants(linktag))
			{
				var sourceId = node.Attribute("Source").Value;
				var targetId = node.Attribute("Target").Value;
				var source = vertices[sourceId];
				var target = vertices[targetId];
				var edge = new EdgeViewModelBase(source, target);

				SetAttribute(node, "Label", value => edge.Label = value);

				graph.AddEdge(edge);
			}

			return graph;
		}

		private static void SetAttribute(XElement node, string name, Action<string> action)
		{
			var attribute = node.Attribute(name);

			if (attribute != null)
			{
				action(attribute.Value);
			}
		}

		public static T FindVisualParent<T>(this DependencyObject child)
			where T : DependencyObject
		{
			T result = null;
			// get parent item
			var parentObject = VisualTreeHelper.GetParent(child);

			// we’ve reached the end of the tree
			if (parentObject != null)
			{
				// check if the parent matches the type we’re looking for
				T parent = parentObject as T;

				if (parent != null)
				{
					result = parent;
				}
				else
				{
					// use recursion to proceed with next level
					result = FindVisualParent<T>(parentObject);
				}
			}

			return result;
		}

		public static void SaveGraph(string kind, string name, string dgml)
		{
			var proposedFileName = string.Format("{0} - {1}.dgml", kind, name);

			var dialog = new SaveFileDialog()
			{
				FileName = GetSafeFileName(proposedFileName),
				Filter = "Directed Graph Markup Language files (*.dgml)|*.dgml|Text files (*.txt)|*.txt|All files (*.*)|*.*",
				InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
			};

			var ok = dialog.ShowDialog();

			if (ok.HasValue && ok.Value)
			{
				File.WriteAllText(dialog.FileName, dgml, Encoding.UTF8);
			}
		}

		public static string GetSafeFileName(string fileName)
		{
			//return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
			return string.Join("_", fileName.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries));
		}
	}
}
